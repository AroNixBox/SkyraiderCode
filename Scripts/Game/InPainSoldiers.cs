using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InPainSoldiers : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] inPainSfx;
    
    private float _timeSinceLastPlay;
    private float delayBetweenPlays;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        SetRandomDelay();
    }
    private void Update()
    {
        _timeSinceLastPlay += Time.deltaTime;

        if (_timeSinceLastPlay >= delayBetweenPlays)
        {
            PlayRandomSound();
            _timeSinceLastPlay = 0f;
            SetRandomDelay();
        }
    }
    private void PlayRandomSound()
    {
        var index = UnityEngine.Random.Range(0, inPainSfx.Length);
        _audioSource.clip = inPainSfx[index];
        _audioSource.Play(); 
    }
    private void SetRandomDelay()
    {
        delayBetweenPlays = UnityEngine.Random.Range(7f, 12f);
    }
}
