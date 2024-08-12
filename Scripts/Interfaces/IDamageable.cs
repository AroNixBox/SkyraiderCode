using UnityEngine;
public interface IDamagable
{
    void Damage(float damageAmount, Vector3 hitPoint, Vector3 hitForward);
}