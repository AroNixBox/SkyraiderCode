using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.References;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy.States
{
    public class EnemyState_Patrol : IState
    {
        private EnemyReferences _enemyReferences;
        private NavMeshAgent _navMeshAgent;
        private float _patrolRadius = 10f;
        private float _waitTime = 5f;
        private bool _isWaiting;
        private float initSpeed;
        private float patrolSpeed = 1.2f;
        
    
        private List<Vector3> _patrolPoints = new List<Vector3>();
        private int _currentPatrolIndex = 0;



        public EnemyState_Patrol(EnemyReferences enemyReferences)
        {
            _enemyReferences = enemyReferences;
            _navMeshAgent = _enemyReferences.NavMeshAgent;
        }
    
        //TODO Set the EnemyAnim_Patrol points by hand.
        public void OnEnter()
        {
            _navMeshAgent.isStopped = false;
            GeneratePatrolPoints();
            MoveToNextPatrolPoint();
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Patrol, true);
            initSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = patrolSpeed;
        }
    
        public void Tick()
        {
            if (!_navMeshAgent.pathPending && !_isWaiting && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                _enemyReferences.StartCoroutine(WaitAndMove());
            }
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_PatrolSpeed, _navMeshAgent.desiredVelocity.sqrMagnitude);
        }
    
        public void OnExit()
        {
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Patrol, false);
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_PatrolSpeed, 0);
            //_navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _navMeshAgent.speed = initSpeed;
        }
    
        public Color GizmoState()
        {
            return Color.green;
        }
    
        private void GeneratePatrolPoints()
        {
            _patrolPoints.Clear();
            if (_enemyReferences.PatrolPoints.Length > 0)
            {
                //Take Manual Position
                foreach (var patrolPoint in _enemyReferences.PatrolPoints)
                {
                    _patrolPoints.Add(patrolPoint.position);
                }
            }
            else
            {
                //Else Generate Random Points
                for (int i = 0; i < 3; i++)
                {
                    var randomDirection = Random.insideUnitSphere * _patrolRadius;
                    randomDirection += _enemyReferences.transform.position;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomDirection, out hit, _patrolRadius, 1))
                    {
                        _patrolPoints.Add(hit.position);
                    }
                }
            }
            

        }
    
        private void MoveToNextPatrolPoint()
        {
            if (_patrolPoints.Count == 0) return;
            if(_navMeshAgent.isOnNavMesh)
                _navMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex]);
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
        }
    
        private IEnumerator WaitAndMove()
        {
            _isWaiting = true;
            yield return new WaitForSeconds(_waitTime);
    
            Vector3 directionToNextPoint = (_patrolPoints[_currentPatrolIndex] - _enemyReferences.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToNextPoint);
            float angle = Vector3.SignedAngle(_enemyReferences.transform.forward, directionToNextPoint, Vector3.up);
    
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Turn, true);
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_TurnSpeed, Mathf.Clamp(angle / 180f * 8f, -1f, 1f));
    
            float rotationSpeed = 4f;
            float forceCorrectionTime = 3f;
            float elapsedTime = 0f;
    
            while (Quaternion.Angle(_enemyReferences.transform.rotation, targetRotation) > 0.5f)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= forceCorrectionTime)
                {
                    //Debug.LogWarning("Force correction applied to enemy rotation! Consider tweaking the parameters or checking for obstacles.");
                    break;
                }
    
                _enemyReferences.transform.rotation = Quaternion.RotateTowards(_enemyReferences.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 30);
                yield return null;
            }
            _enemyReferences.Animator.SetFloat(GlobalAnimationHashes.EnemyAnim_TurnSpeed, 0f);
            _enemyReferences.Animator.SetBool(GlobalAnimationHashes.EnemyAnim_Turn, false);
    
            _isWaiting = false;
            MoveToNextPatrolPoint();
        }
    
    }

}
