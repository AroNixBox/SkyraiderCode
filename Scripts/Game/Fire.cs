using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private PlayerHealth _playerHealth;
    private float fireDamageTimer = 0f;
    private float damageInsideFire = 20f;

    private void Start()
    {
        _playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (fireDamageTimer <= 0)
            {
                _playerHealth.Damage(damageInsideFire, transform.position, transform.forward);
                fireDamageTimer = 2f;
            }
            else
            {
                fireDamageTimer -= Time.deltaTime;
            }
        }
    }
}
