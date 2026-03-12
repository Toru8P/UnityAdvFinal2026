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
        [SerializeField] private List<BuffChance> _buffSetups;
        [SerializeField] private GameObject buffPrefab;

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
                if (availableCells.Contains(cell))
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

            foreach (BuffChance buffChance in _buffSetups)
            {
                if (buffChance.Setup == null || buffChance.Chance <= 0f) continue;

                roll -= buffChance.Chance;
                if (roll <= 0f)
                {
                    selectedSetup = buffChance.Setup;
                    break;
                }
            }

            if (selectedSetup == null) return;

            Vector3Int randomCell = availableCells[Random.Range(0, availableCells.Count)];
            Vector3 spawnPosition = spawnZone.GetCellCenterWorld(randomCell);

            GameObject createdBuff = Instantiate(buffPrefab, spawnPosition, Quaternion.identity);
            createdBuff.GetComponent<Buff>().Initialize(selectedSetup);

            _spawnCounter++;
        }
        
        [Serializable]
        private class BuffChance
        {
            public BuffSetup Setup;
            public float Chance;

            public BuffChance(BuffSetup setup, float chance)
            {
                Setup = setup;
                Chance = chance;
            }
        }
        
    }
}