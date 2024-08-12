using System;
using System.Collections;
using Enemy.Monobehaviors;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.References
{
    [DisallowMultipleComponent]
    public class EnemyReferences : MonoBehaviour
    {
        public NavMeshAgent NavMeshAgent { get; private set; }
        public Animator Animator { get; private set; }
        public EnemyVision Vision { get; private set; }
        public EnemyShooter Shooter { get; private set; }
        public EnemyHealth Health { get; private set; }
        public AudioSource AudioSource { get; private set; }
        
        public Covers Covers { get; private set; }
        public bool IsPlayerDead { get; private set; }

        public Transform EnemyHead;
        public Transform PlayerHead;
        public Transform Player;
        public Transform[] PatrolPoints;
        
        private Action _callback;
        
    
        private void Awake()
        {
            Health = GetComponent<EnemyHealth>();
            Vision = GetComponent<EnemyVision>();
            Shooter = GetComponent<EnemyShooter>(); 
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Covers = FindObjectOfType<Covers>();
            GameManager.Instance.OnPlayerDied += OnPlayerDeath;
        }
        
        void OnPlayerDeath()
        {
            IsPlayerDead = true;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnPlayerDied -= OnPlayerDeath;
        }

        /// <summary>
        /// Referemces to Monobehavior Methods that are needed in states
        /// </summary>
        public void RequestEndOfAnimation(string animationNameInAnimator, int animationLayer,Action callback)
        {
            StartCoroutine(WaitForAnimation(animationNameInAnimator, animationLayer, callback));
        }
        IEnumerator WaitForAnimation(string animationNameInAnimator, int animationLayer,Action callback)
        {
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(animationLayer).IsName(animationNameInAnimator));
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(animationLayer).normalizedTime >= 1.0f);
            callback?.Invoke();
        }
    }
}

