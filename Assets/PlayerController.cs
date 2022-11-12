using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    float horizontal;
    float vertical;
    float anglespeed = 1.0f;
    void Start()
    {
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Quaternion quaternion = Quaternion.Euler(vertical * anglespeed, horizontal * anglespeed, 0.0f);


        transform.position += transform.forward * speed * Time.deltaTime;
        transform.rotation *= quaternion;
        // transform.rotation = new Quaternion(transform.rotation.x + horizontal, transform.rotation.y + vertical, transform.rotation.z, transform.rotation.w);
    }
}
