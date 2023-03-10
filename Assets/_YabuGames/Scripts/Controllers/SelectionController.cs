using System;
using _YabuGames.Scripts.Interfaces;
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
                        _selectedState.GetComponent<IInteractable>().Select();
                        _selectedState = null;
                    }
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
                            if(isSame) return;
                            
                            _selectedState = objectHit.gameObject;
                            state.Select();
                        }
                        else
                        {
                            _selectedState = objectHit.gameObject;
                            state.Select();
                        }
                    }
                }
                else
                {
                    //_selectedRadio = null;
                    if (_selectedState)
                    {
                        _selectedState.GetComponent<IInteractable>().Select();
                        _selectedState = null;
                    }
                }
            }
        }
    }
}
