using System;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class GrabController : MonoBehaviour
    {
        private Vector3 _difference;
        private Vector3 _destination;
        private Vector3 _startPosition;
        private Vector3 _distanceOffset;
        private CollisionController _collisionController;
        private RadioScanner _radioScanner;
        private bool _isGrabbing = false;
        private float _distance;
        private Camera _cam;

        private void Awake()
        {
            _collisionController = GetComponent<CollisionController>();
            _radioScanner = GetComponent<RadioScanner>();
            _cam=Camera.main;
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnDragging += DragSituation;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnDragging -= DragSituation;
        }

        private void DragSituation(bool grabbing)
        {
            _isGrabbing = grabbing;
        }

        private void OnMouseUp()
        {
            //if(!_isGrabbing) return;
            CoreGameSignals.Instance.OnDragging?.Invoke(false);
            _collisionController.SetMergeBool(true);
          _radioScanner.SetScanningBool(true);
        }

        private void OnMouseDown()
        {
            if (_isGrabbing) return;
            
            _radioScanner.SetScanningBool(false);
            _collisionController.SetMergeBool(false);
            _startPosition = _cam.WorldToScreenPoint(transform.position);
            _distanceOffset = transform.position -
                              _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, _startPosition.y,
                                  Input.mousePosition.y));

        }

        private void OnMouseDrag()
        {

            if (_cam != null)
            {
                Vector3 lastPos = new Vector3(Input.mousePosition.x, _startPosition.y, Input.mousePosition.y);
                var targetPos=_cam.ScreenToWorldPoint(lastPos) + _distanceOffset;
                transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
//                 Vector3 targetPos = _cam.ScreenToWorldPoint(lastPos) + _distanceOffset;
//                 Vector3 dir = targetPos - transform.position;
//                 float dist = dir.magnitude;
//                 Vector3.Normalize(dir);
// // change 1.0f to something else if you want:
//                 transform.position += new Vector3(dir.x * dist * .000001f, 0, dir.z * dist * .000001f);

            }

            
        }
    }
}