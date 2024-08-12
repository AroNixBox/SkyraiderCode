using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private SceneField firstLevel;
    [SerializeField] private SceneField tutorialLevel;
    [Header("Buttons")] [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    
    [Header("Loading Bar")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingProgressBar;
    [SerializeField] private float lagTimeamountLoadingBar = 0.5f;
    [SerializeField] private float minLoadingTimeMultiplier = 20f;

    private void Start()
    {
        playButton.onClick.AddListener(LoadGame);
        tutorialButton.onClick.AddListener(LoadTutorial);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void LoadGame()
    {
        StartCoroutine(LoadGameAsync(firstLevel));
        loadingScreen.SetActive(true);
    }

   private IEnumerator LoadGameAsync(SceneField sceneToLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad.SceneName);
        asyncLoad.allowSceneActivation = false;
    
        float startTime = Time.time;
    
        bool hasFirstStuckHappened = false;
        bool hasSecondStuckHappened = false;
    
        yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);
    
        float actualLoadTime = Time.time - startTime;
        float simulatedLoadTime = actualLoadTime * minLoadingTimeMultiplier;
    
        float progress = 0;
        float totalSimulatedTimePassed = 0;
        float durationOfLerp = 1f; 
        float lerpStartTime;
    
        while (totalSimulatedTimePassed < simulatedLoadTime)
        {
            totalSimulatedTimePassed += Time.deltaTime;
            float simulatedProgress = totalSimulatedTimePassed / simulatedLoadTime;
    
            if (simulatedProgress >= 0.2f && !hasFirstStuckHappened)
            {
                hasFirstStuckHappened = true;
                lerpStartTime = Time.time;
                float lerpEndValue = progress + 0.1f;
    
                while (Time.time < lerpStartTime + durationOfLerp)
                {
                    progress = Mathf.Lerp(progress, lerpEndValue, (Time.time - lerpStartTime) / durationOfLerp);
                    loadingProgressBar.value = progress;
                    yield return null;
                }
    
                yield return new WaitForSeconds(lagTimeamountLoadingBar / 2);
            }
            else if (simulatedProgress >= 0.8f && !hasSecondStuckHappened)
            {
                hasSecondStuckHappened = true;
                lerpStartTime = Time.time;
                float lerpEndValue = progress + 0.1f;
    
                while (Time.time < lerpStartTime + durationOfLerp)
                {
                    progress = Mathf.Lerp(progress, lerpEndValue, (Time.time - lerpStartTime) / durationOfLerp);
                    loadingProgressBar.value = progress;
                    yield return null;
                }
    
                yield return new WaitForSeconds(lagTimeamountLoadingBar);
            }
    
            if (simulatedProgress > progress)
            {
                progress = simulatedProgress;
                loadingProgressBar.value = progress;
            }
    
            yield return null;
        }
    
        loadingProgressBar.value = 1f;
        asyncLoad.allowSceneActivation = true;
    }
   

    private void LoadTutorial()
    {
        StartCoroutine(LoadGameAsync(tutorialLevel));
        loadingScreen.SetActive(true);
    }
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
