using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class CoreGameSignals : MonoBehaviour
    {
        public static CoreGameSignals Instance;

        public UnityAction OnGameStart = delegate { };
        public UnityAction OnLevelLoad= delegate {  };
        public UnityAction OnLevelWin=delegate { };
        public UnityAction OnLevelFail=delegate { };
        public UnityAction OnSave = delegate { };
        public UnityAction<int,bool> OnSpawnCoins=delegate { };

        #region Singleton
        private void Awake()
        {
            if (Instance != null && Instance != this) 
            {
                Destroy((this));
                return;
            }

            Instance = this;
        }
        #endregion
    }
}
