using System.Collections;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Spawners
{
    public class CoinSpawner : MonoBehaviour
    {
        public static CoinSpawner Instance;
        
        [SerializeField]
        private GameObject loseTarget, winTarget;

        private bool _isWon;
        private Camera _cam;
        
        private void Awake()
        {
            #region Singleton
            if (Instance!=null && Instance!=this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            #endregion
            _cam=Camera.main;
        }

        private void Start()
        {
            CoreGameSignals.Instance.OnSpawnCoins += SpawnCoins;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnSpawnCoins -= SpawnCoins;
        }

        private IEnumerator Begin(int coin,float time)
        {
            yield return new WaitForSeconds(time);
            if (coin <= 0) yield break;
            var target = loseTarget;
            if (_isWon)
            {
                target = winTarget;
            }
            for (var i = 0; i < coin; i++)
            {
                Instantiate(Resources.Load<GameObject>(path: "Spawnables/Coin"), _cam.WorldToScreenPoint(Vector3.zero),
                    loseTarget.transform.rotation, target.transform);
            }
        }

        private void SpawnCoins(int coin,bool isWin)
        {
            _isWon = isWin;
            float delay;
            if (isWin)
            {
                delay = .8f;
            }
            else
            {
                delay = 0;
            }
            StartCoroutine(Begin(coin, delay));
        }
    }
}