using System;
using System.Collections;
using Enemy.Monobehaviors;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class MeeleController : MonoBehaviour
    {
        public bool isInTutorial;
        
        private const float MinimalStabDistance = .2f;

        private PlayerReferences _playerReferences;
        
        private PlayerUI _playerUI;
        private PlayerAnimator _playerAnimator;
        private StarterAssetsInputs _starterAssetInputs;
        private ThirdPersonController _thirdPersonController;
        private ShooterController _shooterController;
        private Animator _animator;
        
        
        private EnemyHealth _enemyHealth;
        private const float BackstabDotOffset = 0.1f;
        private const float FrontstabDotOffset = -0.1f;
        private const float RotationSpeed = 5f;
        private bool _isInBackstabPosition;
        private bool _isInFrontstabPosition;
        
        private enum StabbingPosition
        {
            None,
            Back,
            Front
        }
        private enum AttackType
        {
            Backstab, 
            Frontstab
        }
        
        private StabbingPosition _currentStabbingPosition = StabbingPosition.None;
        private StabbingPosition _lastStabbingPosition = StabbingPosition.None;
        [SerializeField] private LayerMask enemyLayerMask;
        [SerializeField] private float detectClosestGuardRadius = 10f;
        [SerializeField] private float stabDistance = 2f;

        void Awake()
        {
            _playerReferences = GetComponent<PlayerReferences>();
        }
    
        private void Start()
        {
            _shooterController = _playerReferences.ShooterController;
            _playerUI = _playerReferences.PlayerUI;
            _playerAnimator = _playerReferences.PlayerAnimator;
            _starterAssetInputs = _playerReferences.StarterAssetsInputs;
            _thirdPersonController = _playerReferences.ThirdPersonController;
            _animator = _playerReferences.Animator;
            
            _playerUI.SetActiveWeapon(PlayerUI.WeaponType.Rifle);
        }
    
        void Update()
        {
            if(isInTutorial) return;
            
            DetectClosestGuard();
            UpdateStabbingPosition();
            ShowHints();
            HandleMeeleAttack();
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectClosestGuardRadius);
        }
        private void DetectClosestGuard()
        {
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectClosestGuardRadius, enemyLayerMask);
            float closestDistanceSqr = Mathf.Infinity;
            EnemyHealth closestEnemy = null;
    
            foreach (Collider col in enemiesInRange)
            {
                if (col.TryGetComponent(out EnemyHealth potentiallyClosestEnemy))
                {
                    float distanceToGuardSqr = (potentiallyClosestEnemy.transform.position - transform.position).sqrMagnitude;
                    if (distanceToGuardSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceToGuardSqr;
                        closestEnemy = potentiallyClosestEnemy;
                    }
                }
            }
    
            if (closestEnemy != null)
            {
                _enemyHealth = closestEnemy;
    
                Transform enemyTransform = _enemyHealth.transform;
                Vector3 dirFromEnemyToPlayer = (transform.position - enemyTransform.position).normalized;
                float dot = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);
    
                _isInBackstabPosition = IsInPositionForAttack(dot, AttackType.Backstab);
                _isInFrontstabPosition = IsInPositionForAttack(dot, AttackType.Frontstab);
            }
            else
            {
                _enemyHealth = null;
                _isInBackstabPosition = false;
                _isInFrontstabPosition = false;
            }
        }
        private void UpdateStabbingPosition()
        {
            if (!_enemyHealth)
            {
                _currentStabbingPosition = StabbingPosition.None;
                return;
            }
    
            if (_isInBackstabPosition)
            {
                _currentStabbingPosition = StabbingPosition.Back;
            }
            else if (_isInFrontstabPosition)
            {
                _currentStabbingPosition = StabbingPosition.Front;
            }
            else
            {
                _currentStabbingPosition = StabbingPosition.None;
            }
        }
        private void ShowHints()
        {
            if (_lastStabbingPosition == _currentStabbingPosition)
            {
                return; 
            }
    
            string actionText = "";
            switch (_currentStabbingPosition)
            {
                case StabbingPosition.Back:
                    actionText = "Press LMB to Backstab";
                    break;
    
                case StabbingPosition.Front:
                    actionText = "Press LMB to Frontstab";
                    break;
    
                default:
                    actionText = "";
                    break;
            }
            _playerUI.ShowStabbingText(actionText, !string.IsNullOrEmpty(actionText));
            _lastStabbingPosition = _currentStabbingPosition;
        }

        bool IsInPositionForAttack(float dot, AttackType attackType)
        {
            float attackThreshold;
            bool comparison = false;

            switch (attackType)
            {
                case AttackType.Backstab:
                    attackThreshold = -1 + BackstabDotOffset;
                    comparison = dot < attackThreshold;
                    break;
        
                case AttackType.Frontstab:
                    attackThreshold = 1 + FrontstabDotOffset;
                    comparison = dot > attackThreshold;
                    break;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, _enemyHealth.transform.position);
            bool isCloseEnough = distanceToEnemy < stabDistance;
            
            bool isVeryClose = distanceToEnemy < MinimalStabDistance;

            return (comparison && isCloseEnough) || isVeryClose;
        }
        
        

        private void PrepareAttack(int attackType, EnemyHealth.ExecutionState executionState)
        {
            _playerAnimator.IsMeeleAttacking = true;
            _thirdPersonController.isStopped = true;
            _animator.SetFloat(GlobalAnimationHashes.PlayerAnim_Speed, 0f);
            _playerAnimator.StartEquipMeele();
            Vector3 dirToEnemy = (_enemyHealth.transform.position - transform.position).normalized;
            dirToEnemy.y = 0;
    
            StartCoroutine(RotateTowardsEnemy(dirToEnemy, () =>
            {
                ExecuteAttack(attackType, executionState);
            }));
        }
        private void ExecuteAttack(int attackType, EnemyHealth.ExecutionState executionState)
        {
            StartCoroutine(WaitForAnimation("EquipMeele",() =>
            {
                _animator.SetTrigger(attackType);
                _enemyHealth.Init(p => ReEnableControls(), executionState, _shooterController);
            }));
        }

    
        private IEnumerator RotateTowardsEnemy(Vector3 targetDirection, Action callback)
        {
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    
            float elapsedTime = 0f;
    
            while (elapsedTime < 1f)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
                elapsedTime += Time.deltaTime * RotationSpeed;
                yield return null;
            }

            callback?.Invoke();
        }
     
        
        IEnumerator WaitForAnimation(string animationNameInAnimator, Action callback)
        {
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(1).IsName(animationNameInAnimator));
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f);
            callback?.Invoke();
        }
        
    
        private void HandleMeeleAttack()
        {
            if (_playerAnimator.IsMeeleAttacking || !_starterAssetInputs.shoot ||
                !_thirdPersonController.Grounded || !_enemyHealth 
                || _starterAssetInputs.aim) return;
            
            
            if (_shooterController.isReloading)
            {
                _shooterController.ExitAimingMode();
                _shooterController.isReloading = false;
                _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Reload, false);
            }
            
            
            if (_isInBackstabPosition)
            {
                PrepareAttack(GlobalAnimationHashes.PlayerAnim_Backstab, EnemyHealth.ExecutionState.Backstab);
            }
            else if (_isInFrontstabPosition)
            {
                PrepareAttack(GlobalAnimationHashes.PlayerAnim_Frontstab, EnemyHealth.ExecutionState.Frontstab);
            }

        }
                
        private void ReEnableControls()
        {
            _playerAnimator.StartUnEquipMeele();
            _thirdPersonController.isStopped = false;
        }
    
    }
}

