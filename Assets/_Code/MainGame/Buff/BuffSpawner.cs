using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Code.MainGame.Buff
{
    public class BuffSpawner : MonoBehaviour
    {
        [Header("Buff Prefabs")]
        [Tooltip("Default prefab for speed buffs (e.g. FishBuff).")]
        [SerializeField] private GameObject buffPrefab;
        [Tooltip("Setup + chance + optional prefab override (e.g. ImmunityBuff with goldenfish for Immunity).")]
        [SerializeField] private List<BuffChance> _buffSetups;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxBuffs = 5;

        [SerializeField] private Tilemap spawnZone;
        [SerializeField] private Tilemap obstacleZone;
        private List<Vector3Int> availableCells;

        private int _spawnCounter;
        private bool _continueSpawning = true;

        private void Start()
        {
            availableCells = new List<Vector3Int>();
            BoundsInt bounds = spawnZone.cellBounds;

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                if (spawnZone.HasTile(cell))
                {
                    availableCells.Add(cell);
                }
            }
            
                        
            foreach (Vector3Int cell in obstacleZone.cellBounds.allPositionsWithin)
            {
                if (obstacleZone.HasTile(cell) && availableCells.Contains(cell))
                {
                    availableCells.Remove(cell);
                }
            }
            
            InvokeRepeating(nameof(SpawnBuff), spawnInterval, spawnInterval);
        }

        public void StopSpawning()
        {
            _continueSpawning = false;
            CancelInvoke(nameof(SpawnBuff));
        }

        private void SpawnBuff()
        {
            if (_spawnCounter >= maxBuffs || !_continueSpawning) return;
            if (availableCells == null || availableCells.Count == 0) return;
            if (_buffSetups == null || _buffSetups.Count == 0) return;

            float totalChance = 0f;
            foreach (BuffChance buffChance in _buffSetups)
            {
                if (buffChance.Setup != null && buffChance.Chance > 0f)
                    totalChance += buffChance.Chance;
            }

            if (totalChance <= 0f) return;

            float roll = Random.Range(0f, totalChance);
            BuffSetup selectedSetup = null;
            GameObject prefabToSpawn = buffPrefab;

            foreach (BuffChance buffChance in _buffSetups)
            {
                if (buffChance.Setup == null || buffChance.Chance <= 0f) continue;

                roll -= buffChance.Chance;
                if (roll <= 0f)
                {
                    selectedSetup = buffChance.Setup;
                    prefabToSpawn = buffChance.PrefabOverride != null ? buffChance.PrefabOverride : buffPrefab;
                    break;
                }
            }

            if (selectedSetup == null || prefabToSpawn == null) return;

            Vector3Int randomCell = availableCells[Random.Range(0, availableCells.Count)];
            Vector3 spawnPosition = spawnZone.GetCellCenterWorld(randomCell);

            GameObject createdBuff = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            createdBuff.GetComponent<Buff>().Initialize(selectedSetup);

            _spawnCounter++;
        }
        
        [Serializable]
        private class BuffChance
        {
            [Tooltip("Buff data (type, duration, value).")]
            public BuffSetup Setup;
            [Tooltip("Spawn weight (higher = more likely).")]
            public float Chance;
            [Tooltip("Optional: prefab for this buff (e.g. ImmunityBuff with goldenfish). If empty, uses default Buff Prefab.")]
            public GameObject PrefabOverride;

            public BuffChance(BuffSetup setup, float chance)
            {
                Setup = setup;
                Chance = chance;
            }
        }
        
    }
}