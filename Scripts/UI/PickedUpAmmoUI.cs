using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PickedUpAmmoUI : MonoBehaviour
{
    [SerializeField] private ShooterController shooterController;
    [SerializeField] private AudioClip ammoPickedUpSound;

    void OnSingleAmmoAdded()
    {
        SoundManager.Instance.Play2DSound(ammoPickedUpSound, .05f);
        shooterController.AddSingleAmmo();
    }
    
}
