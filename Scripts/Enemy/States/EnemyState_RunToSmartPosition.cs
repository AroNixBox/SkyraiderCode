using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_RunToSmartPosition : IState
    {
        private EnemyReferences _enemyReferences;
        private bool _hasSetSmartPosition;
        private bool _initialSmartPosition;
        private float _minDistanceFromOtherEnemies = 3f;
        
        public bool CoverCloserThanPlayer { get; private set; }
    
        public EnemyState_RunToSmartPosition(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
    
        public void OnEnter()
        {
            _hasSetSmartPosition = false;
            CoverCloserThanPlayer = false;
            _enemyReferences.NavMeshAgent.SetDestination(_enemyReferences.Player.position);
            
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
        }
    
        public void Tick()
        {
            bool isPlayerOutOfRangeOrCantBeSeen = _enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, true);
    
            if (isPlayerOutOfRangeOrCantBeSeen)
            {
                if (!_hasSetSmartPosition)
                {
                    Vector3 smartPosition = GameManager.Instance.RequestNewPosition(
                        _enemyReferences.EnemyHead.position, 
                        _enemyReferences.PlayerHead.position, 
                        _enemyReferences.Vision.shootRange / 2f + _enemyReferences.Vision.shootRange / 4f, 
                        _enemyReferences.Vision.alarmRadius / 2f,
                        _minDistanceFromOtherEnemies);
                    _enemyReferences.NavMeshAgent.SetDestination(smartPosition);
                    _hasSetSmartPosition = true;
                    
                    if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
                    {
                        GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
                    }
                    GameManager.Instance.RegisterCover(_enemyReferences.gameObject.name, smartPosition);
                    
                    Cover betterCover = _enemyReferences.Vision.GetBetterCoverPosition(_enemyReferences.Covers, smartPosition);
                    
                    if (betterCover != null)
                    {
                        CoverCloserThanPlayer = true;
                        return;
                    }
                }
            }
            else if (!_hasSetSmartPosition)
            {
                _enemyReferences.NavMeshAgent.SetDestination(_enemyReferences.Player.position);
            }
            //TODO do this different.... Cover check is expensive
            if (!isPlayerOutOfRangeOrCantBeSeen && !GameManager.Instance.IsTooCloseToOtherPositions(_enemyReferences.EnemyHead.position, _minDistanceFromOtherEnemies))
            {
                Cover betterCover = _enemyReferences.Vision.GetBetterCoverPosition(_enemyReferences.Covers, _enemyReferences.EnemyHead.position);
                    
                if (betterCover != null)
                {
                    CoverCloserThanPlayer = true;
                    return;
                }
                        
                _enemyReferences.NavMeshAgent.SetDestination(_enemyReferences.EnemyHead.position);
                _hasSetSmartPosition = true;
            }
            
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_Speed, _enemyReferences.NavMeshAgent.desiredVelocity.sqrMagnitude);
            _enemyReferences.Vision.CallAlarmInRange(_enemyReferences.Vision.alarmRadius / 2f);
        }

        public void OnExit()
        {
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_Speed, 0f);
            _enemyReferences.NavMeshAgent.ResetPath();
            _hasSetSmartPosition = false;
        }
    
        public Color GizmoState()
        {
            return Color.blue;
        }
    
        public bool HasArrivedAtDestination()
        {
            return _enemyReferences.NavMeshAgent.remainingDistance < 0.1f && !_enemyReferences.NavMeshAgent.pathPending;
        }
    }
}
