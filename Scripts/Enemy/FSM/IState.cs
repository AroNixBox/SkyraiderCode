using UnityEngine;

namespace FSM
{
   public interface IState
   {
       void OnEnter();
       void Tick();
       void OnExit();
       
       Color GizmoState();
   } 
}
