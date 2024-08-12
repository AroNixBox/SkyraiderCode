using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_Shoot : IState
    {
        private EnemyReferences _enemyReferences;
        private Transform _target;
        
    
        public EnemyState_Shoot(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
            _target = _enemyReferences.Player;
        }
        
        public void OnEnter()
        {
            _enemyReferences.NavMeshAgent.ResetPath();
            _target = _enemyReferences.Player;
        }
        public void Tick()
        {
            Vector3 lookPos = _target.position - _enemyReferences.transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            _enemyReferences.transform.rotation = Quaternion.Slerp(_enemyReferences.transform.rotation, rotation, Time.deltaTime * 10f);
                
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Shoot, true); 
        }
    
        public void OnExit()
        {
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Shoot, false);
            _target = null;
        }
    
        public Color GizmoState()
        {
            return Color.red;
        }
    }
}

