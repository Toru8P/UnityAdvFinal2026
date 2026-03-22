using System.Collections;
using System.Collections.Generic;
using _Code.MainGame.Enemy.Difficulty;
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
        [SerializeField] private Tilemap spawnZone;

        [Header("Enemy Light Settings")]
        [SerializeField] private bool enemiesHaveLight = false;

        [Header("Difficulty Settings")]
        [SerializeField] private float currentSpeed;

        [Header("Visual Enemies Counter")]
        [SerializeField] private TextMeshProUGUI enemiesCounterText;

        private int _spawnCounter;
        private bool _continueSpawning = true;

        private List<Vector3Int> _availableCells;
        private Coroutine _spawnRoutine;

        // Time left until the next spawn.
        // Starts at spawnInterval so the first enemy appears after the interval.
        private float _timeUntilNextSpawn;

        private void Awake()
        {
            var difficultySystem = GetComponent<DifficultySystem>();
            difficultySystem.SubscribeToDifficultyUpdate(OnDifficultyChanged);

            _availableCells = new List<Vector3Int>();
            var bounds = spawnZone.cellBounds;

            foreach (var cell in bounds.allPositionsWithin)
            {
                if (spawnZone.HasTile(cell))
                {
                    _availableCells.Add(cell);
                }
            }

            _timeUntilNextSpawn = spawnInterval;
        }

        private void Start()
        {
            StartSpawnRoutineIfNeeded();
        }

        private void OnEnable()
        {
            StartSpawnRoutineIfNeeded();
        }

        private void OnDisable()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        private void LateUpdate()
        {
            UpdateEnemiesCounterText();
        }

        private void StartSpawnRoutineIfNeeded()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!_continueSpawning)
            {
                return;
            }

            if (_spawnCounter >= maxEnemies)
            {
                return;
            }

            if (_spawnRoutine != null)
            {
                return;
            }

            _spawnRoutine = StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (_spawnCounter < maxEnemies && _continueSpawning)
            {
                while (_timeUntilNextSpawn > 0f)
                {
                    _timeUntilNextSpawn -= Time.deltaTime;
                    yield return null;
                }

                if (_spawnCounter >= maxEnemies || !_continueSpawning)
                {
                    break;
                }

                SpawnEnemy();
                _timeUntilNextSpawn = spawnInterval;
            }

            _spawnRoutine = null;
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

            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        public void OnPlayerDeath()
        {
            StopSpawning();
        }

        private void SpawnEnemy()
        {
            if (!_continueSpawning || !enemyLowPrefab || !enemyMidPrefab || !enemyHiPrefab || !target)
            {
                return;
            }

            if (_availableCells == null || _availableCells.Count == 0)
            {
                return;
            }

            var randomCell = _availableCells[Random.Range(0, _availableCells.Count)];

            GameObject enemyPrefab;
            switch (currentSpeed)
            {
                case 1:
                case 2:
                case 3:
                    enemyPrefab = enemyLowPrefab;
                    break;
                case 4:
                    enemyPrefab = enemyMidPrefab;
                    break;
                default:
                    enemyPrefab = enemyHiPrefab;
                    break;
            }

            var enemy = Instantiate(enemyPrefab, randomCell, Quaternion.identity);

            var controller = enemy.GetComponent<EnemyController>();
            if (controller)
            {
                controller.SetTarget(target);
                controller.SetSpeed(currentSpeed);
                controller.TurnLight(enemiesHaveLight);
            }
            _spawnCounter++;
        }

        private void OnDifficultyChanged(DifficultyPreset preset)
        {
            currentSpeed = preset.mobSpeed;
        }
    }
}