using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10.0f;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }
}