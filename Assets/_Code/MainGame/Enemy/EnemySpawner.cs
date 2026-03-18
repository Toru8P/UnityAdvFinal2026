using System.Collections;
using System.Collections.Generic;
using _Code.MainGame.Enemy.Difficulty;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Code.MainGame.Enemy
{
    [RequireComponent(typeof(DifficultySystem))]
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Player Target")]
        [SerializeField] private Transform target;
        
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject enemyLowPrefab;
        [SerializeField] private GameObject enemyMidPrefab;
        [SerializeField] private GameObject enemyHiPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxEnemies = 10; 
        [SerializeField] private Tilemap spawnZone;

        [Header("Enemy Light Settings")]
        [SerializeField] private bool enemiesHaveLight = false;
        
        [Header("Difficulty Settings")]
        [SerializeField] private float currentSpeed;
        
        private int _spawnCounter;
        private bool _continueSpawning = true;
    
        private float timer;
    
        private List<Vector3Int> availableCells;
    
        private IEnumerator SpawnEnemies()
        {
            while (_spawnCounter < maxEnemies && _continueSpawning)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        public void StopSpawning()
        {
            _continueSpawning = false;
        }
    
        private void SpawnEnemy()
        {
            if (_continueSpawning && enemyLowPrefab && enemyMidPrefab && enemyHiPrefab && target)
            {
                Vector3Int randomCell = availableCells[Random.Range(0, availableCells.Count-1)];
                
                GameObject enemyPrefab = null;
                switch (currentSpeed)
                {
                    case 1: case 2: case 3:
                        enemyPrefab = enemyLowPrefab;
                        break;
                    case 4: 
                        enemyPrefab = enemyMidPrefab;
                        break;
                    default: 
                        enemyPrefab = enemyHiPrefab;
                        break;
                }
                GameObject enemy = Instantiate(enemyPrefab, randomCell, Quaternion.identity);

                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller)
                {
                    controller.SetTarget(target);
                    controller.SetSpeed(currentSpeed);
                    controller.TurnLight(enemiesHaveLight);
                }

                _spawnCounter++;
            }
        }
    
        private void Awake()
        {
            DifficultySystem difficultySystem = GetComponent<DifficultySystem>();
            difficultySystem.SubscribeToDifficultyUpdate(OnDifficultyChanged);
            
            availableCells = new List<Vector3Int>();
            BoundsInt bounds = spawnZone.cellBounds;

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                if (spawnZone.HasTile(cell))
                {
                    availableCells.Add(cell);
                }
            }
        
            StartCoroutine(SpawnEnemies());
        }
        
        private void OnDifficultyChanged(DifficultyPreset preset)
        {
            currentSpeed = preset.mobSpeed;
        }
    }
}
