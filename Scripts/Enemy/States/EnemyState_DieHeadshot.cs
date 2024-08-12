using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_DieHeadshot : IState
    {
        private EnemyReferences _enemyReferences;


        public EnemyState_DieHeadshot(EnemyReferences enemyReferences)
        {
             _enemyReferences = enemyReferences;    
        }
        public void OnEnter()
        {
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
            
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_Headshot);
            _enemyReferences.Animator.SetLayerWeight(1, 0f);
            _enemyReferences.NavMeshAgent.isStopped = true;
        }
    
        public void Tick()
        {
            
        }
    
        public void OnExit()
        {
            //TODO Needs to be called! Never Exits, Code Cleanup? Enemy dies here!
        }
    
        public Color GizmoState()
        {
            return Color.magenta;
        }
    }
}
