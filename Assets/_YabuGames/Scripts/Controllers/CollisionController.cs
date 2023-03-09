using System;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.transform.tag)
            {
                default:
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                default:
                    break;
            }
        }
    }
}
