using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolInspector : MonoBehaviour
{
    GameObject m_original;

    /// <summary>
    /// Set the pool to be inspected using the GameObject it was made from. Does not need to be called by the user.
    /// </summary>
    /// <param name="original">The GameObject the pool to be inspected was made from.</param>
    public void Initialize(GameObject original)
    {
        if (m_original != null || original == null)
        {
            if (m_original != null)
            {
                ObjectPool.logger.LogWarning("PoolInspector", "Tried to Initialize a PoolInspector that was already initialized.");
            }

            if (original == null)
            {
                ObjectPool.logger.LogWarning("PoolInspector", "Tried to Initialize using a GameObject that does not exist.");
            }
        }
        else
        {
            m_original = original;
        }
    }

    /// <summary>
    /// The GameObject the pool being inspected was made from.
    /// </summary>
    public GameObject original
    {
        get { return m_original; }
    }
}
