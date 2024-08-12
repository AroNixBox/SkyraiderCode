using System;
using Enemy.Monobehaviors;
using Enemy.References;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SingleSoundComponent : MonoBehaviour
    {
        [SerializeField] private AudioClip clipToPlay;
        [SerializeField] private AudioClip headSmashOnPianoClip;
        private AudioSource _audioSource;
        private float checkInterval = 0.2f;
        private float nextCheckTime = 0.0f; 

        private enum SoundDependency
        {
            None,
            Dependent
        }
        [Header("Sound Dependency")]
        [SerializeField] private SoundDependency soundDependency;

        [SerializeField] private EnemyReferences _enemyReferences;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            PlayThisClip();
        }

        private void PlayThisClip()
        {
            _audioSource.clip = clipToPlay;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void Update()
        {
            if(soundDependency != SoundDependency.Dependent) return;
            if (_enemyReferences == null) return;
            
            if (Time.time >= nextCheckTime)
            {
                nextCheckTime = Time.time + checkInterval;
                
                if (_enemyReferences.Health.GotShot() ||
                    _enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Backstab) ||
                    _enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Frontstab) ||
                    _enemyReferences.Vision.IsAlarm())
                {
                    StopThisClip();
                    
                    _audioSource.PlayOneShot(headSmashOnPianoClip);
                }
            }
        }

        private void StopThisClip()
        {
            _audioSource.Stop();
            enabled = false;
        }

    }
}
