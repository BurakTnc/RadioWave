using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Signals;
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
        private readonly List<Material[]> _childMaterials = new List<Material[]>();
        private bool _isOnline;
        private bool _isSelected;
        private bool _hasRadio;
        private int _radioLevel;
        private int _upgradePrice;
        private int _buyPrice = 500;
        private bool _firstContact;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            foreach (var t in _renderer.materials)
            {
                var mat = new Material(t);
                _defaultMaterials.Add(mat);
            }

            if(transform.childCount<1) return;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                if (i <= 0) continue;
                
                var mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                var materials = new Material[mesh.materials.Length];
                for (var j = 0; j < mesh.materials.Length; j++)
                {
                    materials[j] = new Material(mesh.materials[j]);
                }
                _childMaterials.Add(materials);
                
            }

            if (transform.childCount>0)
            {
                _selectionEffect = transform.GetChild(0).gameObject;
            }
            
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        #region Subscribtions

        private void Subscribe()
        {
            CoreGameSignals.Instance.OnGrid += SetGrid;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnGrid -= SetGrid;
        }

        #endregion

        private void Start()
        {
            foreach (var t in _renderer.materials)
            {
                t.color = offRangeMat.color;
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                if (i <= 0) continue;
                var mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                foreach (var mat in mesh.materials)
                {
                    mat.color = offRangeMat.color;
                }
            }

            StartCoroutine(FirstContact());
        }

        private IEnumerator FirstContact()
        {
            yield return new WaitForSeconds(1);
            _firstContact = true;
        }
        private void SetGrid(bool onGrid)
        {
            if (onGrid)
            {
                //transform.DOMoveY(.1f, .3f).SetEase(Ease.OutBack).SetRelative(true);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(true);
            }
            else
            {
                //transform.DOMoveY(-.1f, .3f).SetEase(Ease.InBack).SetRelative(true);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(false);
            }
        }
        private void EnableScan(GameObject radio)
        {
            radio.GetComponent<RadioScanner>().SetScanningBool(true);
        }
        private void GetStats(int upgradeCost,int level,bool hasRadio)
        {
            _upgradePrice = upgradeCost;
            _radioLevel = level;
            _hasRadio = hasRadio;
        }
        public void Interact(GameObject obj)
        {
            obj.GetComponent<RadioScanner>().SetScanningBool(false);
            var radioController = obj.GetComponent<RadioController>();
            _radioLevel = radioController.radioLevel;
            _upgradePrice = _radioLevel * 500;
            _hasRadio = true;
            GetStats(_upgradePrice,_radioLevel,_hasRadio);
            obj.transform.DOMove(transform.position + Vector3.up *0.2f, .5f).SetEase(Ease.OutSine)
                .OnComplete(() => EnableScan(obj));
            CoreGameSignals.Instance.OnGrid?.Invoke(false);

        }

        public void SetZone(bool onRange,float delay)
        {
            if (onRange)
            {
                _isOnline = true;
                for (var i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(_defaultMaterials[i].color, .7f).SetEase(Ease.InSine).SetDelay(delay);
                }

                if (transform.childCount == 0) return;
                
                for (var i = 0; i < transform.childCount; i++)
                {
                    if (i <= 0) continue;
                    var mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                    for (var j = 0; j < mesh.materials.Length; j++)
                    {
                        mesh.materials[j].DOColor(_childMaterials[i-1][j].color, .7f).SetEase(Ease.InSine)
                            .SetDelay(delay+.4f);
                    }
                }
                
            }
            else
            {
                _isOnline = false;
                for (var i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].DOColor(offRangeMat.color, .7f).SetEase(Ease.OutSine).SetDelay(delay);
                }
                
                if (transform.childCount < 1) return;
                
                for (var i = 0; i < transform.childCount; i++)
                {
                    if(i <= 0) continue;
                    var mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                    for (var j = 0; j < mesh.materials.Length; j++)
                    {
                        mesh.materials[j].DOColor(offRangeMat.color, .7f).SetEase(Ease.InSine)
                            .SetDelay(delay+.2f);
                    }
                }
            }
         
        }

        public void ResetStats()
        {
            
        }
        public void AddTower()
        {
            var tower = Instantiate(Resources.Load<GameObject>("Spawnables/Radio_1"));
            tower.transform.position = transform.position + Vector3.up * 0.2f;
            tower.transform.DOScaleY(.2f, .5f).SetLoops(2, LoopType.Yoyo);
            Interact(tower);
        }
        public void Select()
        {
            _isSelected = !_isSelected;
            if (_isSelected)
            {
                //transform.DOMoveY(.1f, .3f).SetEase(Ease.OutBack).SetRelative(true);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(true);
            }
            else
            {
                //transform.DOMoveY(-.1f, .3f).SetEase(Ease.InBack).SetRelative(true);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(_firstContact) return;
                Interact(other.gameObject);
            }
        }

        public int GiveUpgradeCost() => _upgradePrice;
        public int GiveBuyCost() => _buyPrice;
        public bool GiveRadioBool() => _hasRadio;
        public bool IsOnline() => _isOnline;
    }
    
}