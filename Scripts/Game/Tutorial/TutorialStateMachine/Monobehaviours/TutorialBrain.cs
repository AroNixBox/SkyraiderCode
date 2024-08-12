using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Monobehaviors;
using Enemy.References;
using FIMSpace;
using FSM;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialBrain : MonoBehaviour
{
    private StateMachine _stateMachine;
    [SerializeField] private TutorialReferences _references;
    [SerializeField] private TriggerComponent[] triggerComponents;
    [SerializeField] private EnemyReferences[] enemyReferences;
    [SerializeField] private GameObject[] targets;
    private void Start()
    {
        _stateMachine = new StateMachine();
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);

        var walkAndJump = new TutorialState_Objective(_references,
            "WASD to Move, Shift to sprint and Spacebar to Jump", "FirstDoor", Color.green);
        var crouch =  new TutorialState_Objective2(_references, "Hold STRG to Crouch, Sneak past the Guards", "SecondDoor", Color.yellow);
        var backstabIdlingEnemy = new TutorialState_Objective3(_references,
            "Sneak behind the Guard and press LMB to Backstab", "ThirdDoor", Color.red);
        var backstabPatrollingEnemy = new TutorialState_Objective4(_references,
            "Hide and then backstab the amored guard!", "FourthDoor", Color.magenta);
        var shootingRange = new TutorialState_Objective5(_references,
            "Hold RMB to Aim and press LMB to Shoot, R to Reload", "FifthDoor", Color.blue);
        var extractionZone = new TutorialState_Objective6(_references,
            "Get to the Extraction Zone", "SixthDoor", Color.black);


        
        
        At(walkAndJump, crouch, () => IsPlayerInTrigger(0));
        At(crouch, backstabIdlingEnemy, () => IsPlayerInTrigger(1));
        At(backstabIdlingEnemy, backstabPatrollingEnemy, () => IsEnemyDead(0));
        At(backstabPatrollingEnemy, shootingRange, () => IsEnemyDead(1));
        At(shootingRange, extractionZone, IsAllTargetsDestroyed);
        
        
        _stateMachine.SetState(walkAndJump);
    }

    private bool IsEnemyDead(int index)
    {
        return enemyReferences[index].Health.GotExecuted(EnemyHealth.ExecutionState.Backstab)
               || enemyReferences[index].Health.GotExecuted(EnemyHealth.ExecutionState.Frontstab);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    private bool IsPlayerInTrigger(int index)
    {
        return triggerComponents[index].IsInTriggerZone;
    }

    private bool IsAllTargetsDestroyed()
    {
        foreach (var target in targets)
        {
            if (target.activeSelf)
            {
                return false;
            }
        }

        return true;
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
