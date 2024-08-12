using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_HideBehindCover : IState
    {
        private EnemyReferences _enemyReferences;
        private bool _isAnimationComplete;

    
        public EnemyState_HideBehindCover(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
        public void OnEnter()
        {
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_Delay);
            _enemyReferences.RequestEndOfAnimation("Stand To Cover", 0,() =>
            {
                _isAnimationComplete = true;
            });
        }
    
        public void Tick()
        {
            
        }
    
        public void OnExit()
        {
            
        }
        public Color GizmoState()
        {
            return Color.magenta;
        }
        public bool IsDone()
        {
            return _isAnimationComplete;
        }
    }
}

