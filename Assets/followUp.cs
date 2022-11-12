using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followUp : MonoBehaviour
{
    public GameObject followOrigin;
    Vector3 offset = new Vector3(-1.0f, -1.0f, -1.0f);
    public float poleLength = 1.0f;
    Vector3 prevPosition;
    Quaternion prevRotation;
    float offsetFactor = .98f;
    Vector3 followPoint;
    float timer;

    List<Vector3> followList;

    SteeringSystem steeringSystem;
    void Start()
    {
        timer = 2.0f;
        prevPosition = transform.position;
        prevRotation = transform.rotation;
        steeringSystem = GameObject.FindGameObjectWithTag("system").GetComponent<SteeringSystem>();

        followList = steeringSystem.newPoints;

        if (followList.Count != 0)
        {
            followPoint = followList[0];
        }

    }
    void Update()
    {
        //transform.position = followOrigin.transform.position + offset;

        //  c<----+
        //        +---->c
        //        +     c

        // cam position
        Vector3 lastCamPosition = transform.position;
        Vector3 followPosition = followPoint;

        Vector3 diff = Vector3.ClampMagnitude(lastCamPosition - followPosition, poleLength);
        transform.position = followPosition + diff;
        transform.position = Vector3.Lerp(transform.position, prevPosition, offsetFactor);

        //cam rotation
        Quaternion rotation = Quaternion.LookRotation(-diff, Vector3.up);
        transform.rotation = Quaternion.Slerp(rotation, prevRotation, offsetFactor);

        prevPosition = transform.position;
        prevRotation = transform.rotation;

        timer -= Time.deltaTime;
        if(timer <= 0.0f)
        {
            timer = 2.0f;
        }

        followList = steeringSystem.newPoints;
        if (followList.Count != 0)
        {
            followPoint = followList[0];
        }
    }
}
