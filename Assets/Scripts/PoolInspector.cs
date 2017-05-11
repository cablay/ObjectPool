using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolInspector : MonoBehaviour {

    GameObject original;
    bool hasMaxSize;

    public void Initialize(GameObject original)
    {
        if (this.original == null && original != null)
        {
            this.original = original;
            return;
        }

        if (original == null)
        {
            ObjectPool.logger.LogWarning("PoolInspector", "Tried to Initialize using a GameObject that does not exist.");
        }

        if (this.original != null)
        {
            ObjectPool.logger.LogWarning("PoolInspector", "Tried to Initialize a PoolInspector that was already initialized.");
        }
    }

    public GameObject Original
    {
        get { return original; }
    }

    public int InactiveObjects
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.InactiveObjects) ?? 0; }
    }

    public int ActiveObjects
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.ActiveObjects) ?? 0; }
    }

    public bool DestroyOnLoad
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.DestroyOnLoad) != 0; }
        set
        {
            if (value != (ObjectPool.GetProperty(original, ObjectPool.PoolProperties.DestroyOnLoad) != 0))
            {
                if (value) { ObjectPool.DoDestroyOnLoad(original); }
                else { ObjectPool.DontDestroyOnLoad(original); }
            }
        }
    }

    public bool HasMaxSize
    {
        get
        {
            hasMaxSize = hasMaxSize || MaxSize < int.MaxValue;
            return hasMaxSize;
        }
        set
        {
            hasMaxSize = value;

            if (!value)
            {
                ObjectPool.LimitPoolSize(original, int.MaxValue);
            }
        }
    }

    public int MaxSize
    {
        get{ return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.MaxSize) ?? 0; }
        set { ObjectPool.LimitPoolSize(original, value); }
    }
}
