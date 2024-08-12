using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SuperObjectPoolSO", menuName = "ScriptableObjects/SuperObjectPoolSO", order = 2)]
public class SuperObjectPoolSO : ScriptableObject
{
    public PoolableMonoBehaviour prefab;
    public int defaultCapacity = 10;
    public int maxCapacity = 30;
    [Space(10)]
    public Vector3 defaultSpawnPosition;
    [Space(10)]
    public bool useDefaultSpawnRotation;
    public Quaternion defaultSpawnRotation;
    
    private ObjectPool<PoolableMonoBehaviour> _objectPool;
    
    private List<PoolableMonoBehaviour> activeObjects = new List<PoolableMonoBehaviour>();

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        InitializePool();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        Debug.Log("PoolInitialized");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetAndInitializePool();
    }

    private void OnSceneUnloaded(Scene current)
    {
        ResetPool();
    }

    private void InitializePool()
    {
        _objectPool = new ObjectPool<PoolableMonoBehaviour>(
            CreatePooledObject,
            OnTakeFromPool,
            OnReturnFromPool,
            OnDestroyObject,
            false,
            defaultCapacity,
            maxCapacity
        );
    }


    private void ResetAndInitializePool()
    {
        ResetPool();
        InitializePool();
    }

    private void ResetPool()
    {
        foreach (var activeObject in activeObjects)
        {
            if (activeObject != null)
            {
                activeObject.gameObject.SetActive(false);
            }
        }
        Debug.Log("cleared");
        activeObjects.Clear();
    }


    private PoolableMonoBehaviour CreatePooledObject()
    {
        PoolableMonoBehaviour pm = Instantiate(prefab, defaultSpawnPosition, useDefaultSpawnRotation ? defaultSpawnRotation : Quaternion.identity);
        pm.gameObject.SetActive(true);
        pm.RegisterPool(this);
        pm.OnObjectPoolCreate();
        return pm;
    }

    private void OnTakeFromPool(PoolableMonoBehaviour pm)
    {
        activeObjects.Add(pm);
        pm.gameObject.SetActive(true);
        pm.OnObjectPoolTake();
    }

    private void OnReturnFromPool(PoolableMonoBehaviour pm)
    {
        activeObjects.Remove(pm);
        pm.OnObjectPoolReturn();
        pm.gameObject.SetActive(false);
    }


    private void OnDestroyObject(PoolableMonoBehaviour pm)
    {
        pm.OnObjectPoolDestroy();
        Destroy(pm.gameObject);
    }
    
    public PoolableMonoBehaviour Get()
    {
        return _objectPool.Get();
    }

    public void Release(PoolableMonoBehaviour pm)
    {
        _objectPool.Release(pm);
    }
#if UNITY_EDITOR
    /// <summary>
    /// Because Pooling works with SO's and the pools need to be set, which is called on scene load and in Playmode
    /// there is no scene to load, this section is needed to not make it throw nullreferences.s
    /// </summary>
    [InitializeOnLoad]
    public class EditorPlayModeStateHandler
    {
        static EditorPlayModeStateHandler()
        {
            Debug.Log("EditorPlayModeStateHandler initialized");
            EditorApplication.playModeStateChanged += HandlePlayModeState;
        }

        private static void HandlePlayModeState(PlayModeStateChange state)
        {
            Debug.Log($"Play mode state changed to: {state}");
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                ResetAndInitializeAllPoolsInEditor();
            }
        }

        private static void ResetAndInitializeAllPoolsInEditor()
        {
            var allPools = Resources.FindObjectsOfTypeAll<SuperObjectPoolSO>();
            foreach (var pool in allPools)
            {
                pool.ResetAndInitializePool();
            }
        }
    }
#endif

}



