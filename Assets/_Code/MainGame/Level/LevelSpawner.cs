using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Code.MainGame.Level
{
    public class LevelSpawner : MonoBehaviour
    {
        [Header("Spawn Zones")]
        [Tooltip("Tiles that are allowed to spawn gameplay objects.")]
        [SerializeField] private Tilemap spawnZone;

        [Tooltip("Tiles that block spawning, like walls or map obstacles.")]
        [SerializeField] private Tilemap obstacleZone;

        private readonly List<Vector3Int> _validCells = new();
        private readonly HashSet<Vector3Int> _occupiedCells = new();

        private bool _isInitialized;

        private void Awake()
        {
            CacheValidCells();
        }

        // Builds the list of floor cells that can be used for spawning.
        // This is done once up front so spawners do not scan the tilemap every spawn.
        private void CacheValidCells()
        {
            _validCells.Clear();
            _occupiedCells.Clear();
            _isInitialized = false;

            if (!spawnZone)
            {
                Debug.LogWarning($"{nameof(LevelSpawner)} is missing a spawn zone tilemap.", this);
                return;
            }

            foreach (var cell in spawnZone.cellBounds.allPositionsWithin)
            {
                if (spawnZone.HasTile(cell))
                    _validCells.Add(cell);
            }

            if (obstacleZone)
            {
                // Any blocked tile is removed now, so later spawns only look at valid floor cells.
                for (var i = _validCells.Count - 1; i >= 0; i--)
                {
                    if (obstacleZone.HasTile(_validCells[i]))
                        _validCells.RemoveAt(i);
                }
            }

            _isInitialized = true;
        }

        // Reserves one free cell and returns its world position.
        // Reserving here keeps two different spawners from using the same spot.
        public bool TryReserveRandomSpawnPosition(out Vector3 worldPosition, out Vector3Int reservedCell)
        {
            worldPosition = default;
            reservedCell = default;

            if (!_isInitialized || _validCells.Count == 0)
                return false;

            var freeCount = _validCells.Count - _occupiedCells.Count;
            if (freeCount <= 0)
                return false;

            // Try a few random cells first so normal cases stay cheap.
            var maxAttempts = Mathf.Min(_validCells.Count, 12);
            for (var i = 0; i < maxAttempts; i++)
            {
                var randomCell = _validCells[Random.Range(0, _validCells.Count)];
                if (_occupiedCells.Contains(randomCell)) continue;

                _occupiedCells.Add(randomCell);
                reservedCell = randomCell;
                worldPosition = spawnZone.GetCellCenterWorld(randomCell);
                return true;
            }

            // Fallback scan makes sure a free cell is still found when the map gets crowded.
            foreach (var cell in _validCells)
            {
                if (_occupiedCells.Contains(cell)) continue;

                _occupiedCells.Add(cell);
                reservedCell = cell;
                worldPosition = spawnZone.GetCellCenterWorld(cell);
                return true;
            }

            return false;
        }

        // Lets a spawned object free its tile later if it gets destroyed or consumed.
        public void ReleaseCell(Vector3Int cell)
        {
            _occupiedCells.Remove(cell);
        }

        public bool IsCellOccupied(Vector3Int cell)
        {
            return _occupiedCells.Contains(cell);
        }
    }
}