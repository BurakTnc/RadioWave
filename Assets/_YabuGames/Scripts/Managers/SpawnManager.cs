using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Spawners;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private bool isScanNeeded;
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();
        [SerializeField] private float spawnCooldown = 1f;
        [FormerlySerializedAs("spawnItem")] [SerializeField] private string spawnItemName;

        private float _time;
        private int _queuedItems;

        private void SpawnRandom()
        {
            var r = Random.Range(0, spawners.Count);
            if (isScanNeeded)
            {
                if(!spawners[r].CanSpawn()) return;
            }
            _queuedItems--;
            _queuedItems = Mathf.Clamp(_queuedItems, 0, 10); 
            _time += spawnCooldown;
            spawners[r].ReadyToSpawn(spawnItemName,isScanNeeded);
        }

        private void Update()
        {
            _time -= Time.deltaTime;
            _time = Math.Clamp(_time, 0, spawnCooldown);
            
            if(_time <= 0)
            {
                _queuedItems++;
            }
            if (_queuedItems > 0 ) 
            {
                SpawnRandom();
            }
        }
    }
}
