using UnityEngine;

public class ElectricityPathFollower : MonoBehaviour
{
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float speed = 5.0f;
    private int currentPointIndex = 0;

    void Update()
    {
        if (pathPoints.Length == 0) return;

        Transform currentPoint = pathPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, currentPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentPoint.position) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
            {
                currentPointIndex = 0;
            }
        }
    }
}
