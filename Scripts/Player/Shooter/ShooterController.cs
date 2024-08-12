using System;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UniRx;
using UnityEngine.Serialization;

namespace Player
{
    public class ShooterController : MonoBehaviour
    {
        #region Fields and References    
        
        private readonly Subject<Unit> _onShoot = new Subject<Unit>();
        public IObservable<Unit> OnShootAsObservable => _onShoot;

        [Header("WeaponProperties")] 
        [SerializeField] private Transform spawnBulletPosition;
    
        [SerializeField] private float weaponSoundAlertRadius = 10f;
        [SerializeField] private float headShotDamage;
        [SerializeField] private float bodyShotDamage;
    
        [SerializeField] private WeaponRecoil weaponRecoil;
    
        [Header("AimProperties")] 
        [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
        [SerializeField] private Animator pickedUpAmmoAnimator;
    
        [SerializeField] private float normalSensitivity;
        [SerializeField] private float aimSensitivity;
        [SerializeField] private LayerMask aimColliderMask;
        [SerializeField] private Transform gunOrientationOnCrosshairTransform;
        
        [Header("AmmoProperties")]
        [SerializeField] private int maxAmmo = 100;
        [SerializeField] private int ammoPerMagazine = 10;
        private int _currentAmmo;
        private int _totalAmmo;
        [HideInInspector] public bool isReloading;
        
        private PlayerReferences _playerReferences;
        private StarterAssetsInputs _starterAssetsInputs;
        private ThirdPersonController _thirdPersonController;
        private PlayerAnimator _playerAnimator;
        private Animator _animator;
        private PlayerUI _playerUI;
        
        private bool _wasButtonPressed;
        private bool _wasMeeleButtonPressed;
        
    
        [SerializeField] private float minTimeBetweenShots = 0.1f;
        private float _lastShotTime;
        private bool _isAiming;
        

        private Camera _camera;
        

        #endregion    
        
        #region Initialization    
    
        private void Awake()
        {
            _playerReferences = GetComponent<PlayerReferences>();
        }
    
        private void Start()
        {
            _playerUI = _playerReferences.PlayerUI;
            _playerAnimator = _playerReferences.PlayerAnimator;
            _starterAssetsInputs = _playerReferences.StarterAssetsInputs;
            _thirdPersonController = _playerReferences.ThirdPersonController;
            _animator = _playerReferences.Animator;
            
            _camera = Camera.main;
            InitializeAmmo();
        }
    
        private void InitializeAmmo()
        {
            if (maxAmmo > 0)
            {
                _currentAmmo = ammoPerMagazine;
                _totalAmmo = maxAmmo - _currentAmmo;
            }
            else
            {
                ammoPerMagazine = 10;
                Debug.Log("Max ammo is 0");
            }
            _playerUI.UpdateAmmoText(_currentAmmo, _totalAmmo);  

            
        }
    
        #endregion    
    
        private void Update()
        {
            GunAimData aimData = UpdateGunOrientation();
            HandleShootingAndAiming(aimData);
        }
    
        #region Gun Orientation    
    
        private class GunAimData
        {
            public Vector3 MouseWorldPosition { get; set; }
            public Transform HitTransform { get; set; }
            public RaycastHit HitDetails { get; set; } 
        }
    
        private GunAimData UpdateGunOrientation()
        {
            GunAimData aimData = new GunAimData();
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = _camera.ScreenPointToRay(screenCenterPoint);
    
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
            {
                gunOrientationOnCrosshairTransform.position = Vector3.Lerp(gunOrientationOnCrosshairTransform.position,
                    raycastHit.point, Time.deltaTime * 20);
                aimData.MouseWorldPosition = raycastHit.point;
                aimData.HitTransform = raycastHit.transform;
                aimData.HitDetails = raycastHit;
            }
    
            return aimData;
        }
    
        #endregion    
    
        #region Shooting and Aiming    
        //Dont need this button, only when unequip begins, reload needs to be canceled
    
        private void HandleShootingAndAiming(GunAimData aimData)
        {
            if(_playerAnimator.IsMeeleAttacking)
                return;
            
            //TODO: Maybe Error here?
            if (isReloading)
            {
                ExitAimingMode();
                return;
            }
            
            HandleReloadButtonPress();
            
            if (_starterAssetsInputs.aim)
            {
                _isAiming = true;
                EnterAimingMode(aimData);
            }
            //TODO: Maybe Error here?
            else
            {
                _isAiming = false;
                ExitAimingMode();
            }
        }
        
        private void HandleReloadButtonPress()
        {
            if (_starterAssetsInputs.reload && _currentAmmo < ammoPerMagazine && _totalAmmo > 0)
            {
                isReloading = true;
                _playerAnimator.StartReload();
            }
    
        }
    
        private void EnterAimingMode(GunAimData aimData)
        {
            InitializeAimingVisuals();
            OrientTowardsAim(aimData);
            CheckForShooting(aimData);
        }
    
        private void InitializeAimingVisuals()
        {
            aimVirtualCamera.gameObject.SetActive(true);
            _thirdPersonController.SetSensitivity(aimSensitivity);
            _thirdPersonController.SetRotateOnMove(false);
            _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Aim, true);
            _playerAnimator.SetAimRig(true);
        }
    
