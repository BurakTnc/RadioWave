using UnityEngine;

namespace _YabuGames.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyData-", menuName = "YabuGames/New Enemy Data", order = 0)]
    public class EnemyData : ScriptableObject
    {
        public float health = 100;
        public float speed = 10;
    }
}