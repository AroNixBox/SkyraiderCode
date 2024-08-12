using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audio2D;
    [SerializeField] private AudioSource audio3D;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Play2DSound(AudioClip clip, [UnityEngine.Internal.DefaultValue("1.0F")] float volumeScale)
    {
        audio2D.PlayOneShot(clip, volumeScale);
    }

    public void Play3DSound(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volumeScale)
    {
        //audio3D.transform.position = position;
        //audio3D.PlayOneShot(clip, volumeScale);
        AudioSource.PlayClipAtPoint(clip, position, volumeScale);
    }
}
