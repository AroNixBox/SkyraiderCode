using System;
using System.Collections;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealth : MonoBehaviour, IDamagable
    {
        [Header("Gameplay Affecting Properties")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float criticalHealth = 10f;
        
        [Space(10)]
        [SerializeField] private float blurDuration = 0.1f;
        [SerializeField] private float blurFadeSpeed = 5f;
        
        [Header("References")]
        [SerializeField] private Image bloodyFrame;
        [SerializeField] private Image blurImage;
        
        [Space(10)]
        [SerializeField] private AudioClip[] damageSounds;
        [SerializeField] private AudioClip pulseSound;
        
        private AudioSource _audioSource;

        private PlayerReferences _playerReferences;
        private PlayerUI _playerUI;
        private Animator _animator;
        private float _currentHealth;
        private Coroutine _blurCoroutine;

        private void Awake()
        {
            _playerReferences = GetComponent<PlayerReferences>();
            _animator = bloodyFrame.GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _playerUI = _playerReferences.PlayerUI;
            _currentHealth = maxHealth;
            OnHealthChanged(_currentHealth);
        }

        private float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, maxHealth);
                OnHealthChanged(_currentHealth);
            }
        }

        private void Update()
        {
            HandlePulseSound();
        }

        public void Damage(float damageAmount, Vector3 hitPoint, Vector3 hitForward)
        {
            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0)
            {
                Die();
                GameManager.Instance.PlayerDied();
                VFXManager.Instance.SpawnParticle(ParticleType.SmallBloodImpact, hitPoint, Quaternion.LookRotation(hitForward));
            }
            else
            {
                _audioSource.PlayOneShot(damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)]);
                VFXManager.Instance.SpawnParticle(ParticleType.BigBloodImpact, hitPoint, Quaternion.LookRotation(hitForward));
            }
        }

        private void OnHealthChanged(float health)
        {
            _playerUI.UpdatePlayerHitpoints(health);
            UpdateVisualEffects();
        }

        private void UpdateVisualEffects()
        {
            bloodyFrame.enabled = _currentHealth < maxHealth;
            if(_currentHealth <= criticalHealth)
                _animator.SetTrigger("pulsate");

            if (blurImage != null && _currentHealth < maxHealth && _blurCoroutine == null)
            {
                _blurCoroutine = StartCoroutine(ShowRadialBlur());
            }

            if (_audioSource != null)
            {
                if (_currentHealth <= criticalHealth && !_audioSource.isPlaying)
                {
                    _audioSource.volume = 0f;
                    _audioSource.PlayOneShot(pulseSound);
                }
                else if (_currentHealth > criticalHealth && _audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
            }
        }

        private IEnumerator ShowRadialBlur()
        {
            blurImage.enabled = true;
            var targetAlpha = 1f;
            while (blurImage.color.a < targetAlpha)
            {
                var tempColor = blurImage.color;
                tempColor.a += Time.deltaTime * blurFadeSpeed;
                blurImage.color = tempColor;
                yield return null;
            }

            yield return new WaitForSeconds(blurDuration);

            while (blurImage.color.a > 0f)
            {
                var tempColor = blurImage.color;
                tempColor.a -= Time.deltaTime * blurFadeSpeed;
                blurImage.color = tempColor;
                yield return null;
            }

            blurImage.enabled = false;
            _blurCoroutine = null;
        }

        private void HandlePulseSound()
        {
            if (pulseSound != null && _audioSource.isPlaying)
            {
                _audioSource.volume = Mathf.Clamp01(_audioSource.volume + Time.deltaTime);
            }
        }

        private void Die()
        {
            if (pulseSound != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
            if (_animator != null)
            {
                _animator.enabled = false;
            }
            if (_blurCoroutine != null)
            {
                StopCoroutine(_blurCoroutine);
                _blurCoroutine = null;
            }
            if (bloodyFrame != null)
            {
                bloodyFrame.color = new Color(0, 0, 0, 255);
            }
            
            
            foreach (var component in GetComponents<Component>())
            {
                if (component is MonoBehaviour monoBehaviour && !(component is Animator))
                {
                    monoBehaviour.enabled = false;
                }
    
                if (component is Animator animator)
                {
                    animator.SetLayerWeight(1, 0f);
                    animator.SetTrigger(GlobalAnimationHashes.PlayerAnim_Death);
                }
                
                if (component is Collider col)
                {
                    col.enabled = false;
                }
                //TODO Add Callback so the Camera zooms out and game over screen is shown
            }
        }
        
    }
}
