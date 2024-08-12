using System;
using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    internal class EnemyState_AttackOpenField : IState
    {
        private EnemyReferences _enemyReferences;
        private StateMachine _stateMachine;
        
    
        public EnemyState_AttackOpenField(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
            
            _stateMachine = new StateMachine();
            
            var enemyShoot= new EnemyState_Shoot(_enemyReferences);
            var enemyDelay = new EnemyState_Delay(1f);
            var enemyReload = new EnemyState_Reload(_enemyReferences,"OpenFieldCovering");
            
            At(enemyShoot, enemyReload, () => _enemyReferences.Shooter.ShouldReload());
            At(enemyReload, enemyDelay, () => !_enemyReferences.Shooter.ShouldReload());
            At(enemyDelay, enemyShoot, () => enemyDelay.IsDone());
            
            _stateMachine.SetState(enemyShoot);
            
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condiiton) => _stateMachine.AddAnyTransition(to, condiiton);
        }
        public void OnEnter()
        {
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
            GameManager.Instance.RegisterCover(_enemyReferences.gameObject.name, _enemyReferences.EnemyHead.position);
            
            _enemyReferences.NavMeshAgent.isStopped = false;
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_OpenFieldAttack, true);
            _enemyReferences.NavMeshAgent.ResetPath();
        }
    
        public void Tick()
        {
            _stateMachine.Tick();
        }
    
        public void OnExit()
        {
            //Player now in Range!
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_OpenFieldAttack, false);
        }
    
        public Color GizmoState()
        {
            return Color.black;
        }
    }
}

