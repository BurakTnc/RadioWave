using UnityEngine;

namespace _YabuGames.Scripts.Controllers.IdleArcade
{
    public class PlayerJoystickMovement : MonoBehaviour
    {

        [SerializeField] private float angularSpeed = 3f;
        [SerializeField] private float forwardSpeed = 5f;
        
        private float _xAxis, _yAxis;
        private Vector3 _direction;

        private void Start()
        {
        
        }
        
        private void Update()
        {
            _xAxis = SimpleInput.GetAxis("Horizontal");
            _yAxis = SimpleInput.GetAxis("Vertical");
            _direction = Vector3.forward * _yAxis + Vector3.right * _xAxis;
            Move();
        }

        private void Move()
        {
            if (_direction == Vector3.zero || !Input.GetMouseButton(0)){}

            transform.position += transform.forward * (forwardSpeed * Time.deltaTime);
            var rotation = transform.rotation;
            var desiredRotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(_direction),
                angularSpeed * Time.deltaTime);
            rotation = desiredRotation;
            transform.rotation = rotation;
        }
    }
}
