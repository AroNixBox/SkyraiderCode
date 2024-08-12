using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class InfiniteAmmoSpawner : MonoBehaviour
{
    private ShooterController _playerShooterController;
    private float cooldown = .1f;

    private void Start()
    {
        _playerShooterController = FindObjectOfType<ShooterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (cooldown <= 0)
            {
                _playerShooterController.AddSingleAmmo();
                cooldown = .1f;   
            }
            else
            {
                cooldown -= Time.deltaTime;
            }
                
        }
    }
}
