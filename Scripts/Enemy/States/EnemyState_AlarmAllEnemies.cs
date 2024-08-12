using Enemy.Monobehaviors;
using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_AlarmAllEnemies : IState
    {
        private EnemyReferences _enemyReferences;
        public EnemyState_AlarmAllEnemies(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
        }
        public void OnEnter()
        {
            _enemyReferences.Vision.PlayAlarmSound(EnemyVision.AlarmState.BigAlarm);
            _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_AlarmAllEnemies);
            _enemyReferences.NavMeshAgent.ResetPath();
        }
    
        public void Tick()
        {
            Vector3 lookPos = _enemyReferences.Player.position - _enemyReferences.transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            _enemyReferences.transform.rotation = Quaternion.Slerp(_enemyReferences.transform.rotation, rotation, Time.deltaTime * 5f);
        }
    
        public void OnExit()
        {
            Debug.Log("Has alarmed EveryOne");
        }
    
        public Color GizmoState()
        {
            return Color.red;
        }
    }
}

