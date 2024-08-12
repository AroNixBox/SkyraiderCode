using System;
using UnityEngine;

public class PooledParticleSystem : PoolableMonoBehaviour
{
    private ParticleSystem _system;
    private bool _isDisabled;

    private void Awake()
    {
        _system = GetComponent<ParticleSystem>();
        var main = _system.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    public override void OnObjectPoolTake()
    {
        base.OnObjectPoolTake();
        _isDisabled = false;
    }

    private void OnParticleSystemStopped()
    {
        ReleaseParticleSystem();
    }

    private void OnParticleCollision(GameObject other)
    {
        ReleaseParticleSystem();
    }
    private void ReleaseParticleSystem()
    {
        if (_isDisabled)
        {
            return;
        }
        _isDisabled = true;
        this.Release();
    }
}