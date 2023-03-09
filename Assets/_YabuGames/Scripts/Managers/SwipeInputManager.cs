using UnityEngine;


namespace _YabuGames.Scripts.Managers
{
    public class SwipeInputManager : MonoBehaviour
    {
         public static SwipeInputManager Instance { set; get;}

        [HideInInspector]
        public bool  swipeLeft, swipeRight, swipeUp, swipeDown;

        private Vector2 _swipeDelta, _startTouch;
        private const float DeadZone = 50;
    
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            swipeLeft = swipeRight = swipeDown = swipeUp = false;

            //Input
        
            if (Input.GetMouseButtonDown(0))
            {
                _startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _startTouch = _swipeDelta = Vector2.zero;
            }
        
        
            // Calculating Distance

            _swipeDelta = Vector2.zero;
            if (_startTouch!=Vector2.zero)
            {
                if(Input.GetMouseButton(0))
                {
                    _swipeDelta = (Vector2)Input.mousePosition - _startTouch;
                }
            }

            //  DeadZone?
            if (_swipeDelta.magnitude>DeadZone)
            {
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                if (Mathf.Abs(x)>Mathf.Abs(y))
                {
                    //Left Or Right?
                    if (x<0)
                    {
                        swipeLeft = true;
                    }
                    else
                    {
                        swipeRight = true;
                    }
                }
                else
                {
                    //Up Or Down
                    if (y < 0)
                    {
                        swipeDown = true;
                    }
                    else
                    {
                        swipeUp = true;
                    }
                }

                _startTouch = _swipeDelta = Vector2.zero;
            }
        }
    }
}
