using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtractionZone : MonoBehaviour
{
    public bool isInTutorial;
    [SerializeField] private SceneField mainMenuScene;
    [SerializeField] private Slider extractionProgressSlider;
    [SerializeField] private float requiredTime = 25f;
    private float _currentTime;
    private bool _isPlayerInZone;
    private bool _isObjectiveComplete;
    private bool _isPlayerDead;
    
    private void Start()
    {
        GameManager.Instance.OnObjectiveComplete += OnObjectiveComplete;
        GameManager.Instance.OnPlayerDied += OnPlayerDied;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnObjectiveComplete -= OnObjectiveComplete;
        GameManager.Instance.OnPlayerDied -= OnPlayerDied;

    }


    private void OnObjectiveComplete()
    {
        _isObjectiveComplete = true;
    }
    private void OnPlayerDied()
    {
        _isPlayerDead = true;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GlobalTags.Player))
        {
            extractionProgressSlider.gameObject.SetActive(true);
            _isPlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GlobalTags.Player))
        {
            extractionProgressSlider.gameObject.SetActive(false);
            _isPlayerInZone = false;
        }
    }


    private void Update()
    {
        if ((_isPlayerInZone && !_isPlayerDead) && (isInTutorial || _isObjectiveComplete))
        {
            _currentTime += Time.deltaTime;
            float progressPercentage = Mathf.Clamp((_currentTime / requiredTime) * 100, 0, 100);
            extractionProgressSlider.value = progressPercentage;

            if (progressPercentage >= 100)
            {
                SceneManager.LoadScene(mainMenuScene.SceneName);
            }
        }
    }

}