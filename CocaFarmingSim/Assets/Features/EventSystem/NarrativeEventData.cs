using System.Collections.Generic;
using UnityEngine;

namespace Features.EventSystem
{
    [CreateAssetMenu(fileName = "NarrativeEvent", menuName = "Events/Create Narrative Event")]
    public class NarrativeEventData : ScriptableObject
    {
        public string eventName;
        [TextArea] public string storyContent;
        public Sprite eventIcon;
        public List<EventModifierType> eventModifiers;
        public List<float> eventModifierValues;
        [Range(0,100)] public int initProbability;
        [Range(0, 100)] public int additionalProbabilityAfterFail;
    }
}