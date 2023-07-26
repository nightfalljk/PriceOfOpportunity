using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.GameResources
{
    public class GameResourceUIElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourceNameElement;
        [SerializeField] private TMP_Text resourceAmountElement;
        [SerializeField] private TMP_Text resourceSellPriceElement;

        private GameResource _resource;

        public void SetResourceType(GameResource resource)
        {
            _resource = resource;
        }

        public void SetName(string resourceName)
        {
            resourceNameElement.text = resourceName;
        }
        
        public void SetAmount(int amount)
        {
            resourceAmountElement.text = amount.ToString();
        }

        //TODO: Add buy price to this and indicate buy/sell prices in different colors?
        public void SetSellPrice(float buyPrice, float sellPrice)
        {
            resourceSellPriceElement.text = buyPrice.ToString() + " / " +  sellPrice.ToString();
        }
        
        //TODO: Add a slider UI element and set min/max value between 0 and storage amount for selling, 0 and maxBuyAmount for buying
        //TODO: Toggle slider when clicking -> basically same as with hiring workers

        public void BuyResource(int amount)
        {
            if(PlayerController.CashMoney < amount * _resource.buyPrice) return;
            
            ResourceStorage.Instance.AddToStorage(_resource.type, amount);
            PlayerController.CashMoney -= amount * _resource.buyPrice;
        }

        public void SellResource(int amount)
        {
            if (!ResourceStorage.Instance.CanRemoveFromStorage(_resource.type, amount)) return;

            ResourceStorage.Instance.RemoveFromStorage(_resource.type, amount);
            PlayerController.CashMoney += amount * _resource.sellPrice;
        }
            
    }
}