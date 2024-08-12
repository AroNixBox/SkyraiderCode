using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine.Utility;
using Enemy.Monobehaviors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int levelRestartDelay = 15;
    [SerializeField] private SceneField mainMenuScene;
    public static GameManager Instance { get; private set; }
    private List<EnemyHealth> _enemiesAlive = new List<EnemyHealth>();
    private Dictionary<EnemyHealth, EnemyVision> enemyVisionLookup = new Dictionary<EnemyHealth, EnemyVision>();
    private Dictionary<string, Vector3> occupiedPositions = new Dictionary<string, Vector3>();
    public event Action OnPlayerDied;
    public event Action OnObjectiveComplete;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void OnDrawGizmos()
    {
        if (occupiedPositions == null) return;

        foreach (KeyValuePair<string, Vector3> entry in occupiedPositions)
        {
            // Setzen Sie die Gizmo-Farbe
            Gizmos.color = Color.green;

            // Zeichnen Sie eine kleine Kugel an der Position
            Gizmos.DrawSphere(entry.Value, 0.5f);

            // Optional: Zeichnen Sie den Namen des GameObjects in der Nähe der Kugel
#if UNITY_EDITOR
            UnityEditor.Handles.Label(entry.Value, entry.Key);
#endif
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RegisterAllEnemies();
    }

    public List<EnemyHealth> GetEnemiesAlive()
    {
        return _enemiesAlive;
    }

    public void AlarmEnemiesInRange(Vector3 position, float range)
    {
        foreach (var enemy in _enemiesAlive)
        {
            if (enemy.transform != this.transform)
            {
                float distanceToEnemy = Vector3.Distance(enemy.transform.position, position);
                if (distanceToEnemy <= range)
                {
                    EnemyVision enemyVision = enemyVisionLookup[enemy];
                    if (enemyVision != null)
                    {
                        enemyVision.hasRecievedAlarm = true;
                    }
                }
            }
        }
    }

    private void RegisterAllEnemies()
    {
        if (_enemiesAlive.Count > 0)
            _enemiesAlive.Clear();

        foreach (var enemy in FindObjectsOfType<EnemyHealth>())
        {
            _enemiesAlive.Add(enemy);
            enemyVisionLookup[enemy] = enemy.GetComponent<EnemyVision>();
        }
    }

    public void UnregisterEnemy(EnemyHealth enemyHealth)
    {
        if (_enemiesAlive.Contains(enemyHealth))
        {
            _enemiesAlive.Remove(enemyHealth);
            enemyVisionLookup.Remove(enemyHealth);
            if (_enemiesAlive.Count == 0)
            {
                Debug.Log("All Enemies Dead");
            }
        }
    }

    public void RegisterCover(string gameObjectName, Vector3 coverPosition)
    {
        if (occupiedPositions.ContainsKey(gameObjectName))
        {
            Debug.LogWarning("Coverposition is already in Dict");
            return;
        }
        occupiedPositions.Add(gameObjectName, coverPosition);
    }

    public Vector3 RequestNewPosition(Vector3 enemyCurrentPos, Vector3 playerHead, float attackRadius, float minDistanceFromPlayer, float mindistanceFromOtherEnemies)
    {
        NavMeshPath pathToPlayerVicinity = new NavMeshPath();
        if (NavMesh.CalculatePath(enemyCurrentPos, playerHead, NavMesh.AllAreas, pathToPlayerVicinity))
        {
            Vector3 strategicPosition = FindStrategicPosition(pathToPlayerVicinity, playerHead, attackRadius, minDistanceFromPlayer, mindistanceFromOtherEnemies);
            if (strategicPosition != Vector3.positiveInfinity)
            {
                return strategicPosition;
            }
        }
        return playerHead;
    }

    private Vector3 FindStrategicPosition(NavMeshPath pathToPlayerVicinity, Vector3 playerHead, float attackRadius, float minDistanceFromPlayer, float minDistanceFromOtherEnemies)
    {
        Vector3 lastPathCorner = pathToPlayerVicinity.corners[pathToPlayerVicinity.corners.Length - 1];
        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < 50; i++)
        {
            Vector3 samplePoint = lastPathCorner + Random.insideUnitSphere * attackRadius;
            samplePoint.y = lastPathCorner.y;

            if (NavMesh.SamplePosition(samplePoint, out var hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 sampledPosition = hit.position;
                
                if (NavMesh.CalculatePath(lastPathCorner, sampledPosition, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete && 
                    Vector3.Distance(sampledPosition, playerHead) > minDistanceFromPlayer &&
                    !IsTooCloseToOtherPositions(sampledPosition, minDistanceFromOtherEnemies))
                {
                    if (HasClearLineOfSight(sampledPosition, playerHead))
                    {
                        return sampledPosition;
                    }
                }
            }
        }
        return Vector3.positiveInfinity;
    }

    private bool HasClearLineOfSight(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 rayStartPos = fromPosition + Vector3.up * .9f;
        Vector3 directionToTarget = (toPosition - rayStartPos).normalized;
    
        if (Physics.Raycast(rayStartPos, directionToTarget, out var hit, Vector3.Distance(fromPosition, toPosition)))
        {
            return hit.collider.CompareTag(GlobalTags.Player);
        }
        return false;
    }

    public bool IsTooCloseToOtherPositions(Vector3 position, float minDistance)
    {
        foreach (var entry in occupiedPositions)
        {
            if (Vector3.Distance(entry.Value, position) < minDistance)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsPositionOccupied(string key)
    {
        return occupiedPositions.ContainsKey(key);
    }
    public void ReleasePosition(string gameObjectName)
    {
        occupiedPositions.Remove(gameObjectName);
    }

    
    public void PlayerDied()
    {
        OnPlayerDied?.Invoke();
        StartCoroutine(WaitForRestart());
    }

    IEnumerator WaitForRestart()
    {
        var task = SceneManager.LoadSceneAsync(mainMenuScene.SceneName);
        task.allowSceneActivation = false;
        
        yield return new WaitUntil(() => task.progress >= 0.9f);

        
        yield return new WaitForSeconds(levelRestartDelay);
        task.allowSceneActivation = true;
    }
    public void ObjectiveComplete()
    {
        OnObjectiveComplete?.Invoke();
    }
}
