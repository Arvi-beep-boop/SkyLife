using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private void Start()
    {
        velocity = Random.onUnitSphere * 4.0f;
        Quaternion rotation = transform.rotation;
    }
    public Vector3 velocity = new Vector3(1.0f, 0.0f, 0.0f);
    public Quaternion rotation;
}
