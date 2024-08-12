using Enemy.References;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
namespace Enemy.Monobehaviors
{
    public class EnemyShooter : MonoBehaviour
    {
        [Header("General")]
        public Transform shootPoint;
        public Transform gunPoint;
        public LayerMask layerMask;
        private EnemyReferences _enemyReferences;
        
        [Header("Gun")]
        public Vector3 spread;
        public int maxAmmo = 30;
        private int _currentAmmo;
        [SerializeField] private AudioClip[] enemyShotSounds;
        
        private void Awake()
        {
            _enemyReferences = GetComponent<EnemyReferences>();
            Reload();
        }
    
        public void Shoot()
        {
            if (ShouldReload())
                return;
            Vector3 direction = GetDirection();
            if(Physics.Raycast(shootPoint.position, direction * 10f, out RaycastHit hit, float.MaxValue, layerMask, QueryTriggerInteraction.Collide))
            {
                Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 1f);
                // TODO: Pool
                Vector3 spawnPosition = shootPoint.position + direction * 10f;
                Vector3 directionVector = spawnPosition - shootPoint.position;
                Quaternion rotation = Quaternion.LookRotation(directionVector);
                VFXManager.Instance.SpawnParticle(ParticleType.BulletShot, shootPoint.position, rotation);
                
                AudioClip randomSelectedShotSound = enemyShotSounds[Random.Range(0, enemyShotSounds.Length - 1)];
                AudioSource.PlayClipAtPoint(randomSelectedShotSound, shootPoint.position, .2f);
                _enemyReferences.Vision.CallAlarmInRange(_enemyReferences.Vision.shootRange / 8f);
                _currentAmmo -= 1;
                if (hit.transform.TryGetComponent(out Bodypart bodypart))
                {
                    switch (bodypart.hitboxType)
                    {
                        case Bodypart.HitboxType.Head:
                            //TODO Add VFX?
                            bodypart.Damage(25f, hit.point, hit.transform.forward);
                            break;
                        case Bodypart.HitboxType.Body:
                            //TODO Add VFX?
                            bodypart.Damage(10f, hit.point, hit.transform.forward);
                            break;
                    }   
                }
            }
        }
        
    
        public bool ShouldReload()
        {
            return _currentAmmo <= 0;
        }
        
        //Called from AnimEvent
        public void Reload()
        {
            _currentAmmo = maxAmmo;
        }
    
        private Vector3 GetDirection()
        {
            Vector3 direction = (_enemyReferences.PlayerHead.position - gunPoint.position).normalized;
            direction += new Vector3(
                Random.Range(-spread.x, spread.x),
                Random.Range(-spread.y, spread.y),
                Random.Range(-spread.z, spread.z)
            );
            direction.Normalize();
            return direction;
        }
    }
}

