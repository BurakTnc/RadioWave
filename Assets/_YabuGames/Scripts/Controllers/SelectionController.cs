using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Controllers
{
    public class SelectionController : MonoBehaviour
    {
        public static SelectionController Instance;
        
        private Camera _cam; 
        [HideInInspector] public GameObject selectedRadio;
        [HideInInspector] public GameObject selectedState;
        public EventSystem m_EventSystem;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
            _cam=Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Selection();
            }
        }

        private void Selection()
        {
            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (m_EventSystem.IsPointerOverGameObject()) return;
            if (Physics.Raycast(ray, out var hit))
            {
                var objectHit = hit.transform;

                if (objectHit.TryGetComponent(out RadioController radio))
                {
                    if (selectedRadio)
                    {
                        selectedRadio = null;
                        CoreGameSignals.Instance.OnGrid?.Invoke(false);
                        return;
                    }
                    
                    selectedRadio = radio.gameObject;
                    if (selectedState)
                    {
                        CoreGameSignals.Instance.OnUpgrade?.Invoke(false);
                        selectedState.GetComponent<IInteractable>().Select();
                        selectedState = null;
                    }
                    CoreGameSignals.Instance.OnGrid?.Invoke(true);
                }

                if (objectHit.TryGetComponent(out IInteractable state))
                {
                    if (selectedRadio)
                    {
                        selectedState = objectHit.gameObject;
                        var stateController = selectedState.GetComponent<State>();
                        if(stateController.IsNoneSelectable()) 
                            return;
                        state.Interact(selectedRadio);
                        selectedRadio = null;
                    }
                    else
                    {
                        if (selectedState)
                        {
                            selectedState.GetComponent<IInteractable>().Select();
                            var id = objectHit.gameObject.GetInstanceID();
                            var currentID = selectedState.GetInstanceID();
                            var isSame = id == currentID;
                            selectedState = null;
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(false);

                            if(isSame) return;
                            
                            selectedState = objectHit.gameObject;
                            var stateController = selectedState.GetComponent<State>();
                            var upgradeCost = stateController.GiveUpgradeCost();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            
                            state.Select();
                        }
                        else
                        {
                            selectedState = objectHit.gameObject;
                            
                            var stateController = selectedState.GetComponent<State>();
                            var upgradeCost = stateController.GiveUpgradeCost();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            if(stateController.IsNoneSelectable()) 
                                return;
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            state.Select();
                        }
                    }
                }
            }
            else
            {
                if (selectedState)
                {
                    CoreGameSignals.Instance.OnUpgrade?.Invoke(false);
                    selectedState.GetComponent<IInteractable>().Select();
                    selectedState = null;
                }

                if (selectedRadio)
                {
                    selectedRadio = null;
                    CoreGameSignals.Instance.OnGrid?.Invoke(false);
                }
            }
        }
    }
}
