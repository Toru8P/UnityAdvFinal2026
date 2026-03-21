using System;
using System.Collections.Generic;
using _Code.MainGame.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Code.MainGame.Buff
{
    public class BuffSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("_buffSetups")]
        [Tooltip("Buff setup, spawn weight, and prefab to spawn for that buff.")]
        [SerializeField] private List<BuffChance> buffSetups;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxBuffs = 5;

        [Header("References")]
        // LevelSpawner now owns the tile lookup and free spot reservation logic.
        [SerializeField] private LevelSpawner levelSpawner;

        private int _spawnCounter;
        private bool _continueSpawning = true;

        private void Start()
        {
            if (spawnInterval > 0f)
                InvokeRepeating(nameof(SpawnBuff), spawnInterval, spawnInterval);
        }

        public void StopSpawning()
        {
            _continueSpawning = false;
            CancelInvoke(nameof(SpawnBuff));
        }

        private void SpawnBuff()
        {
            if (!_continueSpawning) return;
            if (_spawnCounter >= maxBuffs) return;
            if (!levelSpawner) return;
            if (buffSetups == null || buffSetups.Count == 0) return;

            var selectedBuff = GetRandomBuffSetup();
            if (selectedBuff == null) return;
            if (!selectedBuff.Setup || !selectedBuff.PrefabOverride) return;

            if (!levelSpawner.TryReserveRandomSpawnPosition(out var spawnPosition, out _))
                return;

            var createdBuff = Instantiate(selectedBuff.PrefabOverride, spawnPosition, Quaternion.identity);
            var buff = createdBuff.GetComponent<Buff>();
            if (buff != null)
                buff.Initialize(selectedBuff.Setup);

            _spawnCounter++;
        }

        // Same weighted roll as before, just moved into its own helper.
        private BuffChance GetRandomBuffSetup()
        {
            var totalChance = 0f;

            foreach (var buffChance in buffSetups)
            {
                if (buffChance.Setup && buffChance.Chance > 0f)
                    totalChance += buffChance.Chance;
            }

            if (totalChance <= 0f)
                return null;

            var roll = UnityEngine.Random.Range(0f, totalChance);

            foreach (var buffChance in buffSetups)
            {
                if (!buffChance.Setup || buffChance.Chance <= 0f) continue;

                roll -= buffChance.Chance;
                if (roll <= 0f)
                    return buffChance;
            }

            return null;
        }

        [Serializable]
        private class BuffChance
        {
            [Tooltip("Buff data like type, duration, and value.")]
            public BuffSetup Setup;

            [Tooltip("Higher value means this buff is picked more often.")]
            public float Chance;

            [Tooltip("Prefab to spawn for this buff entry.")]
            public GameObject PrefabOverride;

            public BuffChance(BuffSetup setup, float chance)
            {
                Setup = setup;
                Chance = chance;
            }
        }
    }
}