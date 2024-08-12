using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField] private AudioClip[] shootingSounds;
        private PlayerReferences _playerReferences;
        private ShooterController _shooterController;
    
        [SerializeField] private AudioClip magRemoveSound;
        [SerializeField] private AudioClip magPutInSound;
        [SerializeField] private AudioClip reloadBulletSound;
    
        private void Awake()
        {
            _playerReferences = GetComponent<PlayerReferences>();
        }
        private void Start()
        {
            _shooterController = _playerReferences.ShooterController;
            _shooterController.OnShootAsObservable.Subscribe(_ => PlayRandomShootingSound()).AddTo(this);
        }
        public void OnMagRemove()
        {
            SoundManager.Instance.Play2DSound(magRemoveSound, .5f);
        }
    
        public void OnMagPutIn()
        {
            SoundManager.Instance.Play2DSound(magPutInSound, .2f);
        }
    
        public void OnReloacBullet()
        {
            SoundManager.Instance.Play2DSound(reloadBulletSound, .1f);
        }
       
    
        private void PlayRandomShootingSound()
        {
            int randomIndex = Random.Range(0, shootingSounds.Length);
            SoundManager.Instance.Play2DSound(shootingSounds[randomIndex], .2f);
        }
    }
}


