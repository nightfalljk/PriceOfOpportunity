using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Features.EventSystem
{
    public class NarrativeEventSystem : MonoBehaviour
    {

        [SerializeField] private GameObject narrativeUIElement;
        [SerializeField] private TMP_Text narrativeUIName;
        [SerializeField] private TMP_Text narrativeUIContent;
        [SerializeField] private List<NarrativeEventData> narrativeEventsData;
        [SerializeField] private Image narrativeUIImage;

        // Legality score of 100 means no illegal activity, 0 a lot of illegal activity 
        private int _legalityScore;
        public static NarrativeEventSystem Instance { get; private set; }
        
        public event Action<float> workerEfficiencyEvent;
        public event Action<float> workerHappinessEvent;
        public event Action<float> workerLossEvent;
        public event Action<float> illegalBuildingDestructionEvent;
        public event Action<float> legalBuildingDestructionEvent;
        //TODO: Storage currently cannot distinguish between legal and illegal resources, might be worth changing
        //public event Action<float> illegalResourceDestructionEvent;
        //public event Action<float> legalResourceDestructionEvent;
        public event Action<float> resourceDestructionEvent;
        public event Action<float> moneyLossEvent; 

        private Dictionary<int, Action> _modifierEvents;
        private List<NarrativeEvent> _narrativeEvents;
        private Dictionary<NarrativeEvent, float> _eventProbabilities;
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

            _narrativeEvents = new List<NarrativeEvent>();

            foreach (var data in narrativeEventsData)
            {
                NarrativeEvent narrativeEvent = new NarrativeEvent(data);
                _narrativeEvents.Add(narrativeEvent);
            }
            _narrativeEvents.Shuffle();
            _legalityScore = 100;
        }

        private void Start()
        {
            GameTimeManager.Instance.MonthEnded += DrawEvent;
        }

        public void DrawEvent()
        {
            foreach (var narrativeEvent in _narrativeEvents)
            {
                int roll = Random.Range(0, 101);
                if (roll > narrativeEvent.Probability)
                {
                    narrativeEvent.IncreaseProbability();
                }
                else
                {
                    foreach (var type in narrativeEvent.ModifierTypes)
                    {
                        InvokeModifierEvent(type, narrativeEvent.GetModifierValue(type));
                    }
                    
                    DisplayEvent(narrativeEvent);
                    //TODO: Need to add continue button that unpauses game and hides narrative event

                    _narrativeEvents.Remove(narrativeEvent);
                    _narrativeEvents.Add(narrativeEvent);
                    narrativeEvent.ResetProbability(_legalityScore);
                    break;
                }
            }
        }

        public void DisplayEvent(NarrativeEvent narrativeEvent)
        {
            GameTimeManager.Instance.PauseGame();
            narrativeUIName.text = narrativeEvent.EventHeading;
            narrativeUIContent.text = narrativeEvent.EventStory;
            narrativeUIImage.sprite = narrativeEvent.EventIcon;
            narrativeUIElement.SetActive(true);
        }

        public void DecreaseLegalityScore(int amount)
        {
            _legalityScore = Mathf.Max(0, _legalityScore - amount);
        }

        public void IncreaseLegalityScore(int amount)
        {
            _legalityScore = Mathf.Min(_legalityScore + amount, 100);
        }

        public void InvokeModifierEvent(EventModifierType modifierType, float modifierValue)
        {
            switch (modifierType)
            {
                case EventModifierType.WorkEfficiency:
                    workerEfficiencyEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.WorkerHappiness:
                    workerHappinessEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.WorkerLoss:
                    workerLossEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.LegalBuildingDestruction:
                    legalBuildingDestructionEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.IllegalBuildingDestruction:
                    illegalBuildingDestructionEvent?.Invoke(modifierValue);
                    break;
                /*case EventModifierType.LegalResourceDestruction:
                    legalResourceDestructionEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.IllegalResourceDestruction:
                    illegalResourceDestructionEvent?.Invoke(modifierValue);
                    break;*/
                case EventModifierType.ResourceDestruction:
                    resourceDestructionEvent?.Invoke(modifierValue);
                    break;
                case EventModifierType.MoneyLoss:
                    moneyLossEvent?.Invoke(modifierValue);
                    break;
                default:
                    break;
            }
        }
        
    }
}