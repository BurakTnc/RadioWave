using System;
using _YabuGames.Scripts.Interfaces;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionController : MonoBehaviour
    {
        private IMergeable _mergeable;
        private RadioController _radioController;
        private bool _canMerge;
        private void Awake()
        {
            _mergeable = GetComponent<IMergeable>();
            _radioController = GetComponent<RadioController>();
        }

        public void SetMergeBool(bool mergeable)
        {
            _canMerge = mergeable;
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out IMergeable radio))
            {
                if(!_canMerge) return;
                radio.Merge(_radioController);
                _mergeable.Disappear();
                _canMerge = false;
            }
            if (other.TryGetComponent(out IInteractable state))
            {
                if(!_canMerge) return;
                state.Interact(gameObject);
                _canMerge = false;
            }
        }
    }
}
