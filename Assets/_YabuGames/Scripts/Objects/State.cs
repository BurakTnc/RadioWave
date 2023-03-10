using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;
namespace _YabuGames.Scripts.Objects
{
    public class State : MonoBehaviour,IInteractable
    {
        [SerializeField] private Material offRangeMat;

        private GameObject _selectionEffect;
        private MeshRenderer _renderer;
        private readonly List<Material> _defaultMaterials = new List<Material>();
        private bool _isOnline;
        private bool _isSelected;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            foreach (var t in _renderer.materials)
            {
                var mat = new Material(t);
                _defaultMaterials.Add(mat);
            }
        }

        private void Start()
        {
            foreach (var t in _renderer.materials)
            {
                t.color = offRangeMat.color;
            }
        }

        public void Interact(GameObject obj)
        {
            obj.transform.position = transform.position + Vector3.up * .5f;
        }

        public void SetZone(bool onRange,float delay)
        {
            if (onRange)
            {
                for (var i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(_defaultMaterials[i].color, .7f).SetEase(Ease.InSine).SetDelay(delay);
                }
            }
            else
            {
                for (var i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(offRangeMat.color, .7f).SetEase(Ease.InSine).SetDelay(delay);
                }
            }
         
        }

        public void Select()
        {
            _isSelected = !_isSelected;
            if (_isSelected)
            {
                transform.DOMoveY(.1f, .3f).SetEase(Ease.OutBack).SetRelative(true);
            }
            else
            {
                transform.DOMoveY(-.1f, .3f).SetEase(Ease.InBack).SetRelative(true);
            }
        }
    }
}