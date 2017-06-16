using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Spawner : MonoBehaviour
{
    [SerializeField] CubeController cube;
    [SerializeField] float spawnTime;

    bool useObjectPool = true;
    float time = 0.0f;

	void FixedUpdate ()
    {
        time += Time.fixedDeltaTime;

        if (time > spawnTime)
        {
            time = 0.0f;
            
            CubeController cc = useObjectPool ? ObjectPool.Spawn(cube, transform.position, transform.rotation) : Instantiate(cube, transform.position, transform.rotation);
            cc.useObjectPool = useObjectPool;
        }
	}

    //Called via UnityEvent when the button is pressed
    public void ToggleObjectPool()
    {
        useObjectPool = !useObjectPool;
    }
}