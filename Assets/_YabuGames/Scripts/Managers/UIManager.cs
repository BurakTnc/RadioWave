using System;
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
        [SerializeField] private TextMeshProUGUI upgradePriceText, buyPriceText, radioLevelText;
        [SerializeField] private Button upgradeButton, buyButton;

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
        }
        
        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnGameStart -= OnGameStart;
            CoreGameSignals.Instance.OnUpgrade -= SetUpgradePanel;
            CoreGameSignals.Instance.GetUpgradeStats -= SetUpgradeStats;
        }

        #endregion

        private void SetBool()
        {
            //_isUpgradeOpen = !_isUpgradeOpen;
        }
        private void SetUpgradePanel(bool isOpen)
        {
            _isUpgradeOpen = isOpen;
            upgradePanel.transform.DOKill();
            if (_isUpgradeOpen)
            {
                upgradePanel.GetComponent<RectTransform>().DOAnchorPosY(0, .5f).SetEase(Ease.OutBack)
                    .OnComplete(SetBool);
            }
            else
            {
                
                upgradePanel.GetComponent<RectTransform>().DOAnchorPosY(-500, .5f).SetEase(Ease.InBack)
                    .OnComplete(SetBool);
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

        private void SetUpgradeStats(int upgradePrice,int radioLevel,bool hasRadio)
        {
            buyButton.interactable = !hasRadio;
            upgradeButton.interactable = hasRadio;
            upgradePriceText.text = "";
            buyPriceText.text = "500";
            radioLevelText.text = "";
            if(!hasRadio) return;
            radioLevelText.text = "Radio Lvl " + radioLevel;
            buyPriceText.text = "";
            upgradePriceText.text = upgradePrice.ToString();
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

    }
}
