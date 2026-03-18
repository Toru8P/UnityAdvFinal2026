using UnityEngine;

namespace _Code.MainGame.Enemy
{
    public interface IKnockbackable2D
    {
        void Knockback(Vector2 direction, float distance, float duration);
    }
}