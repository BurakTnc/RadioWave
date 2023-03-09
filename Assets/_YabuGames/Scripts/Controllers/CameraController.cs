using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 3f;
        [SerializeField] private Vector3 offset;
        
        private Transform _player;
        private bool _isGameRunning = false;

        private void Awake()
        {
            _player = GameObject.Find("Player").transform;
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        void Update()
        {
            Follow();
        }

        #region Subscribtions

        private void Subscribe()
                {
                    CoreGameSignals.Instance.OnGameStart += OnGameStart;
                    CoreGameSignals.Instance.OnLevelFail += OnGameEnd;
                    CoreGameSignals.Instance.OnLevelWin += OnGameEnd;
                }
        
                private void UnSubscribe()
                {
                    CoreGameSignals.Instance.OnGameStart -= OnGameStart;
                    CoreGameSignals.Instance.OnLevelFail -= OnGameEnd;
                    CoreGameSignals.Instance.OnLevelWin -= OnGameEnd;
                }

        #endregion
        
        private void OnGameStart()
        {
            _isGameRunning = true;
        }

        private void OnGameEnd()
        {
            _isGameRunning = false;
        }

        private void Follow()
        {
            if (_isGameRunning)
            {
                transform.position = Vector3.Lerp(transform.position, _player.position + offset,
                    followSpeed * Time.deltaTime);
            }

                                     // For Limited Follow
            
            // if (_isGameRunning)
            // {
            //     Vector3 desiredPos = new Vector3(0, transform.position.y, _player.position.z) + offset;
            //     transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
            // }
        }
    }
}
