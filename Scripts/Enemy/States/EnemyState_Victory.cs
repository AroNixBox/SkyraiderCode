using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_Victory : IState
    {
        private EnemyReferences _enemyReferences;
        public EnemyState_Victory(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
        public void OnEnter()
        {
            if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
            {
                GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
            }
            
            _enemyReferences.NavMeshAgent.ResetPath();
            //TODO: DO this with animation override controller
            string[] victoryAnimations = { "Victory1", "Victory2" };
            string randomAnimation = victoryAnimations[Random.Range(0, victoryAnimations.Length)];
            _enemyReferences.Animator.SetTrigger(randomAnimation);
            _enemyReferences.Health.OnTrashTalk();
        }
    
        public void Tick()
        {
            
        }
    
        public void OnExit()
        {
           
        }
    
        public Color GizmoState()
        {
            return Color.green;
        }
    }
}
