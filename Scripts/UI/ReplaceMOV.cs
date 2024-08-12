using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ReplaceMOV : MonoBehaviour
{
    private enum VideoState
    {
        Menu,
        StartGame,
        Quit,
        Options
    }

    [SerializeField] private VideoPlayer menuScreen;
    [SerializeField] private VideoClip menuVideoClip;

    [SerializeField] private Button startButton;
    [SerializeField] private VideoClip onStartGameClip;
    
    [SerializeField] private Button optionButton;
    [SerializeField] private VideoClip onOptionsGameClip;
    
    [SerializeField] private Button quitButton;
    [SerializeField] private VideoClip onQuitGameClip;
    
    private VideoState _currentVideoState = VideoState.Menu;
    
    private void Awake()
    {
        startButton.onClick.AddListener(() => SwapMov(onStartGameClip, VideoState.StartGame));
        optionButton.onClick.AddListener(() => SwapMov(onOptionsGameClip, VideoState.Options));
        quitButton.onClick.AddListener(() => SwapMov(onQuitGameClip, VideoState.Quit));
        menuScreen.loopPointReached += CheckAndLoadNextScene;
    }

    void SwapMov(VideoClip clipToPlay, VideoState state)
    {
        menuScreen.clip = clipToPlay;
        menuScreen.isLooping = false;
        menuScreen.Play();
        _currentVideoState = state;
    }

    void CheckAndLoadNextScene(VideoPlayer vp)
    {
        if (!menuScreen.isLooping)
        {
            switch (_currentVideoState)
            {
                case VideoState.StartGame:
                    LoadNextScene();
                    break;

                case VideoState.Quit:
                    QuitGame();
                    break;

                case VideoState.Options:
                    ShowOptionsMenu();
                    break;
            }
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowOptionsMenu()
    {
        Debug.Log(_currentVideoState);
    }

    void LoadNextScene()
    {

        SceneManager.LoadScene("Playground"); 
    }
    
}
