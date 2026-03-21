using _Code.MainGame.Level;
using UnityEngine;

namespace _Code.MainGame.Vines
{
    public class VinesSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxVines = 5;

        [Header("References")]
        // LevelSpawner handles valid spawn tiles and reserves a free one for us.
        [SerializeField] private LevelSpawner levelSpawner;

        // This is the vine object to place in the level.
        [SerializeField] private GameObject vinePrefab;

        private int _spawnCounter;
        private bool _continueSpawning = true;

        private void Start()
        {
            if (spawnInterval > 0f)
                InvokeRepeating(nameof(SpawnVine), spawnInterval, spawnInterval);
        }

        public void StopSpawning()
        {
            _continueSpawning = false;
            CancelInvoke(nameof(SpawnVine));
        }

        private void SpawnVine()
        {
            if (!_continueSpawning) return;
            if (_spawnCounter >= maxVines) return;
            if (!levelSpawner) return;
            if (!vinePrefab) return;

            // Ask LevelSpawner for one free valid tile.
            // That keeps vines from spawning on blocked or already used spots.
            if (!levelSpawner.TryReserveRandomSpawnPosition(out var spawnPosition, out _))
                return;

            Instantiate(vinePrefab, spawnPosition, Quaternion.identity);
            _spawnCounter++;
        }
    }
}