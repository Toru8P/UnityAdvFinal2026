using UnityEngine;

namespace _Code.MainGame.Enemy.Difficulty
{
    [System.Serializable]
    public class DifficultyPreset
    {
        public int mobSpeed;
        
        public AnimationCurve mobSpeedCurve;
        
        public DifficultyPreset Copy()
        {
            return new DifficultyPreset
            {
                mobSpeed = this.mobSpeed,
                mobSpeedCurve = new AnimationCurve(this.mobSpeedCurve.keys),
            };
        }
    }
}