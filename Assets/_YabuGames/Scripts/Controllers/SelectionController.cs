using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class SelectionController : MonoBehaviour
    {
        private Camera _cam;
        private GameObject _selectedRadio;
        private GameObject _selectedState;

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
            
            if (Physics.Raycast(ray, out var hit))
            {
                var objectHit = hit.transform;

                if (objectHit.TryGetComponent(out RadioController radio))
                {
                    if (_selectedRadio)
                    {
                        _selectedRadio = null;
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
                            var radioLevel = stateController.GiveRadioLevel();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,radioLevel,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            
                            state.Select();
                        }
                        else
                        {
                            _selectedState = objectHit.gameObject;
                            
                            var stateController = _selectedState.GetComponent<State>();
                            var upgradeCost = stateController.GiveUpgradeCost();
                            var radioLevel = stateController.GiveRadioLevel();
                            var hasRadio = stateController.GiveRadioBool();
                            
                            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(upgradeCost,radioLevel,hasRadio);
                            CoreGameSignals.Instance.OnUpgrade?.Invoke(true);
                            state.Select();
                        }
                    }
                }
                else
                {
                    //_selectedRadio = null;
                    if (_selectedState)
                    {
                        Debug.Log(objectHit.name);
                        CoreGameSignals.Instance.OnUpgrade?.Invoke(false);
                        _selectedState.GetComponent<IInteractable>().Select();
                        _selectedState = null;
                    }
                }
            }
        }
    }
}
