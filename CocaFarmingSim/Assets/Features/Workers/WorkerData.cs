using UnityEngine;

namespace Features.Workers
{
    [CreateAssetMenu(menuName = "Workers/Create Worker")]
    public class WorkerData : ScriptableObject
    {
        public GameObject workerPrefab;
        public GameObject workerUIElementPrefab;
        public Worker.WorkerType type;
        public float minSalary;
        public float maxSalary;
        public float baseLaborRechargePerSecond;
        public float maxLabor;
    }
}