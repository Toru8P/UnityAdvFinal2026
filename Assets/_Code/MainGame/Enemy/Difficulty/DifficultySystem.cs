using UnityEngine;
using UnityEngine.Events;

namespace _Code.MainGame.Enemy.Difficulty
{
    public class DifficultySystem : MonoBehaviour
    {
        [SerializeField] private UnityEvent<DifficultyPreset> onDifficultyUpdated = new UnityEvent<DifficultyPreset>();
        
        [SerializeField] private float maximumDifficultyTimePoint = 300f;
        private float _timeProgress;
        private float _lastInterval = 0f;
        [SerializeField] private float updateInterval = 2f;
        private float CurrentProgress => Mathf.Clamp01(_timeProgress / maximumDifficultyTimePoint);

        
        [SerializeField] private DifficultyPreset defaultPreset;
        public DifficultyPreset currentPreset;
        
        float CurrentSpeed => defaultPreset.mobSpeed * currentPreset.mobSpeedCurve.Evaluate(CurrentProgress);
        
        private void Start()
        {
            currentPreset = defaultPreset.Copy();
            UpdateDifficulty(false);
        }
        
        void FixedUpdate()
        {
            _timeProgress += Time.deltaTime;
            if (_timeProgress - _lastInterval > updateInterval)
            {
                _lastInterval = _timeProgress;
                UpdateDifficulty(true);
            }
        }

        private void UpdateDifficulty(bool progression)
        {
            if (progression) {
                currentPreset.mobSpeed = (int)CurrentSpeed;
            }
            onDifficultyUpdated.Invoke(currentPreset);
        }
        
        public void SubscribeToDifficultyUpdate(UnityAction<DifficultyPreset> action)
        {
            onDifficultyUpdated.AddListener(action);
        }
    }
}