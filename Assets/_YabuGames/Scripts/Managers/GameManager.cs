using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public int money;

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
            GetValues();
        }

        #region Subscribtions
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            CoreGameSignals.Instance.OnSave += Save;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnSave -= Save;
        }

        #endregion

        private void GetValues()
        {
            money = PlayerPrefs.GetInt("money", 0);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("money",money);
        }

        public void ArrangeMoney(int value)
        {
            money += value;
        }

        public int GetMoney()
        {
            return money < 0 ? 0 : money;
        }
    }
}