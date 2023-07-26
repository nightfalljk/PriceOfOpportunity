using System.Collections.Generic;
using UnityEngine;

namespace Features.EventSystem
{
    public class NarrativeEvent
    {
        public int Probability => _probability;
        public string EventHeading { get; }
        public string EventStory { get; }
        public Sprite EventIcon { get; }
        public List<EventModifierType> ModifierTypes => _modifierTypes;
        
        private float _eventModifierValue;
        private List<EventModifierType> _modifierTypes;
        private Dictionary<EventModifierType, float> _modifierValues;
        private int _baseProbability;
        private int _additionalProbabilityAfterFail;
        private int _probability;
        

        public NarrativeEvent(NarrativeEventData narrativeEventData)
        {
            if (narrativeEventData.eventModifiers.Count != narrativeEventData.eventModifierValues.Count)
            {
                Debug.LogError("Number of event modifiers does not correspond to number of modifier values");
                return;
            }

            _modifierTypes = narrativeEventData.eventModifiers;
            _modifierValues = new Dictionary<EventModifierType, float>();
            for (int i = 0; i < narrativeEventData.eventModifiers.Count; i++)
            {
                _modifierValues.Add(narrativeEventData.eventModifiers[i], narrativeEventData.eventModifierValues[i]);
            }

            EventHeading = narrativeEventData.eventName;
            EventStory = narrativeEventData.storyContent;
            EventIcon = narrativeEventData.eventIcon;
            _probability = narrativeEventData.initProbability;
            _baseProbability = narrativeEventData.initProbability;
            _additionalProbabilityAfterFail = narrativeEventData.additionalProbabilityAfterFail;
        }

        public void IncreaseProbability()
        {
            _probability = Mathf.Min(_probability + _additionalProbabilityAfterFail, 100);
        }

        public void ResetProbability(int legalityScore)
        {
            _probability = Mathf.Min(_baseProbability + (50 - legalityScore/2), 100);
        }

        public float GetModifierValue(EventModifierType modifierType)
        {
            return _modifierValues[modifierType];
        }


    }
}