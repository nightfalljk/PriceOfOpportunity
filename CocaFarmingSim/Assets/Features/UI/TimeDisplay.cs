using System;
using TMPro;
using UnityEngine;

namespace Features.UI
{
    public class TimeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textAsset;
        private void Start()
        {
            GameTimeManager.Instance.MonthEnded += UpdateTimeDisplay;
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            textAsset.text = String.Format("Year {0}, Month {1}", GameTimeManager.Instance.CurrentYear,
                GameTimeManager.Instance.CurrentMonth);
        }
    }
}