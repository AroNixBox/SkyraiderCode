using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Enemy.References;
using Extensions;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Monobehaviors
{
    public class EnemyHealth : MonoBehaviour, IDamagable
    {
        [SerializeField] private float maxHealth = 100f;
        private readonly int _ammoRewardForPlayer = 4;
        [SerializeField] private Transform backStabBloodPostion;
        private EnemyReferences _enemyReferences;
        private Transform _enemyHead;
        private AudioSource _audioSource;
        [SerializeField] private AudioClip[] backstabGore;
        [SerializeField] private AudioClip[] backstabGorePullOut;
        [SerializeField] private AudioClip[] backstabScream;
        [SerializeField] private AudioClip[] backstabScreamPullOut;
        [SerializeField] private AudioClip[] trashTalkSounds;
    
        private float _currentHealth;
        private Action<EnemyHealth> _onMeeleDieAction;
        private ShooterController _playerShooterController;
        //TODO DO this somewhat else, currently backstab needs to be triggered via bool!
        [HideInInspector] public bool gotBackStabbed;
        [HideInInspector] public bool gotfrontStabbed;

        private bool isNowDead;
    
        void Awake()
        {
            _enemyReferences = GetComponent<EnemyReferences>();
        }
    
        private void Start()
        {
            _enemyHead = _enemyReferences.EnemyHead;
            _audioSource = _enemyReferences.AudioSource;
            _currentHealth = maxHealth;
        }
    
        public enum ExecutionState
        {
            Frontstab,
            Backstab
        }
        
        
        public void Init(Action<EnemyHealth> onDieAction, ExecutionState executionState, ShooterController playerShooterController)
        {
            switch (executionState)
            {
                case ExecutionState.Backstab:
                    gotBackStabbed = true;
                    break;
                case ExecutionState.Frontstab:
                    gotfrontStabbed = true;
                    break;
            }
            
            _onMeeleDieAction = onDieAction;
            
            TimerUtility.SetTimer(playerShooterController, () =>
            {
                if (!playerShooterController || !playerShooterController.enabled) return;
                
                playerShooterController.AddAmmo(_ammoRewardForPlayer);
                
            }, 1.3f);
        }
    
        public void Damage(float damage, Vector3 hitPoint, Vector3 hitForward)
        { 
            GameManager.Instance.AlarmEnemiesInRange(transform.position, 7f);
            _currentHealth -= damage;
            
            if (_currentHealth <= 0)
            {
                GameManager.Instance.UnregisterEnemy(this);
                AudioSource.PlayClipAtPoint(backstabGore[UnityEngine.Random.Range(0, backstabGore.Length)], backStabBloodPostion.position, 2f);
                Quaternion rotation = Quaternion.LookRotation(hitForward);
                VFXManager.Instance.SpawnParticle(ParticleType.SmallBloodImpact, hitPoint, rotation);
            }
            else
            {
                Quaternion rotation = Quaternion.LookRotation(hitForward);
                VFXManager.Instance.SpawnParticle(ParticleType.BigBloodImpact, hitPoint, rotation);
            }
        }
        public bool GotShot()
        {
            return _currentHealth <= 0;
        }
    
        public void OnTrashTalk()
        {
            StartCoroutine(PlayTrashTalkWithDelay());
        }
        private IEnumerator PlayTrashTalkWithDelay()
        {
            float randomPitch = UnityEngine.Random.Range(.75f, 1.1f);
            AudioClip selectedClip = trashTalkSounds[UnityEngine.Random.Range(0, trashTalkSounds.Length)];
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(.2f, 1f));
                _audioSource.pitch = randomPitch;
                _audioSource.PlayOneShot(selectedClip, .8f);
                yield return new WaitForSeconds(selectedClip.length);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 6f));
            }
        }
        public void OnSwordHitEnemyBack()
        {
            Quaternion rotation = Quaternion.LookRotation(backStabBloodPostion.forward);
            VFXManager.Instance.SpawnParticle(ParticleType.BigBloodImpact, backStabBloodPostion.position, rotation);
            AudioSource.PlayClipAtPoint(backstabGore[UnityEngine.Random.Range(0, backstabGore.Length)], backStabBloodPostion.position, 2f);
            AudioSource.PlayClipAtPoint(backstabScream[UnityEngine.Random.Range(0, backstabScream.Length)], _enemyHead.position, .4f);
        }
    
        public void OnSwordPullOut()
        {
            AudioSource.PlayClipAtPoint(backstabGorePullOut[UnityEngine.Random.Range(0, backstabGore.Length)], backStabBloodPostion.position, 2f);
            AudioSource.PlayClipAtPoint(backstabScreamPullOut[UnityEngine.Random.Range(0, backstabScream.Length)], _enemyHead.position, .4f);
        }
        public void OnExecutionComplete()
        {
            BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            GetComponent<NavMeshAgent>().enabled = false;
            _onMeeleDieAction(this);
        }
        public void OnGotShot()
        {
            BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            GetComponent<NavMeshAgent>().enabled = false;
        }
        public bool GotExecuted(ExecutionState executionState)
        {
            switch (executionState)
            {
                case ExecutionState.Backstab:
                    if (gotBackStabbed)
                    {
                        if (!isNowDead)
                        {
                            Debug.Log("called once");
                            GameManager.Instance.UnregisterEnemy(this);
                        }
                        
                        isNowDead = true;
                        return true;
                    }
                    break;
                case ExecutionState.Frontstab:
                    if (gotfrontStabbed)
                    {
                        if (!isNowDead)
                        {
                            Debug.Log("called once");
                            GameManager.Instance.UnregisterEnemy(this);
                        }
                        
                        isNowDead = true;
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}


