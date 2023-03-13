using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Controllers
{
    public class SelectionController : MonoBehaviour
    {
        private Camera _cam;
        private GameObject _selectedRadio;
        private GameObject _selectedState;
       public GraphicRaycaster m_Raycaster;
        PointerEventData m_PointerEventData;
       public EventSystem m_EventSystem;

        private void Awake()
        {
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
                    if (_selectedRadio)
                    {
                        _selectedRadio = null;
                        CoreGameSignals.Instance.OnGrid?.Invoke(false);
                        return;
                    }
                    
                    _selectedRadio = radio.gameObject;
                    if (_selectedState)
                    {
                        CoreGameSignals.Instance.OnUpgrade?.Invoke(false);
                        _selectedState.GetComponent<IInteractable>().Select();
                        _selectedState = null;
                    }
                    CoreGameSignals.Instance.OnGrid?.Invoke(true);
                }

                if (objectHit.TryGetComponent(out IInteractable state))
                {
                    if (_selectedRadio)
                    {
                        state.Interact(_selectedRadio);
                        _selectedRadio = null;
                    }
                    else
                    {
                        if (_selectedState)
                        {
                            _selectedState.GetComponent<IInteractable>().Select();
                            var id = objectHit.gameObject.GetInstanceID();
                            var currentID = _selectedState.GetInstanceID();
                            var isSame = id == currentID;
                            _selectedState = null;
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(false);

                            if(isSame) return;
                            
                            _selectedState = objectHit.gameObject;
                            var stateController = _selectedState.GetComponent<State>();
                            var upgradeCost = stateController.GiveUpgradeCost();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            
                            state.Select();
                        }
                        else
                        {
                            _selectedState = objectHit.gameObject;
                            
                            var stateController = _selectedState.GetComponent<State>();
                            var upgradeCost = stateController.GiveUpgradeCost();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            state.Select();
                        }
                    }
                }
            }
            else
            {
                if (_selectedState)
                {
                    CoreGameSignals.Instance.OnUpgrade?.Invoke(false);
                    _selectedState.GetComponent<IInteractable>().Select();
                    _selectedState = null;
                }
            }
        }
    }
}
