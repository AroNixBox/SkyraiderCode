using Enemy.Monobehaviors;
using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_AlarmRecieved : IState
    {
        private EnemyReferences _enemyReferences;
        public EnemyState_AlarmRecieved(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
        public void OnEnter()
        {
            _enemyReferences.Vision.PlayAlarmSound(EnemyVision.AlarmState.SmallAlarm);
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_AlarmRecieved);
        }
    
        public void Tick()
        {
    
        }
    
        public void OnExit()
        {
            
        }
    
        public Color GizmoState()
        {
            return Color.red;
        }
    }
}

