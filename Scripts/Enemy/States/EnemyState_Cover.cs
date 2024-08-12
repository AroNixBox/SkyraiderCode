using System;
using Enemy.References;
using FSM;
using UnityEngine;

namespace Enemy.States
{
   public class EnemyState_Cover : IState
   {
       private EnemyReferences _enemyReferences;
       private StateMachine _stateMachine;
       Covers _covers;


       public EnemyState_Cover(EnemyReferences enemyReferences, Covers coverArea)
       {
           this._enemyReferences = enemyReferences;
           _covers = coverArea;
   
           _stateMachine = new StateMachine();
           
           var hidebehindcover = new EnemyState_HideBehindCover(_enemyReferences);
           var enemyShoot= new EnemyState_Shoot(enemyReferences);
           var enemyDelay = new EnemyState_Delay(1f);
           var enemyReload = new EnemyState_Reload(enemyReferences, "Cover");
           
           At(hidebehindcover, enemyShoot, () => hidebehindcover.IsDone());
           At(enemyShoot, enemyReload, () => enemyReferences.Shooter.ShouldReload());
           At(enemyReload, enemyDelay, () => !enemyReferences.Shooter.ShouldReload());
           At(enemyDelay, enemyShoot, () => enemyDelay.IsDone());
           
           _stateMachine.SetState(hidebehindcover);
           
           void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
           void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
           
       }
   
       public void Tick()
       {
           _stateMachine.Tick();
       }
   
       public void OnEnter()
       {
           _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Combat, true);
       }
   
       public void OnExit()
       {
           _covers.LeaveCover(_enemyReferences.transform.name);
           _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Combat, false);
       }
   
       public Color GizmoState()
       {
           return Color.black;
       }
   } 
}

