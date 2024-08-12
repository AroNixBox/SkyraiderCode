using System;
using UnityEngine;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    [Header("VFX Configurations")]
    [SerializeField] private List<ParticleInfo> particlePools;
    private Dictionary<ParticleType, SuperObjectPoolSO> _vfxPools;

    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeVFXPoolDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeVFXPoolDictionary()
    {
        _vfxPools = new Dictionary<ParticleType, SuperObjectPoolSO>();

        foreach (var pool in particlePools)
        {
            _vfxPools.Add(pool.type, pool.particlePool);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void SpawnParticle(ParticleType type, Vector3 position, Quaternion rotation)
    {
        if (_vfxPools.TryGetValue(type, out var pool))
        {
            PooledParticleSystem shotVFXParticle = (PooledParticleSystem)pool.Get();
            shotVFXParticle.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            Debug.LogWarning($"No pool found for particle type {type}");
        }
    }
    
    //Method to bind it to its parent
    public void SpawnParticleAndBindToParent(ParticleType type, Transform parent)
    {
        if (_vfxPools.TryGetValue(type, out var pool))
        {
            PooledParticleSystem shotVFXParticle = (PooledParticleSystem)pool.Get();
            Quaternion rotation = Quaternion.LookRotation(parent.transform.forward);
            shotVFXParticle.transform.SetPositionAndRotation(parent.transform.position, rotation);
            shotVFXParticle.transform.SetParent(parent);
        }
        else
        {
            Debug.LogWarning($"No pool found for particle type {type}");
        }
    }
}

[System.Serializable]
public class ParticleInfo
{
    public ParticleType type;
    public SuperObjectPoolSO particlePool;
}

public enum ParticleType
{
    BulletImpact,
    SmallBloodImpact,
    RifleShotFlame,
    BulletShot,
    BigBloodImpact,
    ObjectiveFlame
}
