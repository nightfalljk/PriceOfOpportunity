using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameTimeManager : MonoBehaviour
{
    public event Action MonthEnded;
    
    private const int SpeedFactor = 2;
    private const int MaxSpeed = 4;
    private const float MinSpeed = 1;

    [SerializeField] private float timePerMonth;
    private float _gameSpeed;
    private float _previousGameSpeed;
    private float _gameTime;
    private float _timeSinceLastMonth;

    private float _currentMonth;
    private float _currentYear;

    public static GameTimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _gameSpeed = 1;
        _previousGameSpeed = _gameSpeed;
        _gameTime = 0;
        _timeSinceLastMonth = 0;
        _currentYear = 1;
        _currentMonth = 1;
        MonthEnded += CalcCurrentMonthAndYear;
    }

    private void Update()
    {
        if (IsGamePaused()) return;
        
        _gameTime += Time.deltaTime * GameSpeed;
        _timeSinceLastMonth += Time.deltaTime * GameSpeed;
        if (timePerMonth < _timeSinceLastMonth)
        {
            _timeSinceLastMonth = 0;
            MonthEnded?.Invoke();
        }

    }

    public void IncreaseGameSpeed()
    {
        _gameSpeed = Mathf.Min(_gameSpeed * SpeedFactor, MaxSpeed);
    }

    public void DecreaseGameSpeed()
    {
        _gameSpeed = Mathf.Max(MinSpeed, _gameSpeed / SpeedFactor);
    }

    public void TogglePauseStatus()
    {
        if(_gameSpeed == 0)
            UnpauseGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if(_gameSpeed != 0)
            _previousGameSpeed = _gameSpeed;
        _gameSpeed = 0;
    }

    public void UnpauseGame()
    {
        _gameSpeed = _previousGameSpeed;
    }

    public bool IsGamePaused()
    {
        return _gameSpeed == 0;
    }

    public void ResetGameSpeed()
    {
        _gameSpeed = 1;
    }

    private void CalcCurrentMonthAndYear()
    {
        _currentMonth++;
        if (_currentMonth <= 12) return;
        _currentYear++;
        _currentMonth %= 12;
    }
    
    public float GameSpeed => _gameSpeed;

    public float CurrentMonth => _currentMonth;

    public float CurrentYear => _currentYear;
}
