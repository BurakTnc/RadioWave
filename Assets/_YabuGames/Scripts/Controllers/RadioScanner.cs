using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(RadioController))]
    public class RadioScanner : MonoBehaviour
    {
        [SerializeField] private LayerMask layer;
        [SerializeField] private float scanDelay = .3f;
        [SerializeField] private float scanPeriod = 300;
        [SerializeField] private float areaGrowingMultiplier = 0.01f;
        [SerializeField] private GameObject scanParticle;
        [SerializeField] private float scanSizeMultiplier = .05f;

        private List<IInteractable> _onlineStates = new List<IInteractable>();
        private RadioController _radioController;
        private int _radioLevel;
        private float _rangeMultiplier;
        private float _timer;
        private bool _isScanning;
        private float _tempRadius, _mainRadius;
        private float _particleTimer = 0;

        private void Awake()
        {
            _radioController = GetComponent<RadioController>();
        }

        private void Start()
        {
            _radioLevel = _radioController.radioLevel;
            _rangeMultiplier = _radioController.rangeMultiplier;
            _mainRadius = _radioLevel * _rangeMultiplier;
            _isScanning = true;
        }

        private void Update()
        {
            StartScan();
            SpawnScanParticle();
            _particleTimer -= Time.deltaTime;
        }

        private void StartScan()
        {
            if (!_isScanning) return;
            
            if (_timer <= scanPeriod)
            {
                ScanTheArea();
                _timer += scanDelay;
            }

            else
            {
                _tempRadius = 0;
                _timer = 0;
            }
            
            _timer = Math.Clamp(_timer, 0, scanPeriod+scanDelay);
        }

        private void ScanTheArea()
        {
            var delay = 0f;
            var colliders = Physics.OverlapSphere(transform.position, _tempRadius,layer);
            foreach (var state in colliders)
                if (state.gameObject.TryGetComponent(out IInteractable stateScript))
                {
                    if (!_onlineStates.Contains(state.GetComponent<IInteractable>()))
                    {
                        stateScript.SetZone(true,delay);
                        _onlineStates.Add(state.GetComponent<IInteractable>());
                        
                    }
                    delay += 0.1f;
                }
                    
            _tempRadius += areaGrowingMultiplier;
            _tempRadius = Mathf.Clamp(_tempRadius, 0, 300);
        }

        private void SpawnScanParticle()
        {
            if (_particleTimer > 0) return;
            _particleTimer += 2;
            var particle = Instantiate(scanParticle);
            particle.transform.localScale = Vector3.zero;
            particle.transform.position = transform.position;
            particle.transform.DOScale(Vector3.one * scanPeriod / 100, 5);
        }

        private void StopScanning()
        {
            var delay = 0f;
            for (var i = _onlineStates.Count-1; i > -1; i--)
            {
                _onlineStates[i].SetZone(false,delay);
                delay += 0.1f;
            }
            _onlineStates.Clear();
        }

        private void OnDrawGizmos()
        {
            var range = _rangeMultiplier * _radioLevel;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _tempRadius);
        }

        public void SetScanningBool(bool onIdle)
        {
            _tempRadius = 0;
            _timer = 0;
            _isScanning = onIdle;
            StopScanning();
        }
    }
}