        private void OrientTowardsAim(GunAimData aimData)
        {
            Vector3 worldAimTarget = aimData.MouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            //If I lerp this, the body will do a weird rotation
            transform.forward = aimDirection;
        }
    
        private void CheckForShooting(GunAimData aimData)
        {
            if (_starterAssetsInputs.shoot && Time.time - _lastShotTime >= minTimeBetweenShots)
            {
                if (_currentAmmo <= 0 && _totalAmmo > 0)
                {
                    isReloading = true;
                    _playerAnimator.StartReload();
                    return;
                }
                if (_currentAmmo > 0)
                {
                    ShootAtTarget(aimData);
                    _lastShotTime = Time.time;
                    _currentAmmo--;
                    _playerUI.UpdateAmmoText(_currentAmmo, _totalAmmo);
                }
            }
        }
    
        private void ShootAtTarget(GunAimData aimData)
        {
            if(aimData.HitTransform.TryGetComponent(out Bodypart bodypart))
            {
                switch (bodypart.hitboxType)
                {
                    case Bodypart.HitboxType.Head:
                        //TODO Add VFX?
                        bodypart.Damage(headShotDamage, aimData.HitDetails.point, aimData.HitDetails.transform.forward);
                        break;
                    case Bodypart.HitboxType.Body:
                        //TODO Add VFX?
                        bodypart.Damage(bodyShotDamage, aimData.HitDetails.point, aimData.HitDetails.transform.forward);
                        break;
                }   
            }
    
            if (VFXManager.Instance != null)
            {
                Vector3 direction = gunOrientationOnCrosshairTransform.position - spawnBulletPosition.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                VFXManager.Instance.SpawnParticle(ParticleType.BulletShot, spawnBulletPosition.position, rotation);
                VFXManager.Instance.SpawnParticle(ParticleType.RifleShotFlame, spawnBulletPosition.position, spawnBulletPosition.rotation);
                rotation = Quaternion.LookRotation(aimData.HitDetails.normal);
                VFXManager.Instance.SpawnParticle(ParticleType.BulletImpact, aimData.HitDetails.point, rotation);
            }
    
            GameManager.Instance.AlarmEnemiesInRange(this.transform.position, weaponSoundAlertRadius);
            weaponRecoil.TriggerRecoil();
            HandleBulletHits(aimData);
            
            // Event: playerSoundController.PlayRandomShootingSound();
            _onShoot.OnNext(Unit.Default);
        }
        
        //Animation Event
        public void ReloadAmmo()
        {
            int ammoNeeded = ammoPerMagazine - _currentAmmo;
            int ammoToTake = Mathf.Min(ammoNeeded, _totalAmmo);
    
            _currentAmmo += ammoToTake;
            _totalAmmo -= ammoToTake;
            
            ExitAimingMode();
            _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Reload, false);
            isReloading = false;
            _playerUI.UpdateAmmoText(_currentAmmo, _totalAmmo);
        }

        public void AddAmmo(int ammo)
        {
            pickedUpAmmoAnimator.SetTrigger(GlobalAnimationHashes.UI_PickupAmmo);
        }
        
        //Animation envent, so only one ammo is added per event 
        public void AddSingleAmmo()
        {
            _totalAmmo++;
            _playerUI.UpdateAmmoText(_currentAmmo, _totalAmmo);
        }
        private void OnReloadCanceled()
        {
            isReloading = false;
            _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Reload, false);
            ExitAimingMode();
        }
    
    
        private void HandleBulletHits(GunAimData aimData)
        {
            if (aimData.HitTransform != null)
            {
                ExecuteImpact(aimData);
            }
        }
    
        private void ExecuteImpact(GunAimData aimData)
        {
            if (aimData.HitTransform.TryGetComponent(out Fracture fracture))
            {
                fracture.CauseFracture();
                ApplyForceToNearbyObjects(aimData);
            }
            else if (aimData.HitTransform.TryGetComponent(out Slice slice))
            {
                Vector3 sliceNormalWorld = -aimData.HitTransform.forward;
                Vector3 sliceOriginWorld = aimData.HitDetails.point;
                slice.ComputeSlice(sliceNormalWorld, sliceOriginWorld);
                ApplyForceToNearbyObjects(aimData);
            }
    

        }
    
        private void ApplyForceToNearbyObjects(GunAimData aimData)
        {
            Vector3 sphereCastCenter = aimData.HitDetails.point;
            RaycastHit[] hits = Physics.SphereCastAll(sphereCastCenter, 2, Vector3.one, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 forceDirection = (hit.transform.position - sphereCastCenter).normalized;
                    rb.AddForce(forceDirection * 2, ForceMode.Impulse);
                }
            }
        }
    
        public void ExitAimingMode()
        {
            aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonController.SetSensitivity(normalSensitivity);
            _thirdPersonController.SetRotateOnMove(true);
            _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Aim, false);
            _playerAnimator.SetAimRig(false);
            _starterAssetsInputs.shoot = false;
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, weaponSoundAlertRadius);
        }
    
        #endregion   
    }
}

