using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [RequireComponent(typeof(AudioSource))]
    public class Objective : MonoBehaviour
    {
        private bool _isTowerShutdown;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource beepAudioSource;

        [SerializeField] private Material normalMaterial;
        [SerializeField] private Transform objectiveParentTransform;
        
        [SerializeField] private Light objectiveLight;

        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip[] beepSfx;
        [SerializeField] private Transform machineTipTransform;
        [SerializeField] private AudioClip machineIdleSfx;
        [SerializeField] private AudioClip machineShutdownSfx;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            InitializeTowerSfx();
            StartCoroutine(SignalLoop());
        }


        private void InitializeTowerSfx()
        {
            audioSource.clip = machineIdleSfx;
            audioSource.loop = true;
            audioSource.Play();
        }
        private IEnumerator SignalLoop()
        {
            while (!_isTowerShutdown)
            {
                AudioClip clipToPlay = beepSfx[UnityEngine.Random.Range(0, beepSfx.Length)];
                float randomPitch = UnityEngine.Random.Range(.8f, .1f);
                beepAudioSource.pitch = randomPitch;
                beepAudioSource.PlayOneShot(clipToPlay);
                
                VFXManager.Instance.SpawnParticleAndBindToParent(ParticleType.ObjectiveFlame, machineTipTransform);
                
                yield return new WaitForSeconds(8f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.CompareTag(GlobalTags.Player) && !_isTowerShutdown)
            {
                _isTowerShutdown = true;
                TowerShutdown();
            }
        }

        private void TowerShutdown()
        {
            audioSource.clip = machineShutdownSfx;
            audioSource.loop = false;
            audioSource.Play();
            animator.SetTrigger(GlobalAnimationHashes.Objective_TurnOff);

            while (objectiveLight.intensity > 0)
            {
                objectiveLight.intensity -= Time.deltaTime;
            }

            objectiveLight.enabled = false;
            
            //Reset Material outline by replacing the material
            ChangeChildrenMaterials(objectiveParentTransform);
        
            GameManager.Instance.ObjectiveComplete();
        }
        
        private void ChangeChildrenMaterials(Transform parent)
        {
            if(objectiveParentTransform.TryGetComponent(out MeshRenderer rend))
            {
                rend.material = normalMaterial;
                
                foreach (Transform child in parent)
                {
                    var childRenderer = child.GetComponent<MeshRenderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.material = normalMaterial;
                    }

                    // Rekursiver Aufruf für Unterkinder
                    if (child.childCount > 0)
                    {
                        ChangeChildrenMaterials(child);
                    }
                }
            }

        }
    }
}
