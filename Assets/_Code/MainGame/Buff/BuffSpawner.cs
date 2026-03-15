using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Code.MainGame.Buff
{
    public class BuffSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("_buffSetups")]
        [Tooltip("Setup + chance + optional prefab override (e.g. ImmunityBuff with goldenfish for Immunity).")]
        [SerializeField] private List<BuffChance> buffSetups;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxBuffs = 5;

        [SerializeField] private Tilemap spawnZone;
        [SerializeField] private Tilemap obstacleZone;
        private List<Vector3Int> _availableCells;

        private int _spawnCounter;
        private bool _continueSpawning = true;

        private void Start()
        {
            _availableCells = new List<Vector3Int>();
            BoundsInt bounds = spawnZone.cellBounds;

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                if (spawnZone.HasTile(cell))
                {
                    _availableCells.Add(cell);
                }
            }
            
                        
            foreach (Vector3Int cell in obstacleZone.cellBounds.allPositionsWithin)
            {
                if (obstacleZone.HasTile(cell) && _availableCells.Contains(cell))
                {
                    _availableCells.Remove(cell);
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
            if (_availableCells == null || _availableCells.Count == 0) return;
            if (buffSetups == null || buffSetups.Count == 0) return;

            float totalChance = 0f;
            foreach (BuffChance buffChance in buffSetups)
            {
                if (buffChance.Setup && buffChance.Chance > 0f)
                    totalChance += buffChance.Chance;
            }

            if (totalChance <= 0f) return;

            float roll = Random.Range(0f, totalChance);

            foreach (BuffChance buff in buffSetups)
            {
                if (!buff.Setup || buff.Chance <= 0f) continue;

                roll -= buff.Chance;
                if (roll <= 0f)
                {
                    if (SpawnAtTile(buff.PrefabOverride, buff.Setup)) _spawnCounter++;
                    break;
                }
            }
        }
        
        private bool SpawnAtTile(GameObject prefabToSpawn, BuffSetup selectedSetup)
        {
            if (!prefabToSpawn) return false;
            if (!selectedSetup) return false;
            
            Vector3Int randomCell = _availableCells[Random.Range(0, _availableCells.Count)];
            Vector3 spawnPosition = spawnZone.GetCellCenterWorld(randomCell);

            GameObject createdBuff = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            createdBuff.GetComponent<Buff>().Initialize(selectedSetup);
            return true;
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