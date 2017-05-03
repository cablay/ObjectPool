using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Class used for pooling GameObjects.
/// </summary>
public static class ObjectPool
{
    static Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>(); //collection of pools where the key is the GameObject to be pooled
    static bool initialized = false; //initialization flag is used so the user doesn't have to intialize explicitly



    /// <summary>
    /// Returns a spawned clone of the GameObject original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing GameObject that you want to spawn a clone of from a pool.</param>
    /// <returns>The spawned clone.</returns>
    public static GameObject Spawn(GameObject original)
    {
        return iSpawn(original);
    }

    /// <summary>
    /// Returns a spawned clone of the GameObject original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="parent">Parent that will be assigned to the spawned GameObject.</param>
    /// <returns>The spawned clone.</returns>
    public static GameObject Spawn(GameObject original, Transform parent)
    {
        return iSpawn(original, iParent: parent);
    }

    /// <summary>
    /// Returns a spawned clone of the GameObject original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="parent">Parent that will be assigned to the spawned GameObject.</param>
    /// <param name="spawnInWorldSpace">Pass true when assigning a parent GameObject to maintain the world position of original, instead of setting its position relative to the new parent. Pass false to set the GameObject's position relative to its new parent.</param>
    /// <returns>The spawned clone.</returns>
    public static GameObject Spawn(GameObject original, Transform parent, bool spawnInWorldSpace)
    {
        return iSpawn(original, iParent : parent, iWorldSpace : spawnInWorldSpace);
    }

