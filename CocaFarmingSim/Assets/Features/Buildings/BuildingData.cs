using UnityEngine;


namespace Features.Buildings
{
    [CreateAssetMenu(menuName = "Buildings/Create Building")]
    public class BuildingData : ScriptableObject
    {
        public GameObject buildingPrefab;
        [TextArea] public string tooltipText;
        public int price;

        public override string ToString()
        {
            return tooltipText + "\nCost: " + price;
        }
    }
}
