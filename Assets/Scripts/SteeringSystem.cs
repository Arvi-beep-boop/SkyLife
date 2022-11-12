using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
    Boid one of the birds:
        - position
        - velocity

 Steering Behaviors:
    - alignment - fly the same direction as neighbors: take neighbors' velocity, make average and add as your velocity
    - cohesion - fly in flocks/schools: take neighbors' positions, make average and go there
    - separation - avoid neighbors: take vector from neighbor and the closer you are, the more you move away

    v += v1 + v2 + v3;
    clamp(v); min-max
    p += v * t;

    1. Just move the boid - but with ECS (entity component system) approach

    entity - bird / fish / crab, just game object containing components
    component - pure data, pure member fields, struct, no logic, no behavior code, no update, no start, no methods at all
    system - pure logic, methods, no data

    for i:
        v[i] += v1[i] + v2[i] + v3[i];
        clamp(v[i]); min-max
        p[i] += v[i] * t;
 */

public class SteeringSystem : MonoBehaviour
{

    public Vector3 boundMin = new Vector3(-1.0f, -1.0f, -1.0f);
    public Vector3 boundMax = new Vector3(1.0f, 1.0f, 1.0f);
    Vector3 pointPosition;
    Boid player;

    // points list

    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> newPoints = new List<Vector3>();
    float timer;


    void Start()
    {
        pointPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Boid>();
        points.Add(player.transform.position);
        timer = .1f;
    }

