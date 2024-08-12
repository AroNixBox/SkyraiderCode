using Enemy.References;
using FSM;
using UnityEngine;
namespace Enemy.States
{
   public class EnemyState_BackstabDeath : IState
   {
       private EnemyReferences _enemyReferences;
       private Vector3 _targetForward;
       private Vector3 _forceMoveToPosition;


       public EnemyState_BackstabDeath(EnemyReferences enemyReferences)
       {
           _enemyReferences = enemyReferences;
       }
       public void OnEnter()
       {
           if (GameManager.Instance.IsPositionOccupied(_enemyReferences.gameObject.name))
           {
               GameManager.Instance.ReleasePosition(_enemyReferences.gameObject.name);
           }
           //OnBackstab playerposition and enemyposition are getting normalized and then the enemy is moved to the playerposition
           _enemyReferences.NavMeshAgent.isStopped = true;
           _targetForward = (_enemyReferences.transform.position - _enemyReferences.Player.position).normalized;   
           _enemyReferences.transform.forward = _targetForward;
   
           _forceMoveToPosition = _enemyReferences.Player.position + _targetForward * .6f;
           
           _enemyReferences.Animator.SetTrigger(GlobalAnimationHashes.EnemyAnim_Backstab);
       }
   
       public void Tick()
       {
           // Lerp des Vorwärts-Vektors
           _enemyReferences.transform.forward = Vector3.Lerp(_enemyReferences.transform.forward, _targetForward, Time.deltaTime * 10f);
   
           // Lerp der Position
           _enemyReferences.transform.position = Vector3.Lerp(_enemyReferences.transform.position, _forceMoveToPosition, Time.deltaTime * 10f);
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
