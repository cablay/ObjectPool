using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleObjectPoolEvent : UnityEvent { }

public class CreateSpawners : MonoBehaviour
{
    [SerializeField] Spawner spawner;
    [SerializeField] int numSpawners;

    ToggleObjectPoolEvent toggleObjectPoolEvent;

    void Awake()
    {
        toggleObjectPoolEvent = new ToggleObjectPoolEvent();

        //Create a variable number of Spawners in an evenly-spaced circle around this object's position
        for (int i = 0; i < numSpawners; i++)
        {
            Spawner spawnerInstance = Instantiate(spawner, transform, true);
            toggleObjectPoolEvent.AddListener(spawnerInstance.ToggleObjectPool); //allow spawners to listen to button press
            spawnerInstance.transform.RotateAround(transform.position, Vector3.up, i * (360 / numSpawners)); //spacing
        }
    }

    public void ToggleObjectPool()
    {
        toggleObjectPoolEvent.Invoke();
    }
}
