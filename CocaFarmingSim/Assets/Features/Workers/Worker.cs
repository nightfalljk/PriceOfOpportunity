using System;
using Features.EventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Workers
{
    public class Worker : MonoBehaviour
    {
        private static int workerCount = 0;

        public event Action<Worker> WorkerFired;
        public enum WorkerType
        {
            FARMER,
            LABORER
        }
        private string _workerName;
        private WorkerType _type;
        private float _salary;
        private float _workEfficiency;
        private const float MaxWorkEfficiency = 2;
        private float _happiness;
        private const float MaxHappiness = 2;
        private float _maxLabor;
        private float _maxLaborPerSecond;
        private float _currentLabor;
        private float _laborRechargePerSecond;

        private void Awake()
        {
            workerCount++;
            _workerName = "Worker " + workerCount;
            _workEfficiency = 1;
            _happiness = 1;
        }

        private void Start()
        {

            NarrativeEventSystem.Instance.workerEfficiencyEvent += WorkEfficiencyEvent;
            NarrativeEventSystem.Instance.workerHappinessEvent += WorkerHappinessEvent;
        }

        private void Update()
        {
            RechargeLabor();
        }

        private void WorkerHappinessEvent(float amount)
        {
            _happiness = Mathf.Min(_happiness + amount, MaxHappiness);
        }

        private void WorkEfficiencyEvent(float amount)
        {
            _workEfficiency = Mathf.Min(_workEfficiency + amount, MaxWorkEfficiency);
        }

        private void RechargeLabor()
        {
            if (_currentLabor < _maxLabor)
            {
                _currentLabor += (_laborRechargePerSecond * _happiness) * Time.deltaTime * GameTimeManager.Instance.GameSpeed;
            }
        }

        public void InitVariables()
        {
            _currentLabor = _maxLabor;
        }

        public void Fire()
        {
            workerCount--;
            NarrativeEventSystem.Instance.workerEfficiencyEvent -= WorkEfficiencyEvent;
            NarrativeEventSystem.Instance.workerHappinessEvent -= WorkerHappinessEvent;
            WorkerFired?.Invoke(this);
        }

        public float ProvideLaborPerSecond()
        {
            float providedLabor = Mathf.Min(_maxLaborPerSecond, _currentLabor);
            providedLabor = providedLabor * _workEfficiency * Time.deltaTime * GameTimeManager.Instance.GameSpeed;
            _currentLabor -= providedLabor;
            return providedLabor;
        }

        public float Salary
        {
            get => _salary;
            set => _salary = value;
        }

        public float MaxLabor
        {
            get => _maxLabor;
            set => _maxLabor = value;
        }

        public float LaborRechargeRatePerSecond
        {
            get => _laborRechargePerSecond;
            set => _laborRechargePerSecond = value;
        }
        public string WorkerName => _workerName;

        public WorkerType Type
        {
            get => _type;
            set => _type = value;
        }
        
    }
}
