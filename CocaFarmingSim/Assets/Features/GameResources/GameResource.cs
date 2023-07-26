using UnityEngine;
using UnityEngine.Serialization;

namespace Features.GameResources
{
    [CreateAssetMenu(menuName = "Resources/Create Resource")]
    public class GameResource : ScriptableObject
    {
        [FormerlySerializedAs("resourceType")] public ResourceType type;
        public string resourceName;
        public Legality legality;
        public int legalityDecrease;
        public float sellPrice;
        public float buyPrice;
        public float workPerUnit;
        public int minYield;
        public int maxYield;
        public float productionTime;
        public bool canSpoil;
        public float spoilTime;
        
        public enum ResourceType
        {
            Coca,
            CocaPaste,
            Coffee,
            Corn,
            Meal,
            Wheat,
            Gasoline
        }
    }
}