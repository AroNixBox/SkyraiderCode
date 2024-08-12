using UnityEngine;
using UnityEngine.Events;

public abstract class PoolableMonoBehaviour : MonoBehaviour
{
    public UnityAction<PoolableMonoBehaviour> OnRelease;
    
    private SuperObjectPoolSO _pool;
    
    public void RegisterPool(SuperObjectPoolSO pool)
    {
        this._pool = pool;
    }
    
    public void Release()
    {
        if (_pool != null)
        {
            this._pool.Release(this);
        }

        if (OnRelease != null)
        {
            OnRelease(this);
            OnRelease = null;
        }
    }

    public virtual void OnObjectPoolCreate() { }
    public virtual void OnObjectPoolTake() { }
    public virtual void OnObjectPoolReturn() { }
    public virtual void OnObjectPoolDestroy() { }
    //Instantiated after Object Destroyed
    public virtual void OnPostGet() { }
    
}
