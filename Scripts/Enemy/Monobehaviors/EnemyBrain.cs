using System;
using Enemy.References;
using Enemy.States;
using FSM;
using UnityEngine;

namespace Enemy.Monobehaviors
{
    public class EnemyBrain : MonoBehaviour
    {
        private enum EnemyType
        {
            IdlingEnemy,
            SittingEnemy,
            PatrolingEnemy,
        }
    
        [SerializeField] private EnemyType enemyType;
        
        private EnemyReferences _enemyReferences;
        private StateMachine _stateMachine;
        private Cover currentCover;
    
        private void Start()
        {
            _enemyReferences = GetComponent<EnemyReferences>();
            _stateMachine = new StateMachine();
            
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
    
            Covers coverArea = FindObjectOfType<Covers>();
            
            //Entry States
            var idle = new EnemyState_Idle(_enemyReferences);
            var idleSitting = new EnemyState_SitIdle(_enemyReferences);
            var patroling = new EnemyState_Patrol(_enemyReferences); 
            //Other States
            var alarmAllEnemies = new EnemyState_AlarmAllEnemies(_enemyReferences);
            var alarmRecieved = new EnemyState_AlarmRecieved(_enemyReferences);
            var runToCover = new EnemyState_RunToCover(_enemyReferences, coverArea);
            var attackOpenField = new EnemyState_AttackOpenField(_enemyReferences);
            var cover = new EnemyState_Cover(_enemyReferences, coverArea);
            
            var frontstabDeath = new EnemyState_FrontstabDeath(_enemyReferences);
            var backstabDeath = new EnemyState_BackstabDeath(_enemyReferences);
            var headhShotDeath = new EnemyState_DieHeadshot(_enemyReferences);
            var victory = new EnemyState_Victory(_enemyReferences); 
    
            var smartEnemyPos = new EnemyState_RunToSmartPosition(_enemyReferences);
            
            //Is the enemy alarmed and NOT CLOSER to a cover?
            At(alarmAllEnemies, attackOpenField,() => _enemyReferences.Vision.IsAlarm() && 
                                                      _enemyReferences.Vision.IsPlayerCloserThanCover(coverArea, _enemyReferences.PlayerHead, _enemyReferences.EnemyHead)&&
                                                      !_enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, false));
            At(alarmRecieved, attackOpenField, () => _enemyReferences.Vision.IsAlarm() && 
                                                     _enemyReferences.Vision.IsPlayerCloserThanCover(coverArea, _enemyReferences.PlayerHead, _enemyReferences.EnemyHead) &&
                                                     !_enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, false));
            
            At(alarmAllEnemies, smartEnemyPos, () => _enemyReferences.Vision.IsAlarm() && 
                                                     _enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, false));
            At(alarmRecieved, smartEnemyPos, () => _enemyReferences.Vision.IsAlarm() && 
                                                   _enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, false));
            
            //Is the enemy alarmed and CLOSER to a cover?
            At(alarmAllEnemies, runToCover, () => _enemyReferences.Vision.IsAlarm() && 
                                                  !_enemyReferences.Vision.IsPlayerCloserThanCover(coverArea, _enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
            At(alarmRecieved, runToCover, () => _enemyReferences.Vision.IsAlarm() && 
                                                !_enemyReferences.Vision.IsPlayerCloserThanCover(coverArea, _enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
            
            //After enemy is alerted, I let them shoot once. this is fine for now, because they will make the checks aftere shooting and will run to a position right after.
            //Enemy goes into cover state machine after reaching cover
            At(runToCover, cover, () => runToCover.HasArrivedAtDestination());
            At(cover, smartEnemyPos, () => _enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, true));
            
            At(attackOpenField, smartEnemyPos, () => _enemyReferences.Vision.IsPlayerOutOfRangeOrCantBeeSeen(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead, true));
            At(attackOpenField, runToCover, () => !_enemyReferences.Vision.IsPlayerCloserThanCover(coverArea, _enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
            

            //Check if cover can see player from cover position!
            At(smartEnemyPos, runToCover, () => smartEnemyPos.CoverCloserThanPlayer);
            At(smartEnemyPos, attackOpenField, () => smartEnemyPos.HasArrivedAtDestination() && !smartEnemyPos.CoverCloserThanPlayer);
            
            //Enemy gets Backstabbed!
            Any(frontstabDeath, () => _enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Frontstab));
            Any(backstabDeath, () => _enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Backstab));
            Any(headhShotDeath, () => _enemyReferences.Health.GotShot());
            Any(victory, () => _enemyReferences.IsPlayerDead && 
                               !_enemyReferences.Health.GotShot() && 
                               !_enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Backstab) 
                               && !_enemyReferences.Health.GotExecuted(EnemyHealth.ExecutionState.Frontstab) 
                               && _enemyReferences.Vision.IsAlarm());
    
            switch (enemyType)
            {
                case EnemyType.IdlingEnemy:
                    At(idle, alarmAllEnemies, () => _enemyReferences.Vision.IsPlayerInRangeAndInViewCone(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
                    At(idle, alarmRecieved, () => _enemyReferences.Vision.HasRecievedAlarm());
                    _stateMachine.SetState(idle);
                    break;
                
                case EnemyType.SittingEnemy:
                    At(idleSitting, alarmAllEnemies, () => _enemyReferences.Vision.IsPlayerInRangeAndInViewCone(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
                    At(idleSitting, alarmRecieved, () => _enemyReferences.Vision.HasRecievedAlarm());
                    _stateMachine.SetState(idleSitting);
                    break;
                
                case EnemyType.PatrolingEnemy:
                    At(patroling, alarmAllEnemies, () => _enemyReferences.Vision.IsPlayerInRangeAndInViewCone(_enemyReferences.PlayerHead, _enemyReferences.EnemyHead));
                    At(patroling, alarmRecieved, () => _enemyReferences.Vision.HasRecievedAlarm());
                    _stateMachine.SetState(patroling);
                    break;
            }
            
        }
    
        private void Update()
        {
            _stateMachine.Tick();
        }
    
        private void OnDrawGizmos()
        { 
            if (_stateMachine != null)
            {
                Gizmos.color = _stateMachine.GetGizmoColor();
                Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
            }
        }
    }
}
