using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Covers : MonoBehaviour
{
    private Cover[] covers;
    private Dictionary<string, Cover> CoverRegistry = new Dictionary<string, Cover>();

    private void Awake()
    {
        covers = GetComponentsInChildren<Cover>();
    }
    public Cover GetNearestAvailableCover(Transform enemyHead, Transform playerHead, float shootingRadius, string enemyName, bool markAsOccupied = true)
    {
        Cover nearestCover = null;
        float shortestDistance = Mathf.Infinity;
        float distanceToPlayer = Vector3.Distance(enemyHead.position, playerHead.position);

        foreach (var cover in covers)
        {
            if (cover.isOccupied)
                continue;

            float distanceToCover = Vector3.Distance(enemyHead.position, cover.transform.position);
            float distanceCoverToPlayer = Vector3.Distance(cover.transform.position, playerHead.position);

            if (distanceToCover < shortestDistance && distanceCoverToPlayer <= shootingRadius && HasClearLineOfSightFromCover(enemyHead, playerHead, cover.transform.position))
            {
                shortestDistance = distanceToCover;
                nearestCover = cover;
            }
        }

        if (nearestCover != null && shortestDistance < distanceToPlayer)
        {
            if(markAsOccupied)
            {
                nearestCover.SetOccupiedStatus(true);
                CoverRegistry[enemyName] = nearestCover;
                
                GameManager.Instance.RegisterCover(nearestCover.gameObject.name, nearestCover.transform.position);
            }
            return nearestCover;
        }
        return null;
    }

    private bool HasClearLineOfSightFromCover(Transform enemyHead, Transform playerHead, Vector3 coverPosition)
    {
        float headHeightOffset = enemyHead.position.y - transform.position.y;
        Vector3 raycastStartPosition = coverPosition + Vector3.up * headHeightOffset;
        Vector3 directionToPlayerHead = playerHead.position - raycastStartPosition;
        float distanceToPlayerHead = directionToPlayerHead.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(raycastStartPosition, directionToPlayerHead.normalized, out hit, distanceToPlayerHead))
        {
            return hit.collider.CompareTag(GlobalTags.Player);
        }
        return false;
    }


    public void LeaveCover(string enemyName)
    {
        if (CoverRegistry.ContainsKey(enemyName))
        {
            GameManager.Instance.ReleasePosition(CoverRegistry[enemyName].gameObject.name);
            Debug.Log(CoverRegistry[enemyName].gameObject.name);
            CoverRegistry[enemyName].SetOccupiedStatus(false);
            CoverRegistry.Remove(enemyName);
        }
    }
}