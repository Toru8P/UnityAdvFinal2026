using UnityEngine;

namespace _Code.MainGame.Buff
{
    [CreateAssetMenu(fileName = "NewBuff", menuName = "Buffs/BuffSetup")]
    public class BuffSetup : ScriptableObject
    {
        public BuffType buffType;
        public float duration;
        public float value;
    }
}