using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_RunToCover : IState
    {
        private EnemyReferences _enemyReferences;
        private Covers _covers;
        private Cover _currentCover;

        public EnemyState_RunToCover(EnemyReferences enemyReferences, Covers covers)
        {
            _enemyReferences = enemyReferences;
            _covers = covers;
        }
        public void OnEnter()
        {
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
            
            Cover nextCover = _covers.GetNearestAvailableCover(_enemyReferences.EnemyHead, _enemyReferences.PlayerHead, _enemyReferences.Vision.shootRange, _enemyReferences.transform.name,true);
            if (nextCover != null)
            {
                _enemyReferences.NavMeshAgent.SetDestination(nextCover.transform.position);
            }
        }
        public void Tick()
        {
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_Speed, _enemyReferences.NavMeshAgent.desiredVelocity.sqrMagnitude);
            _enemyReferences.Vision.CallAlarmInRange(_enemyReferences.Vision.alarmRadius / 2f);
        }
        public void OnExit()
        {
            //In cover now, can Idle!
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_Speed, 0f);
        }
    
        public Color GizmoState()
        {
            return Color.red;
        }
        public Cover GetCurrentCover()
        {
            return _currentCover;
        }
        public bool HasArrivedAtDestination()
        {
            return _enemyReferences.NavMeshAgent.remainingDistance < 0.1f && !_enemyReferences.NavMeshAgent.pathPending;
        }
    }
}

