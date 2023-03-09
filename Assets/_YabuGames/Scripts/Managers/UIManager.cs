using System;
using _YabuGames.Scripts.Signals;
using TMPro;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject mainPanel, gamePanel, winPanel, losePanel, storePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;


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
                    CoreGameSignals.Instance.OnLevelWin += LevelWin;
                    CoreGameSignals.Instance.OnLevelFail += LevelLose;
                    CoreGameSignals.Instance.OnGameStart += OnGameStart;
                }
        
                private void UnSubscribe()
                {
                    CoreGameSignals.Instance.OnLevelWin -= LevelWin;
                    CoreGameSignals.Instance.OnLevelFail -= LevelLose;
                    CoreGameSignals.Instance.OnGameStart -= OnGameStart;
                }

        #endregion
        
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
        private void LevelWin()
        {
            gamePanel.SetActive(false);
            winPanel.SetActive(true);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            gamePanel.SetActive(false);
            gamePanel.SetActive(true);
            HapticManager.Instance.PlayFailureHaptic();
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

        public void NextButton()
        {
            CoreGameSignals.Instance.OnLevelLoad?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void RetryButton()
        {
            CoreGameSignals.Instance.OnLevelLoad?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }
    }
}
