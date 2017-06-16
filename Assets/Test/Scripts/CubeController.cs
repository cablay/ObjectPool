using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public bool useObjectPool;

    [SerializeField] float force;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color.HSVToRGB(Random.value, 1f, 1f);

        //Simulate expenseive Instantiation-time operations
        FindObjectsOfType<CubeController>();
    }

    void OnEnable()
    {
        rb.velocity = transform.up * force; //setting the velocity directly for simplicity
    }

    void OnTriggerEnter(Collider other)
    {
        if (useObjectPool)
        {
            ObjectPool.Despawn(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
