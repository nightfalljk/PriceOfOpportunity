using System;
using System.Collections;
using System.Collections.Generic;
using Features.Buildings;
using Features.EventSystem;
using Features.Workers;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform constructionTarget;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float initMoney = 1000;
    [SerializeField] private Transform buildings;
    [FormerlySerializedAs("_workerManager")] [SerializeField] private WorkerManager workerManager;

    public static float CashMoney = 0;
    public event Action GameOver;

    private PlayerInputActions _input;
    private Camera _camera;
    
    private bool _inBuildMode = false;
    private Building _currentBuilding;
    
    // TODO: Add ground layer mask for casting against floor when building?

    private void Awake()
    {
        CashMoney = initMoney;
        _camera = Camera.main;
        _input = new PlayerInputActions();
    }

    private void Start()
    {
        _input.Building.Cancel.performed += CancelBuilding;
        _input.Building.Confirm.performed += ConfirmBuilding;
        _input.TimeScaling.IncreaseGameSpeed.performed += context => { GameTimeManager.Instance.IncreaseGameSpeed(); };
        _input.TimeScaling.DecreaseGameSpeed.performed += context => { GameTimeManager.Instance.DecreaseGameSpeed(); };
        _input.TimeScaling.PauseGame.performed += context => { GameTimeManager.Instance.TogglePauseStatus(); };

        NarrativeEventSystem.Instance.moneyLossEvent += LoseMoney;
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.TimeScaling.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
        _input.TimeScaling.Disable();
    }

    private void Update()
    {
        MoveCamera();
        BuildMode();
        if (CashMoney <= 0)
        {
            GameOver?.Invoke();
        }
    }

    private void MoveCamera()
    {
        Vector2 inputDir = _input.Player.MoveCamera.ReadValue<Vector2>().normalized;
        if(inputDir == Vector2.zero) return;

        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;
        Vector3 moveDir = forward * inputDir.y + right * inputDir.x;
        moveDir.y = 0;
        moveDir.Normalize();
        cameraTarget.Translate(moveDir * (Time.deltaTime * cameraSpeed));
    }

    public Vector2 GetMousePosition()
    {
        return _input.Player.Mouse.ReadValue<Vector2>();
    }

    public void EnterBuildMode(BuildingData buildingData)
    {
        if(buildingData.price > CashMoney) return;
        _inBuildMode = true;
        //Cursor.visible = false;
        _currentBuilding = Instantiate(buildingData.buildingPrefab, constructionTarget).GetComponent<Building>();
        _currentBuilding.SetBuildingData(buildingData);
        _input.Building.Enable();
    }

    private void BuildMode()
    {
        if (!_inBuildMode) return;
        
        Vector2 mousePos = _input.Building.BuildingPosition.ReadValue<Vector2>();
        Ray mouseRay = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        if (Physics.Raycast(mouseRay, out hit))
        {
            Vector3 targetPos = new Vector3(Mathf.RoundToInt(hit.point.x), constructionTarget.transform.position.y, Mathf.RoundToInt(hit.point.z));
            constructionTarget.position = targetPos;
        }
    }

    private void ConfirmBuilding(InputAction.CallbackContext ctx)
    {
        if(!_inBuildMode) return;
        if (_currentBuilding.CanBuild)
        {
            _currentBuilding.Build();
            _currentBuilding.transform.SetParent(buildings);
            _currentBuilding.ProductReady += workerManager.QueueWork;
            _currentBuilding.ProductGathered += workerManager.DequeueGatheredProductWork;
            //Cursor.visible = true;
            _input.Building.Disable();
            _inBuildMode = false;
        }

    }

    private void CancelBuilding(InputAction.CallbackContext ctx)
    {
        if (!_inBuildMode) return;
        _inBuildMode = false;
        //Cursor.visible = true;
        Destroy(_currentBuilding);
        _input.Building.Disable();
    }
    
    private void LoseMoney(float amount)
    {
        CashMoney -= amount;
        if (amount <= 0)
        {
            GameOver?.Invoke();
        }
    }
}
