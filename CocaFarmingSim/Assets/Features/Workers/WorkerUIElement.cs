using System;
using Features.EventSystem;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Workers
{
    public class WorkerUIElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text workerName;
        [SerializeField] private TMP_Text workerType;
        [SerializeField] private TMP_Text workerSalary;

        private Worker _associatedWorker = null;

        private void Start()
        {
            NarrativeEventSystem.Instance.workerLossEvent += WorkerLossThroughEvent;
        }
        
        private void WorkerLossThroughEvent(float probability)
        {
            int roll = Random.Range(0, 101);
            if ( roll <= probability)
            {
                FireWorker();
            }
        }

        public void SetWorkerName(string workerName)
        {
            this.workerName.text = workerName;
        }
        
        public void SetWorkerType(Worker.WorkerType workerType)
        {
            string workerTypeString = workerType == Worker.WorkerType.FARMER ? "Farmer" : "Laborer";
            this.workerType.text = workerTypeString;
        }
        
        public void SetWorkerSalary(string workerSalary)
        {
            this.workerSalary.text = workerSalary;
        }

        public void SetAssociatedWorker(Worker worker)
        {
            _associatedWorker = worker;
        }

        public void FireWorker()
        {
            NarrativeEventSystem.Instance.workerLossEvent -= WorkerLossThroughEvent;
            _associatedWorker.Fire();
            Destroy(gameObject);
        }
    }
}