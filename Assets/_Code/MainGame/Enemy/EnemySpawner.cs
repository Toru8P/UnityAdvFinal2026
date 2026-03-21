using System;
using System.Collections;
using System.Collections.Generic;
using _Code.MainGame.Enemy.Difficulty;
using _Code.MainGame.Level;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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

        [Header("References")]
        // LevelSpawner now owns the tile lookup and free spot reservation logic.
        [SerializeField] private LevelSpawner levelSpawner;

        [Header("Enemy Light Settings")]
        [SerializeField] private bool enemiesHaveLight = false;
        
        [Header("Difficulty Settings")]
        [SerializeField] private float currentSpeed;
        
        [Header("Visual Enemies Counter")]
        [SerializeField] private TextMeshProUGUI enemiesCounterText;
        
        private int _spawnCounter;
        private bool _continueSpawning = true;
    
        private float _timer;
        
        private IEnumerator SpawnEnemies()
        {
            while (_spawnCounter < maxEnemies && _continueSpawning)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void LateUpdate()
        {
             UpdateEnemiesCounterText();
        }

        private void UpdateEnemiesCounterText()
        {
            if (!enemiesCounterText)
            {
                return;
            }
            enemiesCounterText.text = $"Enemies: {_spawnCounter}";
        }
        
        public void StopSpawning()
        {
            _continueSpawning = false;
        }

        public void OnPlayerDeath()
        {
            StopSpawning();
        }
    
        private void SpawnEnemy()
        {
            if (_continueSpawning && enemyLowPrefab && enemyMidPrefab && enemyHiPrefab && target)
            {
                if (!levelSpawner.TryReserveRandomSpawnPosition(out var spawnPosition, out _))
                    return;
                
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
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

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
            StartCoroutine(SpawnEnemies());
        }
        
        private void OnDifficultyChanged(DifficultyPreset preset)
        {
            currentSpeed = preset.mobSpeed;
        }
    }
}
