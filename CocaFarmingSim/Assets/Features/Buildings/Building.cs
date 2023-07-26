using System;
using Features.EventSystem;
using Features.GameResources;
using Features.Workers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Buildings
{
    public class Building : MonoBehaviour
    {
        //TODO: Add UI element that displays information about building when player selects building by clicking it, i.e. duration until resource is ready etc
        
        [SerializeField] private GameResource[] inputResources;
        [SerializeField] private GameResource outputResource;
        [SerializeField] private Worker.WorkerType workerType;
        [SerializeField] private int maxWorkers;

        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material canBuildMaterial;
        [SerializeField] private Material blockedBuildMaterial;

        [SerializeField] private GameObject popUpText;
    
        public event Action<Building> ProductReady;
        public event Action<Building, int> ProductGathered;

        private float _productionTime;
        private float _spoilTime;
        private bool _hasProduced;
        private bool _hasSpoiled;
        private float _workNeeded;
        private float _workLeft;
        private float _yield;
        private bool _producing;

        private Legality _legality;
        private int _legalityDecreaseNumber;

        private bool _isBuilt;
        private bool _canBuild;
        private BuildingData _buildingData;

        private MeshRenderer _meshRenderer;
        
        private void Awake()
        {
            _isBuilt = false;
            _canBuild = true;
            _hasProduced = false;
            _hasSpoiled = false;
            _producing = false;
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = canBuildMaterial;
            AssignLegality();
        }

        private void Start()
        {
            ResourceStorage.Instance.ResourceAvailable += StartProduction;
            NarrativeEventSystem.Instance.legalBuildingDestructionEvent += DestroyBuildingThroughEvent;
            if (_legality == Legality.Illegal)
            {
                NarrativeEventSystem.Instance.DecreaseLegalityScore(_legalityDecreaseNumber);
                NarrativeEventSystem.Instance.illegalBuildingDestructionEvent += DestroyBuildingThroughEvent;
            }
        }

        private void DestroyBuildingThroughEvent(float probability)
        {
            int roll = Random.Range(0, 101);
            if (roll <= probability)
            {
                NarrativeEventSystem.Instance.legalBuildingDestructionEvent -= DestroyBuildingThroughEvent;
                if (_legality == Legality.Illegal)
                    NarrativeEventSystem.Instance.illegalBuildingDestructionEvent -= DestroyBuildingThroughEvent;
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            ProduceProduct();
            SpoilProduct();
        }
        
        private void AssignLegality()
        {
            _legalityDecreaseNumber = 0;
            foreach (var resource in inputResources)
            {
                if (resource.legality == Legality.Illegal)
                {
                    _legality = Legality.Illegal;
                    _legalityDecreaseNumber += resource.legalityDecrease;
                }
            }

            if (outputResource.legality == Legality.Illegal)
            {
                _legality = Legality.Illegal;
                _legalityDecreaseNumber += outputResource.legalityDecrease;
            }
        }

        private void ProduceProduct()
        {
            if(!_isBuilt) return;
            
            _productionTime -= Time.deltaTime * GameTimeManager.Instance.GameSpeed;
            if (_productionTime < 0 && !_hasProduced)
            {
                _hasProduced = true;
                ProductReady?.Invoke(this);
                _yield = Random.Range(outputResource.minYield, outputResource.maxYield);
                _workNeeded = _yield * outputResource.workPerUnit;
                _workLeft = _workNeeded;
                if (popUpText)
                {
                    var msg = "Ready for Harvest";
                    SpawnPopUpText(msg);
                }
                if (outputResource.canSpoil)
                {
                    _spoilTime = outputResource.spoilTime;
                }
            }
        }

        public bool GatherProduct(float providedWork)
        {
            _workLeft -= providedWork;
            if (_workLeft <= 0)
            {
                ProductGathered?.Invoke(this, (int)_yield);
                _producing = false;
                StartProduction();
                print("Gather");
                if (popUpText)
                {
                    print("PopUp");
                    var msg = "+ " + outputResource.resourceName;
                    SpawnPopUpText(msg);
                }
                return true;
            }

            return false;
        }

        private void SpoilProduct()
        {
            if(!_isBuilt) return;
            if (!outputResource.canSpoil) return;
            if(!_hasProduced) return;
            
            if (_spoilTime < 0 && !_hasSpoiled)
            {
                _hasSpoiled = true;
                ProductGathered?.Invoke(this, (int)(_yield * (_workLeft/_workNeeded)));
                _producing = false;
                print("Spoil");
                if (popUpText && (int)(_yield * (_workLeft / _workNeeded)) > 0)
                {
                    print("PopUp");
                    var msg = "+ " + outputResource.resourceName;
                    SpawnPopUpText(msg);
                }
                StartProduction();
                return;
            }
            
            _spoilTime -= Time.deltaTime * GameTimeManager.Instance.GameSpeed;
            
        }

        public void Build()
        {
            _isBuilt = true;
            _meshRenderer.material = defaultMaterial;
            PlayerController.CashMoney -= _buildingData.price;
            StartProduction();
        }

        public void StartProduction()
        {
            if(_producing) return;
            foreach (var resource in inputResources)
            {
                if (!ResourceStorage.Instance.CanRemoveFromStorage(resource.type, 1)) return;
            }

            foreach (var resource in inputResources)
            {
                ResourceStorage.Instance.RemoveFromStorage(resource.type, 1);
            }
            _hasProduced = false;
            _hasSpoiled = false;
            _productionTime = outputResource.productionTime;
        }

        public void SpawnPopUpText(string msg)
        {
            var go = Instantiate(popUpText, transform.position, Camera.main.transform.rotation, transform);
            go.GetComponent<TextMesh>().text = msg;
        }

        private void OnDestroy()
        {
            ResourceStorage.Instance.ResourceAvailable -= StartProduction;
            NarrativeEventSystem.Instance.IncreaseLegalityScore(_legalityDecreaseNumber);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_isBuilt) return;
            _canBuild = false;
            _meshRenderer.material = blockedBuildMaterial;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isBuilt) return;
            _canBuild = true;
            _meshRenderer.material = canBuildMaterial;
        }

        public void SetBuildingData(BuildingData buildingData)
        {
            _buildingData = buildingData;
        }

        public bool CanBuild => _canBuild;
        public Worker.WorkerType WorkerType => workerType;
        public GameResource OutputResource => outputResource;

        public int MaxWorkers => maxWorkers;
    }
}
