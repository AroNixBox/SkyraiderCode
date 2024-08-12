using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace UI
{
    public class ButtonBehavior : MonoBehaviour
    {
        private enum ButtonName
        {
            Start,
            Gear,
            Bomb,
            Options,
            Quit
        }
        [Header("ButtonProperties")]
        [SerializeField] private ButtonName buttonName;
        [SerializeField] private AudioClip onButtonClick;
        [SerializeField] private AudioClip onButtonRelease;
        [SerializeField] private AudioSource buttonAudioSource;

        [Header("EffectProperties")]
        [SerializeField] private ParticleSystem[] boomParticles;
        [SerializeField] private AudioClip boomSound;
        [SerializeField] private Vector3 shakeImpulse;

        private readonly Dictionary<ParticleSystem, CinemachineImpulseSource> _impulseDict =
            new Dictionary<ParticleSystem, CinemachineImpulseSource>();

        private float _audioSourceBasePitch;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            _audioSourceBasePitch.Equals(buttonAudioSource.pitch);
            foreach (var boom in boomParticles)
            {
                _impulseDict.Add(boom, boom.GetComponent<CinemachineImpulseSource>());
            }
        }

        public void OnButtonClick()
        {
            buttonAudioSource.PlayOneShot(onButtonClick);
        }

        private void PlayGame()
        {
            SceneManager.LoadScene("Playground");
        }

        private void DoGear()
        {
            Debug.Log("Gear");
        }

        private void DoBomb()
        {
            StartCoroutine(Boom());
        }

        private void Options()
        {
            Debug.Log("Options");
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        IEnumerator Boom()
        {
            foreach (var boom in boomParticles)
            {
                boom.Play();
                buttonAudioSource.pitch = UnityEngine.Random.Range(0.6f, 1.3f);
                buttonAudioSource.PlayOneShot(boomSound);
                _impulseDict[boom].GenerateImpulseAtPositionWithVelocity(boom.transform.position, shakeImpulse);
                yield return new WaitForSeconds(0.5f);
            }
            buttonAudioSource.pitch = _audioSourceBasePitch;
            QuitGame();
        }


        public void OnButtonRelease()
        {
            buttonAudioSource.PlayOneShot(onButtonRelease);
            switch (buttonName)
            {
                case ButtonName.Start:
                    PlayGame();
                    break;
            
                case ButtonName.Options:
                    Options();
                    break;
            
                case ButtonName.Gear:
                    DoGear();
                    break;
            
                case ButtonName.Bomb:
                    DoBomb();
                    break;
            
                case ButtonName.Quit:
                    QuitGame();
                    break;
            }
        }
    }
}
