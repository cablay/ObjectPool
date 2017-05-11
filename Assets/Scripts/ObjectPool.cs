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
    public enum PoolProperties { DestroyOnLoad, ActiveObjects, InactiveObjects, MaxSize}; //pool properties that can be queried

    public static Logger logger
    {
        get
        {
            return m_logger;
        }
        set
        {
            if (value != null)
            {
                m_logger = value;
            }
            else
            {
                LogError("Tried to set logger to a Logger object that does not exist.");
            }
        }
    }

    public static int defaultMaxPoolSize
    {
        get
        {
            return m_defaultMaxPoolSize;
        }
        set
        {
            if (value > 0)
            {
                m_defaultMaxPoolSize = value;
            }
            else
            {
                LogError("Tried to set defaultMaxPoolSize to a value less than 1.");
            }
        }
    }

    static Logger m_logger = new Logger(Debug.logger.logHandler); //publicly-accessible Logger to allow users to easily configure logging options
    static int m_defaultMaxPoolSize = int.MaxValue; //allow user to set a global default max size for new pools created

    enum PoolErrors { NullObject, NullPool, NoPool, NoError }; //common pool errors

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
            LogError("Tried to Despawn an object that does not exist.");
            return;
        }

        t = Mathf.Max(0.0F, t); //ensure we have a valid time
        PooledObject po = obj.GetComponent<PooledObject>();

        if (po == null) //ensure this was an object we created that hasn't been compromised
        {
            LogWarning("Tried to Despawn a non-pooled object. Destroying instead.");
            Object.Destroy(obj);
        }
        else if (po.pool == null)
        {
            LogWarning("Tried to Despawn an object whose pool no longer exists. Destroying instead.");
            Object.Destroy(obj);
        }
        else
        {
            if (!obj.activeSelf)
            {
                Log("Tried to Despawn an inactive object. Will still add to inactive pool.");
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
    /// Add the specified number of deactivated GameObjects to the pool made from original.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    /// <param name="numObjs">The number of deactivated clones of original you wish to add to the pool.</param>
    public static void PreWarm(GameObject original, int numObjs)
    {
        if (original == null)
        {
            LogError("Tried to PreWarm a pool for an object that does not exist.");
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
            LogWarning("Tried to PreWarm a pool that no longer exists. Creating new pool.");
            pools[original] = new Pool(original, numObjs);
        }

        pools[original].Prewarm(numObjs);
    }

    /// <summary>
    /// Sets the capacity of the underlying container for the pool made from original to the number of GameObjects currently in the pool, if that number is less than a threshold value.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void Trim(GameObject original)
    {
        if (ValidatePool(original, "DontDestroyOnLoad") != PoolErrors.NoError)
        {
            return;
        }

        pools[original].Trim();
    }

    /// <summary>
    /// Destroy the pool made from original, along with all GameObjects associated with it.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DestroyPool(GameObject original)
    {
        PoolErrors poolError = ValidatePool(original, "DestroyPool");
        if (poolError == PoolErrors.NullObject || poolError == PoolErrors.NoPool ) //no return if the pool is null
        {
            return;
        }

        pools[original].Cleanup();
        pools.Remove(original);
    }

    /// <summary>
    /// Despawn all active GameObjects associated with the pool made from original.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DespawnPool(GameObject original)
    {
        PoolErrors poolError = ValidatePool(original, "DespawnPool");
        if (poolError == PoolErrors.NullPool)
        {
            pools.Remove(original); //remove the null pool
            return;
        }
        else if (poolError != PoolErrors.NoError)
        {
            return;
        }

        pools[original].DespawnAll();
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from original not be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DontDestroyOnLoad(GameObject original)
    {
        if (ValidatePool(original, "DontDestroyOnLoad") != PoolErrors.NoError)
        {
            return;
        }

        pools[original].DestroyOnLoad(false);
    }

    /// <summary>
    /// Makes all GameObjects associated with the pool made from original be destroyed automatically when loading a new scene.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    public static void DoDestroyOnLoad(GameObject original)
    {
        if (ValidatePool(original, "DoDestroyOnLoad") != PoolErrors.NoError)
        {
            return;
        }

        pools[original].DestroyOnLoad(true);
    }

    /// <summary>
    /// Limits the total number of objects, active and inactive, associated with the pool made from original.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    /// <param name="maxSize">The maximum number of objects, active and inactive, in your pool.</param>
    public static void LimitPoolSize(GameObject original, int maxSize)
    {
        if (ValidatePool(original, "LimitPoolSize") != PoolErrors.NoError)
        {
            return;
        }
        else if (maxSize < 1)
        {
            LogError("Tried to LimitPoolSize with a maxSize less than 1.");
            return;
        }

        pools[original].maxSize = maxSize;
    }

    /// <summary>
    /// Returns an integer representing the value of the requested property of the pool made from original. Returns null if invalid arguments are used.
    /// </summary>
    /// <param name="original">An existing GameObject that you've made a pool from.</param>
    /// <param name="property">An property of the pool made from original.</param>
    /// <returns>An nullable integer representing the value of the requested PoolProperty.</returns>
    public static int? GetProperty(GameObject original, PoolProperties property)
    {
        if (ValidatePool(original, "GetProperty") != PoolErrors.NoError)
        {
            return null;
        }

        Pool pool = pools[original];

        switch (property)
        {
            case PoolProperties.DestroyOnLoad:
                return pool.destroyOnLevelLoad ? 1 : 0;
            case PoolProperties.ActiveObjects:
                return pool.activeObjectCount;
            case PoolProperties.InactiveObjects:
                return pool.inactiveObjects.Count;
            case PoolProperties.MaxSize:
                return pool.maxSize;
            default:
                LogError("Tried to GetProperty a pool property that does not exists.");
                return null;
        }
    }

    //Internal function to spawn a pooled clone from user arguments, creating a new pool for original if necessary.
    static GameObject iSpawn(GameObject original, Vector3? pos = null, Quaternion? rot = null, Transform iParent = null, bool iWorldSpace = false)
    {
        if (original == null)
        {
            LogError("Tried to Spawn() an object that does not exist.");
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
            LogWarning("Tried to Spawn from a pool that no longer exists. Creating new pool.");
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
            LogError("Tried to Spawn() an object with a Component that does not exist.");
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
            LogError("Tried to Spawn() an object with a component that does not exist on the pooled objects.");
            Despawn(inst); //recycle spawned GameObject
            return null;
        }
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
        List<GameObject> originals = new List<GameObject>(pools.Keys);

        foreach (GameObject original in originals)
        {
            if (pools[original].destroyOnLevelLoad)
            {
                DestroyPool(original);
            }
        }
    }

    //Consolidate frequent validation checks
    static PoolErrors ValidatePool(GameObject original, string function)
    {
        if (original == null)
        {
            LogError(string.Format("Tried to {0} a pool for an object that does not exist.", function));
            return PoolErrors.NullObject;
        }
        else if (!pools.ContainsKey(original))
        {
            LogError(string.Format("Tried to {0} a pool that does not exist.", function));
            return PoolErrors.NoPool;
        }
        else if (pools[original] == null)
        {
            LogError(string.Format("Tried to {0} a pool that no longer exists.", function));
            return PoolErrors.NullPool;
        }

        return PoolErrors.NoError;
    }

    //Logging functions with uniform tag
    static void Log(string message, Object context = null)
    {
        m_logger.Log("ObjectPool", message, context);
    }

    static void LogWarning(string message, Object context = null)
    {
        m_logger.LogWarning("ObjectPool", message, context);
    }

    static void LogError(string message, Object context = null)
    {
        m_logger.LogError("ObjectPool", message, context);
    }



    class Pool
    {
        const int DEFAULT_POOL_SIZE = 1; //default initial stack size if none is specified on pool creation 

        public bool destroyOnLevelLoad; //deterines if the pool and its associated GameObjects should be destroyed on level load
        public Stack<GameObject> inactiveObjects; //underlying data container
        public GameObject original; //original GameObject that pooled clones are made from
        public int activeObjectCount; //number of clones active in the scene
        public int maxSize; //maximum number of clones (active or inactive) allowed for this pool

        Transform parentObjectTransform; //the default parent for pooled GameObjects

        UnityEvent despawnEvent;
        UnityEvent destroyEvent;
        UnityEvent dontDestroyOnLoadEvent;

        public Pool(GameObject original, int startSize = DEFAULT_POOL_SIZE)
        {
            inactiveObjects = new Stack<GameObject>(startSize);
            this.original = original;
            maxSize = defaultMaxPoolSize;

            GameObject parentObject = new GameObject(original.name + " Pool"); //create the default parent for pooled GameObjects
            parentObjectTransform = parentObject.transform;

            PoolInspector inspector = parentObject.AddComponent<PoolInspector>();
            inspector.Initialize(original);

            destroyOnLevelLoad = true; //default is to destroy pool and all its associated GameObjects on level load
            despawnEvent = new UnityEvent();
            destroyEvent = new UnityEvent();
            dontDestroyOnLoadEvent = new UnityEvent();
        }

        //Destructor/Finalize cannot be used to destroy GameObjects as Destroy must be called on the main thread
        public void Cleanup()
        {
            if (parentObjectTransform != null)
            {
                Object.Destroy(parentObjectTransform.gameObject); //destroy parentObjectTransform and all pooled objects that are not children of parentObjectTransform
            }

            destroyEvent.Invoke(); //destroy all pooled objects that are not children of parentObjectTransform
        }

        //Spawn a pooled GameObject with the specified position, rotation, and parent
        public GameObject Spawn(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), Transform parent = null)
        {
            GameObject inst = null;

            if (inactiveObjects.Count > 0) //if the stack has instances in it, try to get one of them
            {
                while (inst == null && inactiveObjects.Count > 0) //ensure we get a valid instance
                {
                    inst = inactiveObjects.Pop();
                }

                if (inst == null) //if there were no valid instances in stack
                {
                    if (inactiveObjects.Count + activeObjectCount >= maxSize)
                    {
                        Log("Tried to Spawn more objects than pool allows.");
                        return null;
                    }

                    inst = createNew(pos, rot, parent);
                }
                else //if we successfully got an instance from the stack, get it ready to use
                {
                    inst.transform.position = pos;
                    inst.transform.rotation = rot;
                    if (parent != null) { inst.transform.parent = parent; }
                }
            }
            else if (inactiveObjects.Count + activeObjectCount >= maxSize)
            {
                Log("Tried to Spawn more objects than pool allows.");
                return null;
            }
            else //if there's an empty stack, Instantiate a new object
            {
                inst = createNew(pos, rot, parent);
            }

            inst.SetActive(true); //Spawn should consistently  return an active GameObject whether original is active or not
            activeObjectCount++;
            return inst;
        }

        //Despawn a pooled GameObject after an optional delay
        public void Despawn(GameObject obj, float time = 0.0F)
        {
            if (original != null)
            {
                if (inactiveObjects.Count + activeObjectCount <= maxSize)
                {
                    obj.SetActive(false);
                    inactiveObjects.Push(obj); 
                }
                else
                {
                    Log("ObjectPool: Despawning to a pool that has more objects than pool allows. Destroying instead.", obj);
                    Object.Destroy(obj);
                }
            }
            else
            {
                LogWarning("ObjectPool: Tried to Despawn() object whose pool no longer exists. Destroying instead", obj);
                Object.Destroy(obj);
            }

            activeObjectCount--;
        }

        //Create the specified number of deactivated pooled GameObjects
        public void Prewarm(int numObjs)
        {
            for (int i = 0; i < numObjs; i++)
            {
                if (inactiveObjects.Count + activeObjectCount >= maxSize)
                {
                    Log("Tried to Spawn more objects than pool allows.");
                    return;
                }

                GameObject inst = createNew();
                Despawn(inst);
            }
        }

        //Sets the capacity of the underlying container for the pool to the number of GameObjects currently in the pool, if that number is less than a threshold value
        public void Trim()
        {
            inactiveObjects.TrimExcess();
        }

        //Despawn all GameObjects associated with the pool
        public void DespawnAll()
        {
            despawnEvent.Invoke();
        }

        //Set this pool's related flag and call DontDestroyOnLoad with all GameObjects associated with the pool if appropriate
        public void DestroyOnLoad(bool destroy)
        {
            destroyOnLevelLoad = destroy;

            if (!destroy)
            {
                Object.DontDestroyOnLoad(parentObjectTransform.gameObject);
                dontDestroyOnLoadEvent.Invoke();
            }
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
            destroyEvent.AddListener(DestroyPooledObject);
            dontDestroyOnLoadEvent.AddListener(DontDestroy);
        }

        //Inform the object's Pool that it has been destroyed
        void OnDestroy()
        {
            if (!pool.inactiveObjects.Contains(gameObject))
            {
                pool.activeObjectCount--;
            }
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
                LogWarning("ObjectPool: Tried to Despawn() object whose pool no longer exists. Destroying instead", gameObject);
                Destroy(gameObject);
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
            Object.DontDestroyOnLoad(transform.root.gameObject);
        }

        //Destroy gameObject when pool's destroyEvent is triggered
        void DestroyPooledObject()
        {
            Destroy(gameObject);
        }
    }
}