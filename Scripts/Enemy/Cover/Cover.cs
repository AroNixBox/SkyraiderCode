using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool isOccupied;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
    }

    public void SetOccupiedStatus(bool status)
    {
        isOccupied = status;
    }

}
