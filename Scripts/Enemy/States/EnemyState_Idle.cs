using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_Idle : IState
    {
        private EnemyReferences _enemyReferences;

        public EnemyState_Idle(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
        public void OnEnter()
        {
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_Speed, 0f);
            _enemyReferences.NavMeshAgent.isStopped = true;
        }
    
        public void Tick()
        {
            
        }
    
        public void OnExit()
        {
            _enemyReferences.NavMeshAgent.isStopped = false;
        }
    
        public Color GizmoState()
        {
            return Color.green;
        }
    }
}