    void Update()
    { 
       

        // boid properties

        float separationDistance = 5.0f;
        float visibilityDistance = 5.0f;
        float fovFactor = 0.9f;
        float alignment = 0.1f;
        float minSpeed = 3.0f;
        float maxSpeed = 8.0f;
        float cohesionFactor = 0.1f;
        float wanderFactor = 0.02f;
        float leaderWanderFactor = 10.0f;
        float maxBoidHeight = 50.0f;
        float minBoidHeight = 20.0f;

        // terrain properties

        float terrainFactor  = 400.0f; //specifies distance from x =.0f and z.0f, border of the world

        // points system

        if (timer <= 0.0f)
        {
            points.Add(player.transform.position);
            timer = .1f;
            if (points.Count > 30)
            {
                points.Remove(points[0]);
            }
        }

        newPoints = resultPoints(points, 0.1f, timer);

        for (int i = 0; i < newPoints.Count - 1; i++)
        {
           // Debug.DrawLine(newPoints[i], newPoints[i + 1], Color.magenta);
        }

        Boid[] boids = GameObject.FindObjectsOfType<Boid>();

        for (int boIndex = 0; boIndex < boids.Length;  boIndex++)
        {
            Boid bo = boids[boIndex];
            Vector3 avgPosition = Vector3.zero;
            Vector3 avgVelocity = Vector3.zero;



            int counter = 0;
            
            // iterate over neighbours
            foreach (Boid ne in boids)
            {
             
                // ignore myself
                if (bo == ne)
                    continue;

                Vector3 boidsDistance = ne.transform.position - bo.transform.position;
                float boidMag = boidsDistance.magnitude;
                if (boidMag >= visibilityDistance && Vector3.Dot(boidsDistance.normalized, bo.velocity.normalized) < fovFactor)
                {
                    continue;
                }
                // separation
                float sepFactor = separationDistance;
                if(boidMag < sepFactor)
                {
                    float sepForce = -1.0f *unifs(boidMag/sepFactor) + 1.0f;
                    Vector3 sepVec = -boidsDistance.normalized * sepForce;
                    bo.velocity += sepVec;
                    
                }

                counter++;
                avgPosition += ne.transform.position;
                avgVelocity += ne.velocity;
                
            }

            // wander
            if (bo.gameObject.tag != "Player")
            {
                float wanderMagnitude = bo.velocity.magnitude * wanderFactor * (counter == 0 ? leaderWanderFactor : 1.0f);
                Vector3 myRandomVector = Random.onUnitSphere * wanderMagnitude; //new Vector3(Random.Range(-wanderMagnitude, wanderMagnitude), Random.Range(-wanderMagnitude, wanderMagnitude), Random.Range(-wanderMagnitude, wanderMagnitude));
                bo.velocity += myRandomVector;
            }

            //calculate average
            if(counter != 0)
            {
                avgPosition /= counter;
                avgVelocity /= counter;

                // cohesion
               /* if (bo.gameObject.tag != "Player")
                {
                    bo.velocity += Vector3.ClampMagnitude(avgPosition - bo.transform.position, cohesionFactor);
                }
                */

                // alignment
                if (bo.gameObject.tag != "Player")
                {
                   bo.velocity += Vector3.ClampMagnitude(avgVelocity, alignment);
                }

            }
            if (newPoints.Count != 0)
            {
                int followUpPoint = boIndex % newPoints.Count;
                Vector3 myPoint = newPoints[followUpPoint];
                Vector3 PointBoPositionDiff = myPoint - bo.transform.position;

                /* if (Vector3.Dot(PointBoPositionDiff.normalized, player.velocity.normalized) <= 0.9f)
                 {
                     PointBoPositionDiff = pointPosition - bo.transform.position;
                 }*/
                if (bo.gameObject.tag != "Player")
                {
                    bo.velocity += Vector3.ClampMagnitude(PointBoPositionDiff, 1.0f);
                   // Debug.DrawLine(bo.transform.position, myPoint);
                }
            }
            // avoid terrain and sky limit
            float boidHeight = bo.transform.position.y - Terrain.activeTerrain.SampleHeight(bo.transform.position);
            if (boidHeight <= minBoidHeight)
            {
                float atForce = -1.0f * unifs(boidHeight / minBoidHeight) + 1.0f; 
                bo.velocity.y += atForce;
            }
            if (boidHeight >= maxBoidHeight)
            {
                float atForce = unifs(Mathf.Clamp((boidHeight - maxBoidHeight) / 5.0f, 0.0f, 1.0f));
                bo.velocity.y -= atForce;
            }

            // dont go off terrain
            Vector3 boidPosition = bo.transform.position;
            if(boidPosition.x >= terrainFactor)
            {
                float xForce = unifs(Mathf.Clamp((boidPosition.x - terrainFactor) / 10.0f, 0.0f, 1.0f));
                bo.velocity.x -= xForce;
            }
            if (boidPosition.x <= -terrainFactor)
            {
                float xForce = -1.0f * unifs(Mathf.Clamp((boidPosition.x + terrainFactor) / 10.0f, 0.0f, 1.0f)) + 1.0f;
                bo.velocity.x += xForce;
            }
            if (boidPosition.z >= terrainFactor)
            {
                float zForce = unifs(Mathf.Clamp((boidPosition.z - terrainFactor) / 10.0f, 0.0f, 1.0f));
                bo.velocity.z -= zForce;
            }
            if (boidPosition.z <= -terrainFactor)
            {
                float zForce = -1.0f * unifs(Mathf.Clamp((boidPosition.z + terrainFactor) / 10.0f, 0.0f, 1.0f)) + 1.0f;
                bo.velocity.z += zForce;
            }


            if (bo.gameObject.tag == "Player")
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                Vector3 velocityP = new Vector3(horizontal, vertical, 0.0f).normalized;
                velocityP = bo.transform.rotation * velocityP;
                bo.velocity += velocityP;
            }

            // update boid final position
            if (bo.velocity.magnitude <= minSpeed)
            {
                bo.velocity = bo.velocity.normalized * minSpeed;
            }
            
            bo.velocity = Vector3.ClampMagnitude(bo.velocity, maxSpeed);
            bo.transform.position += bo.velocity * Time.deltaTime;
            Quaternion newRotation = Quaternion.LookRotation(bo.velocity.normalized, Vector3.up);
            bo.rotation = newRotation;
            bo.transform.rotation = Quaternion.Slerp(newRotation, bo.transform.rotation, 0.99f);
            // bo.transform.rotation = Quaternion.LookRotation(bo.velocity.normalized, Vector3.up);

        }

        pointPosition = Vector3.ClampMagnitude(pointPosition - player.transform.position, 20.0f) + player.transform.position;
        timer -= Time.deltaTime;

        

    }
    float unifs(float x)
    {
        return x * x / (2 * (x * x - x) + 1);
    }

    public List<Vector3> resultPoints(List<Vector3> pointList, float interval, float timer)
    {
        float alpha = timer / interval;
        List<Vector3> result = new List<Vector3>();

        for(int i = 0; i < pointList.Count - 1; i++)
        {
            result.Add(pointList[i] * alpha + pointList[i + 1] * (1 - alpha));
        }
        return result;
    }
}

