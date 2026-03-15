using _Code.MainGame.Player;
using UnityEngine;

namespace _Code.MainGame.Buff
{
    public class Buff : MonoBehaviour
    {
        public BuffType buffType;
        public float duration;
        public float value;

        public void Initialize(BuffSetup setup)
        {
            if (!setup) return;
            buffType = setup.buffType;
            duration = setup.duration;
            value = setup.value;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                PlayerController player = other.collider.GetComponent<PlayerController>();
                if (player)
                {
                    bool attached = player.AttachBuff(new BuffAttachment(buffType, duration, value));
                    if (attached)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    
    public enum BuffType
    {
        SpeedBoost,
        Immunity,
    }
    
    public class BuffAttachment
    {
        public BuffType Type { get; }
        public float Value { get; }
        public float RemainingDuration { get; private set; }

        public BuffAttachment(BuffType type, float duration, float value)
        {
            Type = type;
            Value = value;
            RemainingDuration = duration;
        }

        public void Update(float deltaTime)
        {
            RemainingDuration -= deltaTime;
        }

        public bool IsExpired => RemainingDuration <= 0f;
    }
}

