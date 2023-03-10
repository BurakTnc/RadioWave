using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Objects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(RadioController))]
    public class RadioScanner : MonoBehaviour
    {
        [SerializeField] private LayerMask layer;
        [FormerlySerializedAs("scanPeriod")] [SerializeField] private float scanParticleSize = 300;
        [SerializeField] private float areaGrowingMultiplier = 0.01f;
        [SerializeField] private GameObject scanParticle;
        [SerializeField] private float scanSize = 50;

        private readonly List<IInteractable> _onlineStates = new List<IInteractable>();
        private bool _isScanning;
        private float _tempRadius;
        private float _particleTimer;

        private void Awake()
        {
        }

        private void Start()
        {
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
            
            if (_tempRadius < scanSize)
            {
                ScanTheArea();
            }
            else
            {
                _tempRadius = 0;
            }
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
                        if (!state.gameObject.GetComponent<State>().IsOnline())
                        {
                            stateScript.SetZone(true,delay);
                            _onlineStates.Add(state.GetComponent<IInteractable>());  
                        }
                    }
                    delay += 0.1f;
                }

            _tempRadius += areaGrowingMultiplier * Time.deltaTime;
            _tempRadius = Mathf.Clamp(_tempRadius, 0, scanSize+10);
        }

        private void SpawnScanParticle()
        {
            if(!_isScanning) return;
            
            if (_particleTimer > 0) return;
            _particleTimer += 3f;
            var particle = Instantiate(scanParticle);
            particle.transform.localScale = Vector3.zero;
            particle.transform.position = transform.position;
            particle.transform.DOScale(Vector3.one * scanParticleSize / 100, 5).SetEase(Ease.InSine);
        }

        private void StopScanning()
        {
            var delay = 0f;
            for (var i = _onlineStates.Count-1; i > -1; i--)
            {
                _onlineStates[i].SetZone(false,delay);
                delay += 0.07f;
            }
            _onlineStates.Clear();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _tempRadius);
        }

        public void SetScanningBool(bool onIdle)
        {
            _tempRadius = 0;
            _particleTimer = 0;
            _isScanning = onIdle;
            StopScanning();
        }
    }
}