using System;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private GameObject mainPanel, gamePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private TextMeshProUGUI upgradePriceText, buyPriceText;
        [SerializeField] private Button upgradeButton, buyButton;

        private int _buyPrice, _upgradePrice;
        private bool _isUpgradeOpen;


        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {

                Destroy(this);
                return;
            }
            
            Instance = this;

            #endregion

        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Start()
        {
            SetMoneyTexts();
        }

        #region Subscribtions
        private void Subscribe()
        {
            CoreGameSignals.Instance.OnGameStart += OnGameStart;
            CoreGameSignals.Instance.OnUpgrade += SetUpgradePanel;
            CoreGameSignals.Instance.GetUpgradeStats += SetUpgradeStats;
            CoreGameSignals.Instance.OnUpdateStats += SetMoneyTexts;
            CoreGameSignals.Instance.OnUpdateStats += CheckButtonStats;
        }
        
        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnGameStart -= OnGameStart;
            CoreGameSignals.Instance.OnUpgrade -= SetUpgradePanel;
            CoreGameSignals.Instance.GetUpgradeStats -= SetUpgradeStats;
            CoreGameSignals.Instance.OnUpdateStats -= SetMoneyTexts;
            CoreGameSignals.Instance.OnUpdateStats -= CheckButtonStats;
        }

        #endregion
        
        private void SetUpgradePanel(bool isOpen)
        {
            _isUpgradeOpen = isOpen;
            upgradePanel.transform.DOKill();
            if (_isUpgradeOpen)
            {
                upgradePanel.GetComponent<RectTransform>().DOAnchorPosY(-137, .5f).SetEase(Ease.OutBack);
            }
            else
            {

                upgradePanel.GetComponent<RectTransform>().DOAnchorPosY(-350, .5f).SetEase(Ease.InBack);
            }
        }
        private void OnGameStart()
        {
            mainPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
        private void SetMoneyTexts()
        {
            if (moneyText.Length <= 0) return;

            foreach (var t in moneyText)
            {
                if (t)
                {
                    t.text = "$" + GameManager.Instance.GetMoney();
                }
            }
        }

        private void CheckButtonStats()
        {
            buyButton.interactable = GameManager.Instance.money >= _buyPrice;
            upgradeButton.interactable = GameManager.Instance.money >= _upgradePrice;
        }
        private void SetUpgradeStats(int upgradePrice,bool hasRadio)
        {
            buyButton.gameObject.SetActive(!hasRadio);
            upgradeButton.gameObject.SetActive(hasRadio);
            upgradePriceText.text = "";
            buyPriceText.text = "500";
            _buyPrice = 500;
            if(!hasRadio) return;
            buyPriceText.text = "";
            upgradePriceText.text = upgradePrice.ToString();
            _upgradePrice = upgradePrice;
        }
        public void PlayButton()
        {
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void MenuButton()
        {
            mainPanel.SetActive(true);
            HapticManager.Instance.PlayLightHaptic();
        }

        public void CloseButton()
        {
            SetUpgradePanel(false);
        }
        public void AddTowerButton()
        {
            if (!SelectionController.Instance.selectedState) 
                return;
            
            if (SelectionController.Instance.selectedState.TryGetComponent(out State state))
            {
                GameManager.Instance.money -= state.GiveBuyCost();
                state.AddTower();
            }
        }

        public void UpgradeButton()
        {
            if (!SelectionController.Instance.selectedState) 
                return;
            
            if (SelectionController.Instance.selectedState.TryGetComponent(out State state))
            {
                GameManager.Instance.money -= state.GiveUpgradeCost();
                state.Upgrade();
            }
        }

    }
}
