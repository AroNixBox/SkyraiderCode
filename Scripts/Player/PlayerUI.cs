using System;
using System.Runtime.CompilerServices;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
   public class PlayerUI : MonoBehaviour
   {
       [SerializeField] private Slider healthSlider;
       [SerializeField] private TextMeshProUGUI stabHintText;
       [SerializeField] private TextMeshProUGUI currentAmmoText;
       [SerializeField] private TextMeshProUGUI totalAmmoText;

       [SerializeField] private AudioClip newHintSfx;
       [SerializeField] private AudioClip objectiveCompleteSfx;
       
       [SerializeField] private Animator playerCanvasAnimator;
       private AudioSource _audioSource;
       private string ammoToAdd;

       [SerializeField] private bool isInTutorial;

       [Header("Prompts")] 
       [SerializeField] private TextMeshProUGUI hintTMP;
       [SerializeField] private string objectiveGoal;
       [SerializeField] private string hintGoal;
       
       [SerializeField] private Animator hintAnimator;

   
       public enum WeaponType
       {
           Rifle,
           Knife
       }
   
       private void Awake()
       {
           _audioSource = GetComponent<AudioSource>();

       }

       private void Start()
       {
           //DIRTY, NOT GOOD, WAS STRESSFUL LOLXD

           #region DONT LOOK

                if (isInTutorial) return;

           #endregion

           
           GameManager.Instance.OnObjectiveComplete += ShowObjectiveHint;
           TimerUtility.SetTimer(this, ShowInitialHint, 5.0f);
       }

       private void OnDisable()
       {
           GameManager.Instance.OnObjectiveComplete -= ShowObjectiveHint;
       }

       public void UpdateAmmoText(int currentAmmo, int totalAmmo)
       {
           currentAmmoText.text = currentAmmo.ToString();
           totalAmmoText.text = totalAmmo.ToString();
       }
       public void SetActiveWeapon(WeaponType activeWeapon)
       {
           switch (activeWeapon)
           {
               case WeaponType.Knife:
                   playerCanvasAnimator.SetTrigger(GlobalAnimationHashes.UI_SwordEquip);
                   break;
               
               case WeaponType.Rifle:
                   Debug.Log(playerCanvasAnimator);
                   playerCanvasAnimator.SetTrigger(GlobalAnimationHashes.UI_MainWeaponEquip);
                   break;
           }
       }
   
       public void UpdatePlayerHitpoints(float currentHitPoints)
       {
           healthSlider.value = currentHitPoints;
       }
   
       public void ShowStabbingText(string hintText, bool enabled)
       {
           if (enabled)
           {
               stabHintText.gameObject.SetActive(true);
               stabHintText.text = hintText;
           }
           else
           {
               stabHintText.gameObject.SetActive(false);
           }
       }
       private void ShowInitialHint()
       {
           hintTMP.text = objectiveGoal;
           hintAnimator.SetTrigger(GlobalAnimationHashes.UI_ShowHintAnim);
           _audioSource.PlayOneShot(newHintSfx);
       }
       void ShowObjectiveHint()
       {
           hintAnimator.SetTrigger(GlobalAnimationHashes.UI_HideHintAnim);
           _audioSource.PlayOneShot(objectiveCompleteSfx);

           TimerUtility.SetTimer(this, () =>
           {
               hintTMP.text = hintGoal;
               hintAnimator.SetTrigger(GlobalAnimationHashes.UI_ShowHintAnim);
               _audioSource.PlayOneShot(newHintSfx);
           }, 5.0f);
       }
   } 
}

