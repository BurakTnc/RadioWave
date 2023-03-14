using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Objects
{
    public class State : MonoBehaviour,IInteractable
    {
        [SerializeField] private Material offRangeMat;
        [SerializeField] private bool noneSelectable;
        
        private GameObject _selectionEffect;
        private MeshRenderer _renderer;
        private readonly List<Material> _defaultMaterials = new List<Material>();
        private readonly List<Material[]> _childMaterials = new List<Material[]>();
        private bool _isOnline;
        private bool _isSelected;
        private bool _hasRadio;
        private int _radioLevel;
        private int _upgradePrice;
        private readonly int _buyPrice = 500;
        private bool _firstContact;
        private Vector3 _startPoS;
        private GameObject _currentTower;
        private float _timer;
        private int _incomeLevel;
        private int _incomeCost;
        private const float _maxTimerValue = 3;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            GetMaterials();
        }

        private void GetMaterials()
        {
            foreach (var t in _renderer.materials)
            {
                var mat = new Material(t);
                _defaultMaterials.Add(mat);
            }

            if (transform.childCount < 1) return;

            _selectionEffect = transform.GetChild(0).gameObject;
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
            SetStartVariables();
            SetDefaultMaterials();
            StartCoroutine(FirstContact());
        }

        private void SetDefaultMaterials()
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
        }

        private void SetStartVariables()
        {
            _timer = 3;
            _startPoS = transform.position;
            _upgradePrice = (_radioLevel) * 1000;
            _incomeCost = (_incomeLevel + 1) * 300;
        }

        private void Update()
        {
            if (!_isOnline) return;
            if (_timer <= 0)
            {
                _timer += 3;
                GameManager.Instance.money += _radioLevel + _incomeLevel;
                PoolManager.Instance.GetMoneyParticle(transform.position+Vector3.up*.5f,1); 
                CoreGameSignals.Instance.OnUpdateStats?.Invoke();
            }
            _timer -= Time.deltaTime;
            _timer = Math.Clamp(_timer, 0, _maxTimerValue);
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
                if(!_selectionEffect || noneSelectable) return;
                _selectionEffect.SetActive(true);
            }
            else
            {
                if(!_selectionEffect || noneSelectable) return;
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
            _currentTower = obj;
            obj.GetComponent<RadioScanner>().SetScanningBool(false);
            var radioController = obj.GetComponent<RadioController>();
            _radioLevel = radioController.radioLevel;
            _upgradePrice = _radioLevel * 1000;
            _hasRadio = true;
            _incomeCost = (_incomeLevel+1) * 300;
            GetStats(_upgradePrice,_radioLevel,_hasRadio);
            obj.transform.DOMove(transform.position + Vector3.up *0.2f, .5f).SetEase(Ease.OutSine)
                .OnComplete(() => EnableScan(obj));
            CoreGameSignals.Instance.OnGrid?.Invoke(false);
            CoreGameSignals.Instance.GetUpgradeStats?.Invoke(_upgradePrice,_hasRadio);
            
            if (!radioController.state) 
            {
                radioController.state = this.gameObject;
                return;
            }

            if (radioController.state)
            {
                radioController.state.GetComponent<State>().ResetStats();
                radioController.state = this.gameObject;
            }

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

        private void ResetStats()
        {
            _radioLevel = 0;
            _upgradePrice = 0;
            _hasRadio = false;
        }
        public void AddTower()
        {
            var tower = Instantiate(Resources.Load<GameObject>("Spawnables/Radio_1"));
            tower.transform.position = transform.position+Vector3.down*.2f;
            tower.transform.DOScaleY(.2f, .5f).SetLoops(2, LoopType.Yoyo);
            Interact(tower);
        }

        public void Upgrade()
        {
            if (_currentTower.TryGetComponent(out RadioScanner scanner))
            {
                scanner.StopScanning();
            }
            Destroy(_currentTower);
            var tower = Instantiate(Resources.Load<GameObject>($"Spawnables/Radio_{_radioLevel+1}"));
            tower.transform.position = transform.position + Vector3.down * 0.3f;
            tower.transform.DOScaleZ(.3f, .2f).SetLoops(2, LoopType.Yoyo);
            Interact(tower);
        }
        public void Select()
        {
            if(!_selectionEffect || noneSelectable) return;
            _isSelected = !_isSelected;
            if (_isSelected)
            {
                transform.DOMove(_startPoS + Vector3.up * .1f, .3f).SetEase(Ease.OutBack);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(true);
            }
            else
            {
                transform.DOMove(_startPoS, .3f).SetEase(Ease.InBack);
                if(!_selectionEffect) return;
                _selectionEffect.SetActive(false);
            }
        }
        public void ChanceIncome()
        {
            _incomeLevel ++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(_firstContact) return;
                Interact(other.gameObject);
            }
        }

        public void SetStateStats(bool radio,int upgradeCost,int incomeLevel)
        {
            _hasRadio = radio;
            _upgradePrice = (_radioLevel+1) * 1000;
            _incomeLevel = incomeLevel;
            _incomeCost = (_incomeLevel + 1) * 300;
        }
        public int GiveUpgradeCost() => _upgradePrice;
        public int GiveBuyCost() => _buyPrice;
        public bool GiveRadioBool() => _hasRadio;
        public bool IsOnline() => _isOnline;
        public bool IsNoneSelectable() => noneSelectable;
        public int GiveIncomeCost() => _incomeCost;

    }
    
}