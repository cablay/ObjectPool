﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour {

    public GameObject prefab;
    public GameObject instance;
    public GameObject instance2;

    class FakeClass : MonoBehaviour { }

	void Start ()
    {
        //StartCoroutine(SpawnDestroyedObject());
        Tests();
        //StartCoroutine(DestroyOnLoadTests());
	}

    IEnumerator DespawnBadObj()
    {
        instance = ObjectPool.Spawn(prefab);

        yield return new WaitForSeconds(1f);

        Destroy(instance);
        ObjectPool.Despawn(instance);

        yield return new WaitForSeconds(1f);

        ObjectPool.Despawn(instance);
        Debug.Log(instance);
    }

    IEnumerator SpawnDestroyedObject()
    {
        instance = (GameObject)Object.Instantiate(prefab);
        instance.name = "Instance";
        Transform it = instance.transform;
        GameObject obj = ObjectPool.Spawn(instance);

        Destroy(instance);

        yield return new WaitForSeconds(1f);


        ObjectPool.Despawn(obj);

        yield return null;
    }

    void TryToDestroyPrefab()
    {
        ObjectPool.Despawn(prefab);
    }

    void TryToDestroyNull()
    {
        Destroy(null);
    }

    void BadGenericInstantiate()
    {
        //FakeClass t = ObjectPool.Spawn(new FakeClass());
        //t.position = new Vector3(1f, 1f, 1f);
    }

    void Tests()
    {
        StartCoroutine(SpawnDespawn());
    }

    IEnumerator DestroyOnLoadTests()
    {
        DontDestroyOnLoad(instance);

        yield return new WaitForSeconds(2f);

        //SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
        instance.transform.parent = instance2.transform;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("TestScene 1");
    }

    IEnumerator SpawnDespawn()
    {
        GameObject obj = ObjectPool.Spawn(instance);
        //GameObject obj2 = ObjectPool.Spawn(obj);

        //Dictionary<GameObject, int> table = new Dictionary<GameObject, int>();
        //table.Add(obj, 1);
        //Destroy(obj);

        yield return new WaitForSeconds(1f);


        //table.Remove((GameObject)null);



        ObjectPool.Despawn(obj);
        //ObjectPool.DontDestroyOnLoad(prefab);

        //yield return new WaitForSeconds(3f);

        //SceneManager.LoadScene("TestScene 1");
    }
}