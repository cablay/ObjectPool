using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnThings : MonoBehaviour {

    public GameObject[] prefabs;

    void Start()
    {
        StartCoroutine("Spawn");
    }
	
	IEnumerator Spawn ()
    {
        while (true)
        {
            GameObject obj = ObjectPool.Spawn(prefabs[Random.Range(0, prefabs.Length - 1)], transform.position, transform.rotation);
            StartCoroutine("Remove", obj);

            yield return new WaitForSeconds(0.01f);
        }
	}

    IEnumerator Remove(GameObject obj)
    {
        yield return new WaitForSeconds(3f);

        ObjectPool.Despawn(obj);

        yield return null;
    }
}