    /// <summary>
    /// Returns a spawned clone of the GameObject original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="position">Position for the new GameObject.</param>
    /// <param name="rotation">Orientation of the new GameObject.</param>
    /// <returns>The spawned clone.</returns>
    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation)
    {
        return iSpawn(original, position, rotation);
    }

    /// <summary>
    /// Returns a spawned clone of the GameObject original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="position">Position for the new GameObject.</param>
    /// <param name="rotation">Orientation of the new GameObject.</param>
    /// <param name="parent">Parent that will be assigned to the spawned GameObject.</param>
    /// <returns>The spawned clone.</returns>
    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
    {
        return iSpawn(original, position, rotation, parent);
    }

    /// <summary>
    /// Returns a Component attached to a spawned clone of the GameObject with Component original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you want to spawn a clone of from a pool.</param>
    /// <returns>The spawned clone's Component.</returns>
    public static T Spawn<T>(T original) where T : Component
    {
        return iSpawn(original);
    }

    /// <summary>
    /// Returns a Component attached to a spawned clone of the GameObject with Component original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="parent">Parent that will be assigned to the spawned Component's GameObject.</param>
    /// <returns>The spawned clone's Component.</returns>
    public static T Spawn<T>(T original, Transform parent) where T : Component
    {
        return iSpawn(original, iParent: parent);
    }

    /// <summary>
    /// Returns a Component attached to a spawned clone of the GameObject with Component original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="parent">Parent that will be assigned to the spawned Component's GameObject.</param>
    /// <param name="spawnInWorldSpace">Pass true when assigning a parent GameObject to maintain the world position of original, instead of setting its position relative to the new parent. Pass false to set the Component's GameObject's position relative to its new parent.</param>
    /// <returns>The spawned clone's Component.</returns>
    public static T Spawn<T>(T original, Transform parent, bool spawnInWorldSpace) where T : Component
    {
        return iSpawn(original, iParent: parent, iWorldSpace: spawnInWorldSpace);
    }

    /// <summary>
    /// Returns a Component attached to a spawned clone of the GameObject with Component original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="position">Position for the new Component's GameObject.</param>
    /// <param name="rotation">Orientation of the new Component's GameObject.</param>
    /// <returns>The spawned clone's Component.</returns>
    public static T Spawn<T>(T original, Vector3 position, Quaternion rotation) where T : Component
    {
        return iSpawn(original, position, rotation);
    }

    /// <summary>
    /// Returns a Component attached to a spawned clone of the GameObject with Component original from a pool. If no pool exists for original one is created.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you want to spawn a clone of from a pool.</param>
    /// <param name="position">Position for the new Component's GameObject.</param>
    /// <param name="rotation">Orientation of the new Component's GameObject.</param>
    /// <param name="parent">Parent that will be assigned to the spawned Component's GameObject.</param>
    /// <returns>The spawned clone's Component.</returns>
    public static T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Component
    {
        return iSpawn(original, position, rotation, parent);
    }

    /// <summary>
    /// Despawns a pooled GameObject, deactivating it and returning it to the pool.
    /// </summary>
    /// <param name="obj">The GameObject to despawn.</param>
    /// <param name="t">The optional amount of time to delay before despawning the GameObject.</param>
    public static void Despawn(GameObject obj, float t = 0.0F)
    {
        if (obj == null) //check for bad or Destroyed object
        {
            Debug.LogError("OjectPool: Tried to Despawn an object that does not exist.");
            return;
        }

        t = Mathf.Max(0.0F, t); //ensure we have a valid time
        PooledObject po = obj.GetComponent<PooledObject>();

        if (po == null) //ensure this was an object we created that hasn't been compromised
        {
            Debug.LogWarning("ObjectPool: Tried to Despawn a non-pooled object. Destroying instead", obj);
            Object.Destroy(obj);
        }
        else if (po.pool == null)
        {
            Debug.LogWarning("ObjectPool: Tried to Despawn an object whose pool no longer exists. Destroying instead", obj);
            Object.Destroy(obj);
        }
        else
        {
            if (!obj.activeSelf)
            {
                Debug.Log("ObjectPool: Tried to Despawn an inactive object. Will still add to inactive pool.");
            }

            if (t == 0.0F) //no need to call the coroutine in PooledObject if we're Dewpawning now
            {
                po.pool.Despawn(obj);
            }
            else
            {
                po.Despawn(t); //need to delay the Despawn using a coroutine in PooledObject
            }
        }
    }

    /// <summary>
    /// Despawns a pooled GameObject attached to Component obj, deactivating it and returning it to the pool.
    /// </summary>
    /// <param name="obj">The Component attached to a GameObject to despawn.</param>
    /// <param name="t">The optional amount of time to delay before despawning the GameObject.</param>
    public static void Despawn(Component obj, float t = 0.0F)
    {
        if (obj == null)
        {
            Debug.LogError("ObjectPool: Tried to Despawn an object with a component that does not exist.");
            return;
        }

        Despawn(obj.gameObject, t);
    }

    /// <summary>
    /// Add the specified number of deactivated GameObjects to the pool made from original.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    /// <param name="numObjs">The number of deactivated clones of original you wish to add to the pool.</param>
    public static void PreWarm(GameObject original, int numObjs)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to PreWarm a pool for an object that does not exist.");
            return;
        }

        if (!initialized) //do any needed one-time setup without any prerequisite user action
        {
            Initialize();
        }

        if (!pools.ContainsKey(original))
        {
            pools.Add(original, new Pool(original, numObjs));
        }
        else if (pools[original] == null)
        {
            Debug.LogWarning("ObjectPool: Tried to PreWarm a pool that no longer exists. Creating new pool.");
            pools[original] = new Pool(original, numObjs);
        }

        pools[original].Prewarm(numObjs);
    }

    /// <summary>
    /// Add the specified number of deactivated GameObjects to the pool made from the GameObject original is attached to.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    /// <param name="numObjs">The number of deactivated clones of the GameObject original is attached to you wish to add to the pool.</param>
    public static void PreWarm(Component original, int numObjs)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to PreWarm a pool for an object with a component that does not exist.");
            return;
        }

        PreWarm(original.gameObject, numObjs);
    }

    /// <summary>
    /// Sets the capacity of the underlying container for the pool made from original to the number of GameObjects currently in the pool, if that number is less than a threshold value.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void Trim(GameObject original)
    {
        if (original == null || !pools.ContainsKey(original))
        {
            Debug.LogError("ObjectPool: Tried to Trim a pool that does not exist.");
            return;
        }
        else if (pools[original] == null)
        {
            Debug.LogError("ObjectPool: Tried to Trim a pool that no longer exists.");
            return;
        }

        pools[original].Trim();
    }

    /// <summary>
    /// Sets the capacity of the underlying container for the pool made from the GameObject original is attached to to the number of GameObjects currently in the pool, if that number is less than a threshold value.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    public static void Trim(Component original)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to Trim a pool for an object with a component that does not exist.");
            return;
        }

        Trim(original.gameObject);
    }

    /// <summary>
    /// Destroy the pool made from original, along with all GameObjects associated with it.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DestroyPool(GameObject original)
    {
        if (original == null || !pools.ContainsKey(original))
        {
            Debug.LogError("ObjectPool: Tried to Destroy a pool that does not exist.");
            return;
        }
        else if (pools[original] == null)
        {
            Debug.LogError("ObjectPool: Tried to Destroy a pool that no longer exists."); //no return after this because we want to remove the null pool
        }

        pools.Remove(original);
    }

    /// <summary>
    /// Destroy the pool made from the GameObject original is attached to, along with all GameObjects associated with it.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    public static void DestroyPool(Component original)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to DestroyPool using an object with a component that does not exist.");
            return;
        }

        DestroyPool(original.gameObject);
    }

    /// <summary>
    /// Despawn all active GameObjects associated with the pool made from original.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DespawnPool(GameObject original)
    {
        if (original == null || !pools.ContainsKey(original))
        {
            Debug.LogError("ObjectPool: Tried to DespawnPool a pool that does not exist.");
            return;
        }
        else if (pools[original] == null)
        {
            Debug.LogError("ObjectPool: Tried to DespawnAllPool a pool that no longer exists.");
            DestroyInvalidPools(); //remove the null pool
            return;
        }

        pools[original].DespawnAll();
    }

    /// <summary>
    /// Despawn all active GameObjects associated with the pool made from the GameObject original is attached to.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    public static void DespawnPool(Component original)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to DespawnAllPool using an object with a component that does not exist.");
            return;
        }

        DespawnPool(original.gameObject);
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from original not be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DontDestroyOnLoad(GameObject original)
    {
        if (original == null || !pools.ContainsKey(original))
        {
            Debug.LogError("ObjectPool: Tried to DontDestroyOnLoad a pool that does not exist.");
            return;
        }
        else if (pools[original] == null)
        {
            Debug.LogError("ObjectPool: Tried to DontDestroyOnLoad a pool that no longer exists.");
            return;
        }

        pools[original].DontDestroyOnLoad();
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from the GameObject original is attached to not be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    public static void DontDestroyOnLoad(Component original)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to DontDestroyOnLoad using an object with a component that does not exist.");
            return;
        }

        DontDestroyOnLoad(original.gameObject);
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from original be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DoDestroyOnLoad(GameObject original)
    {
        if (original == null || !pools.ContainsKey(original))
        {
            Debug.LogError("ObjectPool: Tried to DoDestroyOnLoad a pool that does not exist.");
            return;
        }
        else if (pools[original] == null)
        {
            Debug.LogError("ObjectPool: Tried to DoDestroyOnLoad a pool that no longer exists.");
            return;
        }

        pools[original].DoDestroyOnLoad();
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from the GameObject original is attached to be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing Component attached to a GameObject that you've made a pool from.</param>
    public static void DoDestroyOnLoad(Component original)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to DoDestroyOnLoad using an object with a component that does not exist.");
            return;
        }

        DoDestroyOnLoad(original.gameObject);
    }

    //Internal function to spawn a pooled clone from user arguments, creating a new pool for original if necessary.
    static GameObject iSpawn(GameObject original, Vector3? pos = null, Quaternion? rot = null, Transform iParent = null, bool iWorldSpace = false)
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to Spawn() an object that does not exist.");
            return null;
        }

        if (!initialized) //do any needed one-time setup without any prerequisite user action
        {
            Initialize();
        }

        if (!pools.ContainsKey(original)) //create new Pool if none exists for the given GameObject
        {
            pools.Add(original, new Pool(original));
        }
        else if (pools[original] == null) //create new Pool if one no longer exists for the given GameObject
        {
            Debug.LogWarning("ObjectPool: Tried to Spawn from a pool that no longer exists. Creating new pool.");
            pools[original] = new Pool(original);
        }

        Vector3 position;
        Quaternion rotation;

        if (pos.HasValue && rot.HasValue) //use user-specified Transform values
        {
            position = (Vector3)pos;
            rotation = (Quaternion)rot;
        }
        else if (iParent == null || iWorldSpace) //use original's Transform values (no parent or as specified by user)
        {
            position = original.transform.position;
            rotation = original.transform.rotation;
        }
        else //use user-specified parent's Transform values
        {
            position = iParent.position + original.transform.position;
            rotation = iParent.rotation * original.transform.rotation;
        }

        return pools[original].Spawn(position, rotation, iParent); //get and return GameObject from the correct pool
    }

    //Component-returning wrapper for iSpawn(GameObject, ...)
    static T iSpawn<T>(T original, Vector3? pos = null, Quaternion? rot = null, Transform iParent = null, bool iWorldSpace = false) where T : Component
    {
        if (original == null)
        {
            Debug.LogError("ObjectPool: Tried to Spawn() an object with a Component that does not exist.");
            return null;
        }

        GameObject inst = iSpawn(original.gameObject, pos, rot, iParent, iWorldSpace); //get GameObject from the correct pool
        T requestedComponent = inst.GetComponent<T>();

        if (requestedComponent != null) //successfully spawned GameObject with requested Component
        {
            return requestedComponent;
        }
        else
        {
            Debug.LogError("ObjectPool: Tried to Spawn() an object with a component that does not exist on the pooled objects.");
            Despawn(inst); //recycle spawned GameObject
            return null;
        }
    }

    //Remove pools with original GameObjects that no longer exist
    static void DestroyInvalidPools()
    {
        pools.Remove(null);
    }

    //One-time ObjectPool setup
    static void Initialize()
    {
        initialized = true;

        SceneManager.activeSceneChanged += SceneChanged; //call SceneChanged when scene change event is triggered
    }

    //Remove all pools that aren't marked as DontDestroyOnLoad when the scene changes
    static void SceneChanged(Scene scene1, Scene scene2)
    {
        foreach (KeyValuePair<GameObject, Pool> pool in pools)
        {
            if (pool.Value.destroyOnLevelLoad)
            {
                pools.Remove(pool.Key);
            }
        }
    }



    class Pool
    {
        const int DEFAULT_POOL_SIZE = 1; //default initial stack size if none is specified on pool creation

        public bool destroyOnLevelLoad; //deterines if the pool and its associated GameObjects should be destroyed on level load

        Stack<GameObject> pool; //underlying data container
        GameObject original; //original GameObject that pooled clones are made from
        Transform parentObjectTransform; //the default parent for pooled GameObjects

        UnityEvent despawnEvent;
        UnityEvent destroyEvent;
        UnityEvent dontDestroyOnLoadEvent;

        public Pool(GameObject obj, int startSize = DEFAULT_POOL_SIZE)
        {
            pool = new Stack<GameObject>(startSize);
            original = obj;

            GameObject parentObject = new GameObject(original.name + " Pool"); //create the default parent for pooled GameObjects
            parentObjectTransform = parentObject.transform;

            destroyOnLevelLoad = true; //default is to destroy pool and all its associated GameObjects on level load
            despawnEvent = new UnityEvent();
            destroyEvent = new UnityEvent();
            dontDestroyOnLoadEvent = new UnityEvent();
        }

        ~Pool()
        {
            Object.Destroy(parentObjectTransform.gameObject); //destroy parentObjectTransform and all pooled objects that are not children of parentObjectTransform

            destroyEvent.Invoke(); //destroy all pooled objects that are not children of parentObjectTransform
        }

        //Spawn a pooled GameObject with the specified position, rotation, and parent
        public GameObject Spawn(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), Transform parent = null)
        {
            GameObject inst = null;

            if (pool.Count > 0) //if the stack has instances in it, try to get one of them
            {
                while (inst == null && pool.Count > 0) //ensure we get a valid instance
                {
                    inst = pool.Pop();
                }

                if (inst == null) //if there were no valid instances in stack
                {
                    inst = createNew(pos, rot, parent);
                }
                else //if we successfully got an instance from the stack, get it ready to use
                {
                    inst.transform.position = pos;
                    inst.transform.rotation = rot;
                    if (parent != null) { inst.transform.parent = parent; }
                }
            }
            else //if there's an empty stack, Instantiate a new object
            {
                inst = createNew(pos, rot, parent);
            }

            inst.SetActive(true); //Spawn should consistently  return an active GameObject whether original is active or not
            return inst;
        }

        //Despawn a pooled GameObject after an optional delay
        public void Despawn(GameObject obj, float time = 0.0F)
        {
            if (original != null)
            {
                obj.SetActive(false);
                pool.Push(obj);
            }
            else
            {
                Debug.LogWarning("ObjectPool: Tried to Despawn() object whose pool no longer exists. Destroying instead", obj);
                Object.Destroy(obj);

                ObjectPool.DestroyInvalidPools(); //destroy all pools for GameObjects that no longer exist, including this one
            }
        }

        //Create the specified number of deactivated pooled GameObjects
        public void Prewarm(int numObjs)
        {
            for (int i = 0; i < numObjs; i++)
            {
                GameObject inst = createNew();
                Despawn(inst);
            }
        }

        //Sets the capacity of the underlying container for the pool to the number of GameObjects currently in the pool, if that number is less than a threshold value
        public void Trim()
        {
            pool.TrimExcess();
        }

        //Despawn all GameObjects associated with the pool
        public void DespawnAll()
        {
            despawnEvent.Invoke();
        }

        //Call DontDestroyOnLoad with all GameObjects associated with the pool and clear this pool's related flag
        public void DontDestroyOnLoad()
        {
            dontDestroyOnLoadEvent.Invoke();
            destroyOnLevelLoad = false;
        }

        //Set this pool's destroyOnLevelLoad flag
        public void DoDestroyOnLoad()
        {
            destroyOnLevelLoad = true;
        }

        //Helper function to create a new GameObject as a clone of original with the given position, rotation, and parent
        GameObject createNew(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), Transform parent = null)
        {
            Transform instParent = (parent == null) ? parentObjectTransform : parent;
            GameObject inst = Object.Instantiate(original, pos, rot, instParent);

            PooledObject po = inst.AddComponent<PooledObject>(); //add the component PooledObject, which identifies the GameObject as belonging to this pool
            po.Initialize(this, despawnEvent, destroyEvent, dontDestroyOnLoadEvent); //assocaite the PooledObject with this pool and hook up its relevant functions to this pool's events

            return inst;
        }
    }



    class PooledObject : MonoBehaviour
    {
        public Pool pool; //the pool that the GameObject that this component is attached to belongs to

        //Assocaite the PooledObject with the given pool and hook up the relevant functions to the pool's events
        public void Initialize(Pool pool, UnityEvent despawnEvent, UnityEvent destroyEvent, UnityEvent dontDestroyOnLoadEvent)
        {
            this.pool = pool;
            hideFlags = HideFlags.HideInInspector;

            despawnEvent.AddListener(DespawnNow);
            destroyEvent.AddListener(Destroy);
            dontDestroyOnLoadEvent.AddListener(DontDestroy);
        }

        //Make use of MonoBehavior inheritance to enable the option of a Despawn delay
        public void Despawn(float time)
        {
            StartCoroutine(DespawnCo(time));
        }

        IEnumerator DespawnCo(float time)
        {
            yield return new WaitForSeconds(time);

            if (pool != null)
            {
                pool.Despawn(gameObject);
            }
            else
            {
                Debug.LogWarning("ObjectPool: Tried to Despawn() object whose pool no longer exists. Destroying instead", gameObject);
                Object.Destroy(gameObject);
            }
        }

        //Despawn gameObject when pool's despawnEvent is triggered
        void DespawnNow()
        {
            pool.Despawn(gameObject);
        }

        //Call DontDestroyOnLoad when pool's dontDestroyOnLoadEvent is triggered
        void DontDestroy()
        {
            DontDestroyOnLoad(this);
        }

        //Destroy gameObject when pool's destroyEvent is triggered
        void Destroy()
        {
            Destroy(gameObject);
        }
    }
}