using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;
namespace _YabuGames.Scripts.Objects
{
    public class State : MonoBehaviour,IInteractable
    {
        [SerializeField] private Transform radioPosition;
        [SerializeField] private Material onRangeMat, offRangeMat;

        private MeshRenderer _renderer;
        private List<Material> _defaultMaterials = new List<Material>();
        private Material _currentMaterial;
        private bool _isOnline;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _currentMaterial = new Material(_renderer.material);
            for (int i = 0; i < _renderer.materials.Length; i++)
            {
                var mat = new Material(_renderer.materials[i]);
                _defaultMaterials.Add(mat);
            }
           
        }

        private void Start()
        {
            for (int i = 0; i < _renderer.materials.Length; i++)
            {
                _renderer.materials[i].color = offRangeMat.color;
            }
        }

        public void Interact(GameObject obj)
        {
            obj.transform.position = transform.position + Vector3.up * .5f;
            // obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        public void SetZone(bool onRange,float delay)
        {
            if (onRange)
            {
                //_renderer.material.DOColor(_currentMaterial.color, .7f).SetEase(Ease.InSine).SetDelay(delay);

                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(_defaultMaterials[i].color, .7f).SetEase(Ease.InSine).SetDelay(delay);
                }
            }
            else
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(offRangeMat.color, .7f).SetEase(Ease.InSine).SetDelay(delay);
                }
            }
         
        }
    }
}