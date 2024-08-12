using System.Collections.Generic;
using Enemy.References;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.Monobehaviors
{
    public class EnemyVision : MonoBehaviour
    {
        public bool hasRecievedAlarm;
        private bool _isAlarm;
        private bool _isPlayerOutOfRange;
        
        private List<EnemyHealth> _enemiesAlive;
        public float alarmRadius = 15f;
        public float shootRange = 25f;
        [SerializeField] private float fieldOfViewAngle = 110f;
        [SerializeField] private LayerMask sightLayerMask;
        private EnemyReferences _enemyReferences;
        private AudioSource _audioSource;

        private float _checkDelay = 0.5f;
        private float _timeSinceLastCheck;
        private int _playerNotInSightCount;
        
        [SerializeField] private AudioClip[] canSeePlayerSounds;
        [SerializeField] private AudioClip[] canHearPlayerSounds;
    
        private void Awake()
        {
            _enemyReferences = GetComponent<EnemyReferences>();
        }
    
        private void Start()
        {
            _audioSource = _enemyReferences.AudioSource;
        }
    
        public bool HasRecievedAlarm()
        {
            return hasRecievedAlarm;
        }
        public enum AlarmState
        {
            BigAlarm,
            SmallAlarm
        }
        public void SetAlarm(AlarmState alarmState)
        {
            float thisEnemiesAlarmRadius = alarmRadius;
            switch(alarmState)
            {
                case AlarmState.BigAlarm:
                    thisEnemiesAlarmRadius = alarmRadius;
                    break;
                case AlarmState.SmallAlarm:
                    thisEnemiesAlarmRadius /= 2;
                    break;
            }
            
            _enemiesAlive = GameManager.Instance.GetEnemiesAlive();
            
            foreach (var enemy in _enemiesAlive)
            {
                if (enemy.transform != this.transform)
                {
                    float distanceToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
                    if (distanceToEnemy <= thisEnemiesAlarmRadius)
                    {
                        enemy.transform.GetComponent<EnemyVision>().hasRecievedAlarm = true;
                    }
                }
            }
            _isAlarm = true;
        }
    
        public void PlayAlarmSound(AlarmState alarmState)
        {
            float randomPitch = UnityEngine.Random.Range(.75f, .9f);
            _audioSource.pitch = randomPitch;
            switch(alarmState)
            {
                case AlarmState.BigAlarm:
                    _audioSource.PlayOneShot(canSeePlayerSounds[UnityEngine.Random.Range(0, canSeePlayerSounds.Length)], .5f);
                    break;
                case AlarmState.SmallAlarm:
                    _audioSource.PlayOneShot(canHearPlayerSounds[UnityEngine.Random.Range(0, canSeePlayerSounds.Length)], .5f);
                    break;
            }
        }   
        public void CallAlarmInRange(float range)
        {
            GameManager.Instance.AlarmEnemiesInRange(transform.position, range);
        }
    
        public bool IsAlarm()
        {
            return _isAlarm;
        }
        
        private void OnDrawGizmosSelected()
        {
            Vector3 enemyPos = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemyPos, alarmRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(enemyPos, shootRange);
            
            Gizmos.color = Color.green;
            float halfFOV = fieldOfViewAngle / 2.0f;
            Vector3 leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;
            Vector3 rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up) * transform.forward;
    
            Gizmos.DrawRay(enemyPos + Vector3.up * 1.65f, leftRayRotation * alarmRadius);
            Gizmos.DrawRay(enemyPos + Vector3.up * 1.65f, rightRayRotation * alarmRadius);
        }
        public Cover GetBetterCoverPosition(Covers covers, Vector3 smartPosition)
        {
            Cover nearestCover = covers.GetNearestAvailableCover(_enemyReferences.EnemyHead, _enemyReferences.PlayerHead, _enemyReferences.Vision.shootRange, _enemyReferences.gameObject.name, false);

            if (nearestCover != null)
            {
                float distanceToSmartPosition = Vector3.Distance(_enemyReferences.EnemyHead.position, smartPosition);
                float distanceToCover = Vector3.Distance(_enemyReferences.EnemyHead.position, nearestCover.transform.position);

                if (distanceToCover < distanceToSmartPosition)
                {
                    return nearestCover;
                }
            }
            return null;
        }


    
        public bool IsPlayerCloserThanCover(Covers covers, Transform playerHead, Transform enemyHead)
        {
            Cover nearestCover = covers.GetNearestAvailableCover(enemyHead, playerHead, shootRange, transform.name,false);
    
            if (nearestCover == null) return true;
            
            float distanceToPlayer = Vector3.Distance(transform.position, playerHead.position);
            
            float distanceToCover = Vector3.Distance(transform.position, nearestCover.transform.position);
            float distanceFromCoverToPlayer = Vector3.Distance(nearestCover.transform.position, playerHead.position);
            
            float costOfMovingToCover = distanceToCover;
            float benefitOfMovingToCover = distanceToPlayer - distanceFromCoverToPlayer;
      
            return benefitOfMovingToCover > costOfMovingToCover;
        }
    
        public bool IsPlayerInRangeAndInViewCone(Transform playerHead, Transform enemyHead)
        {
            Vector3 directionToPlayer = playerHead.position - enemyHead.position;
            float angleToPlayer = Vector3.Angle(enemyHead.forward, directionToPlayer);
    
            if (angleToPlayer <= fieldOfViewAngle * 0.5f)
            {
                float distanceToPlayer = directionToPlayer.magnitude;
                if (distanceToPlayer <= alarmRadius)
                {
                    RaycastHit hit;
                    Vector3 rayStartPos = enemyHead.position;
                    if (Physics.Raycast(rayStartPos, directionToPlayer.normalized, out hit, distanceToPlayer, sightLayerMask))
                    {
                        if (hit.collider.CompareTag(GlobalTags.Player))
                        {
                            Debug.Log("Player is in Range and In ViewCone: " + _isPlayerOutOfRange);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        public bool IsPlayerOutOfRangeOrCantBeeSeen(Transform playerHead, Transform enemyHead, bool shouldDoMultipleChecks)
        {
            float distanceToPlayer = Vector3.Distance(enemyHead.position, playerHead.position);

            // Wenn der Spieler außerhalb der Schussreichweite ist, zurückkehren.
            if (distanceToPlayer > shootRange)
            {
                _playerNotInSightCount = 0; // Zähler zurücksetzen
                return true;
            }

            // Zeit seit dem letzten Check aktualisieren
            _timeSinceLastCheck += Time.deltaTime;

            if (_timeSinceLastCheck >= _checkDelay)
            {
                _timeSinceLastCheck = 0f;

                // Überprüfen, ob der Spieler sichtbar ist
                if (CanSeePlayer(playerHead, enemyHead))
                {
                    _playerNotInSightCount = 0; // Zähler zurücksetzen, wenn der Spieler gesehen wird
                }
                else
                {
                    _playerNotInSightCount++; // Zähler erhöhen, wenn der Spieler nicht gesehen wird
                }
            }

            // Wenn 'shouldDoMultipleChecks' auf 'false' gesetzt ist, nur einmal prüfen
            if (!shouldDoMultipleChecks)
            {
                return _playerNotInSightCount > 0;
            }

            // 'True' zurückgeben, wenn der Spieler für mindestens 3 Checks in Folge unsichtbar war
            return _playerNotInSightCount >= 3;
        }


        private bool CanSeePlayer(Transform playerHead, Transform enemyHead)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (playerHead.position - enemyHead.position).normalized;
            float raycastStartOffset = 0.5f;
            Vector3 raycastStartPosition = enemyHead.position + directionToPlayer * raycastStartOffset;

            if (Physics.Raycast(raycastStartPosition, directionToPlayer, out hit, shootRange - raycastStartOffset, sightLayerMask))
            {
                return hit.collider.CompareTag(GlobalTags.Player);
            }
            return false;
        }
    }
}
