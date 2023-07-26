using System;
using System.Collections.Generic;
using Features.Buildings;
using Features.GameResources;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Workers
{
    public class WorkerManager : MonoBehaviour
    {
        [SerializeField] private WorkerData farmerData;
        [SerializeField] private WorkerData laborerData;
        [SerializeField] private int maxWorkers;
        [SerializeField] private Slider hiringRangeSlider;
        [SerializeField] private Transform workerOverviewUI;

        private WorkerData _nextHire;
        private float _nextHireSalary;
        
        private List<Worker> _laborers;
        private List<Worker> _farmers;

        private List<Building> _productionBuildings;
        private List<Building> _farmBuildings;

        private void Awake()
        {
            _laborers = new List<Worker>();
            _farmers = new List<Worker>();
            _productionBuildings = new List<Building>();
            _farmBuildings = new List<Building>();
        }

        private void Start()
        {
            GameTimeManager.Instance.MonthEnded += PayWorkers;
        }

        private void Update()
        {
            ProvideWork();
        }

        private void ProvideWork()
        {
            if(_productionBuildings.Count == 0 && _farmBuildings.Count == 0) return;
            
            ProductionLabor();
            FarmLabor();
        }

        private void ProductionLabor()
        {
            int laborersAvailable = _laborers.Count;
            for (int i = 0; i < _productionBuildings.Count; i++)
            {
                if (laborersAvailable <= 0) return;
                int requiredWorkers = _productionBuildings[i].MaxWorkers;
                for (int j = 0; j < requiredWorkers; j++)
                {
                    laborersAvailable--;
                    bool productGathered = _productionBuildings[i]
                        .GatherProduct(_laborers[laborersAvailable].ProvideLaborPerSecond());
                    if (productGathered || laborersAvailable == 0) return;
                }
            }
        }

        private void FarmLabor()
        {
            int farmersAvailable = _farmers.Count;
            for (int i = 0; i < _farmBuildings.Count; i++)
            {
                //Debug.Log("Farmers Available: " + farmersAvailable);
                if (farmersAvailable <= 0) return;
                int requiredWorkers = _farmBuildings[i].MaxWorkers;
                for (int j = 0; j < requiredWorkers; j++)
                {
                    farmersAvailable--;
                    bool productGathered = _farmBuildings[i].GatherProduct(_farmers[farmersAvailable].ProvideLaborPerSecond());
                    if (productGathered || farmersAvailable == 0) return;
                }
            }
        }

        public void SetNextHireLaborer()
        {
            _nextHire = laborerData;
            hiringRangeSlider.minValue = laborerData.minSalary;
            hiringRangeSlider.maxValue = laborerData.maxSalary;
        }

        public void SetNextHireFarmer()
        {
            _nextHire = farmerData;
            hiringRangeSlider.minValue = farmerData.minSalary;
            hiringRangeSlider.maxValue = farmerData.maxSalary;
        }

        public void SetNextHireSalary(float salary)
        {
            _nextHireSalary = salary;
        }

        public void HireWorker()
        {
            if(!CanHire()) return;
            Worker worker = InstantiateWorker();
            InstantiateWorkerUIElement(worker);
            
            List<Worker> workers = GetWorkerListByType(worker);
            workers.Add(worker);
        }

        private Worker InstantiateWorker()
        {
            Worker worker = Instantiate(_nextHire.workerPrefab, transform).GetComponent<Worker>();
            worker.Type = _nextHire.type;
            worker.Salary = _nextHireSalary;
            worker.MaxLabor = _nextHire.maxLabor;
            worker.LaborRechargeRatePerSecond = _nextHire.baseLaborRechargePerSecond;
            worker.InitVariables();
            worker.WorkerFired += FireWorker;
            return worker;
        }

        private void InstantiateWorkerUIElement(Worker worker)
        {
            WorkerUIElement workerUIElement = Instantiate(_nextHire.workerUIElementPrefab, workerOverviewUI)
                .GetComponent<WorkerUIElement>();
            workerUIElement.SetWorkerName(worker.WorkerName);
            workerUIElement.SetWorkerType(worker.Type);
            workerUIElement.SetWorkerSalary(worker.Salary.ToString());
            workerUIElement.SetAssociatedWorker(worker);
        }

        public void FireWorker(Worker worker)
        {
            List<Worker> workers = GetWorkerListByType(worker);
            workers.Remove(worker);
            Destroy(worker.gameObject);
        }

        public void QueueWork(Building producer)
        {
            List<Building> producerList = GetBuildingListByType(producer);
            producerList.Add(producer);
        }

        public void DequeueGatheredProductWork(Building producer, int amount)
        {
            List<Building> producerList = GetBuildingListByType(producer);
            producerList.Remove(producer);
            ResourceStorage.Instance.AddToStorage(producer.OutputResource.type, amount);
        }

        public List<Worker> GetWorkerListByType(Worker worker)
        {
            return worker.Type == Worker.WorkerType.FARMER ? _farmers : _laborers;
        }

        public List<Building> GetBuildingListByType(Building building)
        {
            return building.WorkerType == Worker.WorkerType.FARMER ? _farmBuildings : _productionBuildings;
        }

        public void PayWorkers()
        {
            foreach (var worker in _farmers)
            {
                PlayerController.CashMoney -= worker.Salary;
            }            
            foreach (var worker in _laborers)
            {
                PlayerController.CashMoney -= worker.Salary;
            }
        }
        public bool CanHire()
        {
            return _farmers.Count + _laborers.Count < maxWorkers;
        }
    }
}