using FSM;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyState_Delay : IState
    {
        private float _waitForSeconds;
        private float deadline;
    
        public EnemyState_Delay(float waitForSeconds)
        {
            this._waitForSeconds = waitForSeconds;
        }
        public void OnEnter()
        {
            deadline = Time.time + _waitForSeconds;
        }
        public void Tick()
        {
            
        }
        public void OnExit()
        {
            
        }
    
        public Color GizmoState()
        {
            return Color.white;
        }
    
        public bool IsDone()
        {
            return Time.time >= deadline;
        }
        
    }
}

