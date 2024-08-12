using UnityEngine;

public class AudioListenerFollow : MonoBehaviour
{
    public Transform cameraTransform; // Verweis auf das Transform der Kamera
    public Transform playerTransform; // Verweis auf das Transform des Spielers

    void Update()
    {
        transform.position = playerTransform.position;

        transform.forward = cameraTransform.forward;
    }
}