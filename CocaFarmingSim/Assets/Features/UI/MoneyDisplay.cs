using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.UI
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textAsset;

        private void Update()
        {
            textAsset.text = "$" + PlayerController.CashMoney;
        }
    }
}