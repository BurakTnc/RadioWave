using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers.Runner
{
    public class PlayerMovementController : MonoBehaviour
    {
        public static bool OnTrap;

        [SerializeField] private float xPosClamp, speedSideways, speed;
        
        private Rigidbody _rb;
        private Vector3 _pos1, _pos2;
        private bool _holding;
        private bool _isGameRunning;
        
       private void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody>();
            _isGameRunning = _holding = OnTrap = false;
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
           CoreGameSignals.Instance.OnGameStart += OnGameStart;
           CoreGameSignals.Instance.OnLevelWin += OnGameEnd;
           CoreGameSignals.Instance.OnLevelFail += OnGameEnd;
       }

       private void UnSubscribe()
       {
           CoreGameSignals.Instance.OnGameStart -= OnGameStart;
           CoreGameSignals.Instance.OnLevelWin -= OnGameEnd;
           CoreGameSignals.Instance.OnLevelFail -= OnGameEnd;
       }

       #endregion
       
       private void Update()
        {
            Movement();
        }

       private void OnGameStart()
       {
           _isGameRunning = true;
       }

       private void OnGameEnd()
       {
           _isGameRunning = false;
       }
       
       private void Movement()
        {
            if(!_isGameRunning) return;
            
            if (!OnTrap)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _pos1 = GetMousePosition();
                    _holding = true;
                }

                if (Input.GetMouseButton(0) && _holding)
                {
                    _pos2 = GetMousePosition();
                    Vector3 delta = _pos1 - _pos2;
                    _pos1 = _pos2;
                    delta = new Vector3(Mathf.Clamp(delta.x, -0.05f, 0.05f), delta.y);
                    _rb.velocity = new Vector3(Mathf.Lerp(_rb.velocity.x, -delta.x * speedSideways, 0.2f), _rb.velocity.y, speed);
                }
                else
                {
                    _rb.velocity = transform.forward * speed;
                }

                var position = transform.position;
                
                position = new Vector3(Mathf.Clamp(position.x, -xPosClamp, xPosClamp),
                    position.y, position.z);
                transform.position = position;
            }
        }

        private Vector2 GetMousePosition()
        {
            var pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

            return pos;
        }

    }
}
