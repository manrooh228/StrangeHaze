using System.Collections;
using UnityEngine;

namespace Assets.Scripts.enemy
{
    public class EventAreaTrigger : MonoBehaviour
    {
        public GameObject monsterType; // Какого монстра спавним
        public Transform spawnLocation; // Где именно (точка на карте)
        public int amount = 10;
        private bool _activated = false;

        [Range(0, 360)] // Удобный ползунок в инспекторе
        public float spawnAngle;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_activated)
            {
                // Передаем spawnAngle в метод спавна
                var spawner = FindAnyObjectByType<EnemySpawner>();
                if (spawner != null)
                {
                    spawner.SpawnWave(monsterType, spawnLocation.position, amount, spawnAngle);
                }

                _activated = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnLocation != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 direction = new Vector3(Mathf.Cos(spawnAngle * Mathf.Deg2Rad), Mathf.Sin(spawnAngle * Mathf.Deg2Rad), 0);
                Gizmos.DrawRay(spawnLocation.position, direction * 2f);
                Gizmos.DrawWireSphere(spawnLocation.position, 0.5f);
            }
        }
    } 
}