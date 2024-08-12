using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
   public class PlayerAnimator : MonoBehaviour
   {
       [Header("Gun Properties")]
       [SerializeField] private Rig aimRig;
   
       [Header("Sword Properties")]
       [SerializeField] private Transform sword;
       [SerializeField] private Transform bodyAssignedSword;
   
       private PlayerReferences _playerReferences;
       private Animator _animator;
       private PlayerUI _playerUI;
   
       #region Animation Hashes
       
       public bool IsMeeleAttacking { get; set; }
       
       #endregion
   
       private void Awake()
       {
           _playerReferences = GetComponent<PlayerReferences>();
       }
   
       private void Start()
       {
           _playerUI = _playerReferences.PlayerUI;
           _animator = _playerReferences.Animator;
           aimRig.weight = 1f;
       }
       private void ResetMeeleAnimatorStates()
       {
           _animator.SetBool(GlobalAnimationHashes.PlayerAnim_EquipMeele, false);
           _animator.SetBool(GlobalAnimationHashes.PlayerAnim_UnequipMeele, false);
       }
       #region Meele Weapon
   
       public void StartEquipMeele()
       {
           aimRig.weight = 0f;
           _animator.SetBool(GlobalAnimationHashes.PlayerAnim_EquipMeele, true);
       }
   
       public void StartUnEquipMeele()
       {
           StartCoroutine(LerpLayerWeight(1, 1, 0.05f));
           _animator.SetBool(GlobalAnimationHashes.PlayerAnim_UnequipMeele, true);
       }
       
       public void OnMeeleEquipEnded()
       {
           ResetMeeleAnimatorStates();
           _animator.SetLayerWeight(1, 0f);
           //Can Attack here
       }
       public void OnMeeleUnequipEnded()
       {
           StartCoroutine(LerpLayerWeight(1, 1, .2f));
           ResetMeeleAnimatorStates();
           IsMeeleAttacking = false;
           //Enable Equip Rifle Animation?
       }
       public enum SwordEnabled
       {
           True,
           False
       }
       //Switch between Knife and BodyKnife
       public void OnSwordPullEvent(SwordEnabled swordEnabled)
       {
           switch (swordEnabled)
           {
               case SwordEnabled.True:
                   bodyAssignedSword.gameObject.SetActive(false);   
                   sword.gameObject.SetActive(true);
                   //TODO Publish event instead of reference
                   _playerUI.SetActiveWeapon(PlayerUI.WeaponType.Knife);
   
                   break;
               
               case SwordEnabled.False:
                   sword.gameObject.SetActive(false);
                   bodyAssignedSword.gameObject.SetActive(true);
                   _playerUI.SetActiveWeapon(PlayerUI.WeaponType.Rifle);
                   break;
           }
       }
       #endregion
   
       public void StartReload()
       {
           //THis isnt a layer, this is the real RIG for the aiming direction
           aimRig.weight = 0f;
           _animator.SetBool(GlobalAnimationHashes.PlayerAnim_Reload, true);
       }
   
       private IEnumerator LerpLayerWeight(int layer, float targetWeight, float duration)
       {
           float time = 0;
           float startWeight = _animator.GetLayerWeight(layer);
   
           while (time < duration)
           {
               _animator.SetLayerWeight(layer, Mathf.Lerp(startWeight, targetWeight, time / duration));
               time += Time.deltaTime;
               yield return null;
           }
   
           _animator.SetLayerWeight(layer, targetWeight);
       }
       #region Gun Weapon
       public void OnPlayerExcuting()
       {
           _animator.ResetTrigger(GlobalAnimationHashes.PlayerAnim_Backstab);
           _animator.ResetTrigger(GlobalAnimationHashes.PlayerAnim_Frontstab);
           //Play Particles from killing Enemy here?
           //Sound here?
       }
       
       public void SetAimRig(bool enabled)
       {
           float targetWeight = enabled ? 20f : 0f;
           aimRig.weight = Mathf.Lerp(aimRig.weight, targetWeight, Time.deltaTime * 10f);
       }
       
       #endregion
   } 
}

