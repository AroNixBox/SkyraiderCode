using Enemy.Monobehaviors;
using Player;
using UnityEngine;

public class Bodypart : MonoBehaviour, IDamagable
{
    private enum TargetType
    {
        Player,
        Enemy
    }

    [SerializeField] private TargetType targetType;

    private IDamagable _damagableEntity;

    private void Awake()
    {
        switch (targetType)
        {
            case TargetType.Player:
                _damagableEntity = GetComponentInParent<PlayerHealth>();
                break;
            case TargetType.Enemy:
                _damagableEntity = GetComponentInParent<EnemyHealth>();
                break;
        }   
    }

    public enum HitboxType
    {
        Head,
        Body
    }

    public HitboxType hitboxType;

    public void Damage(float damageAmount, Vector3 hitPoint, Vector3 hitForward)
    {
        _damagableEntity?.Damage(damageAmount, hitPoint, hitForward);
    }
}