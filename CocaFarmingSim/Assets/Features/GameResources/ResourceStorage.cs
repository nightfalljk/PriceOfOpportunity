using System;
using System.Collections.Generic;
using System.Linq;
using Features.EventSystem;
using UnityEngine;

namespace Features.GameResources
{
    public class ResourceStorage : MonoBehaviour
    {
        public static ResourceStorage Instance { get; private set; }
        public event Action ResourceAvailable;
    
        [SerializeField] private Transform resourceUIOverview;
        [SerializeField] private GameObject resourceUIElementPrefab;
        [SerializeField] private List<GameResource> resourceData;

        private Dictionary<int, int> _storage;
        private Dictionary<int, GameResourceUIElement> _resourceUIElements;
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

            _storage = new Dictionary<int, int>();
            //TODO: Initiate these at start up and just set visibility to false -> should be fine with ui 
            _resourceUIElements = new Dictionary<int, GameResourceUIElement>();

            foreach (var element in resourceData)
            {
                InitResourceUIElement(element);
            }
        }

        private void Start()
        {
            NarrativeEventSystem.Instance.resourceDestructionEvent += OnResourceDestruction;
        }

        public void OnResourceDestruction(float percentage)
        {
            int totalStorage = 0;
            foreach (var key in _storage.Keys.ToList())
            {
                totalStorage += _storage[key];
            }
            int resourcesToDestroy = (int)(totalStorage * percentage);
            foreach (var key in _storage.Keys.ToList())
            {
                if (resourcesToDestroy <= 0) return;
                
                int inStorage = _storage[key];
                if(inStorage == 0) continue;

                resourcesToDestroy -= inStorage;
                _storage[key] = Mathf.Max(0, -resourcesToDestroy);
            }
        }

        public void AddToStorage(GameResource.ResourceType resource, int amount)
        {
            if (_storage.ContainsKey((int)resource))
            {
                _storage[(int)resource] += amount;
            }
            else
            {
                _storage.Add((int)resource, amount);
                _resourceUIElements[(int)resource].gameObject.SetActive(true);
            }

            _resourceUIElements[(int)resource].SetAmount(_storage[(int)resource]);
            
            ResourceAvailable?.Invoke();
        }

        public bool CanRemoveFromStorage(GameResource.ResourceType resource, int amount)
        {
            if (!_storage.ContainsKey((int)resource)) return false;
            if (_storage[(int)resource] < amount) return false;
            return true;
        }

        public int RemoveFromStorage(GameResource.ResourceType resource, int amount)
        {
            _storage[(int)resource] -= amount;
            _resourceUIElements[(int)resource].SetAmount(_storage[(int)resource]);
            return amount;
        }

        private void InitResourceUIElement(GameResource resource)
        {
            GameResourceUIElement uiElement = Instantiate(resourceUIElementPrefab, resourceUIOverview)
                .GetComponent<GameResourceUIElement>();
            uiElement.SetName(resource.resourceName);
            uiElement.SetResourceType(resource);
            uiElement.SetSellPrice(resource.buyPrice, resource.sellPrice);
            uiElement.SetAmount(0);
            _resourceUIElements.Add((int)resource.type, uiElement);
        }
    }
}
