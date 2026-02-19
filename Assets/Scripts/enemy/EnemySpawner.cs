using System.Collections;
using UnityEngine;

namespace Assets.Scripts.enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Настройки времени и дистанции")]
        public bool onoff = false;
        public GameObject[] enemyPrefabs; // Массив разных типов монстров
        public float spawnInterval = 2f;  // Цикличность (секунды)
        public float spawnDistance = 12f; // Расстояние от игрока

        private Transform _player;

        void Start()
        {
            if (onoff)
            {
                _player = GameObject.FindGameObjectWithTag("Player").transform;
                StartCoroutine(SpawnRoutine());
            }
        }

        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnRandomEnemy();
            }
        }

        public void SpawnRandomEnemy()
        {
            // Генерируем случайную точку на круге вокруг игрока
            Vector2 spawnPos = (Vector2)_player.position + Random.insideUnitCircle.normalized * spawnDistance;
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], spawnPos, Quaternion.identity);
        }


        //group spawner
        public void SpawnWave(GameObject prefab, Vector3 position, int count, float spawnAngle)
        {
            for (int i = 0; i < count; i++)
            {
                //разброс
                Vector2 offset = Random.insideUnitCircle * 1.5f;

                Quaternion rotation = Quaternion.Euler(0f, 0f, spawnAngle);

                Instantiate(prefab, position + (Vector3)offset, rotation);
            }
        }
    }
}