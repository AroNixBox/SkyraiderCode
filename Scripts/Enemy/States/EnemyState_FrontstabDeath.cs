using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_FrontstabDeath : IState
    {
        private EnemyReferences _enemyReferences;
        private Vector3 _targetForward;
        private Vector3 _forceMoveToPosition;


        public EnemyState_FrontstabDeath(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
    
        public void OnEnter()
        {
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
            
            _targetForward = -_enemyReferences.Player.forward; 
            _enemyReferences.NavMeshAgent.isStopped = true;
            _enemyReferences.transform.forward = _targetForward;
    
            _forceMoveToPosition = _enemyReferences.Player.position + _targetForward * -1.05f;
    
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_Frontstab);
        }
    
        public void Tick()
        {
            _enemyReferences.transform.forward = Vector3.Lerp(_enemyReferences.transform.forward, _targetForward, Time.deltaTime * 10f);
            
            _enemyReferences.transform.position = Vector3.Lerp(_enemyReferences.transform.position, _forceMoveToPosition, Time.deltaTime * 10f);
        }
    
        public void OnExit()
        {
            //Never gets called, No transition from this
        }
    
        public Color GizmoState()
        {
            return Color.magenta;
        }
    }
}

