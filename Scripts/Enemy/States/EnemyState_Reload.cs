using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_Reload : IState
    {
        private EnemyReferences _enemyReferences;
        private string _attackBlendTreeValue;
        
    
        public EnemyState_Reload(EnemyReferences enemyReferences, string attackBlendTreeValue)
        {
            _enemyReferences = enemyReferences;
            _attackBlendTreeValue = attackBlendTreeValue;
        }
        public void OnEnter()
        {
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_Reload);
        }
    
        public void Tick()
        {
            
        }
    
        public void OnExit()
        {
            _enemyReferences.NavMeshAgent.ResetPath();
            _enemyReferences.Animator.ResetTrigger(GlobalAnimationHashes.EnemyAnim_Reload);
        }
    
        public Color GizmoState()
        {
            return Color.cyan;
        }
    }
}

