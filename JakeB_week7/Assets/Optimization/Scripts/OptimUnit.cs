using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class OptimUnit : MonoBehaviour
{
    private Vector3 currentVelocity;
    private float timeToVelocityChange;
    private float currentAngularVelocity;
    private float timeToAngularVelocityChange;

    private Vector3 areaSize;

    private float rotationUpdateInterval = 1f; // Interval for rotation updates
    private float borderCheckUpdateInterval = 0.5f;
    private float rotationTimer = 0f;
    private float borderTimer = 0f;

    public void SetAreaSize(Vector3 size)
    {
        areaSize = size;
    }

    private void Start()
    {
        PickNewVelocity();
        PickNewVelocityChangeTime();
        PickNewAngularVelocity();
        PickNewAngularVelocityChangeTime();
    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("Handle Time");
        HandleTime();
        Profiler.EndSample();

        Profiler.BeginSample("Handle Move");
        Move();
        Profiler.EndSample();

        Profiler.BeginSample("Handle Rotation");
        HandleRotation();
        Profiler.EndSample();

        Profiler.BeginSample("Handle border detection");
        HandleBorderDetection();
        Profiler.EndSample();
    }

    private void LateUpdate() {

    }


    private void PickNewVelocity()
    {
        currentVelocity = Random.insideUnitSphere;
        currentVelocity.y = 0;
        currentVelocity *= 10.0f;
    }

    private void PickNewAngularVelocity()
    {
        currentAngularVelocity = Random.Range(-180.0f, 180.0f);
    }

    private void PickNewVelocityChangeTime()
    {
        timeToVelocityChange = Random.Range(2.0f, 5.0f);
    }

    private void PickNewAngularVelocityChangeTime()
    {
        timeToAngularVelocityChange = Random.Range(2.0f, 5.0f);
    }

    void HandleRotation() {
        // Update the rotation only after the interval passes
        rotationTimer += Time.deltaTime;
        if (rotationTimer >= rotationUpdateInterval) {
            var t = transform;

            if (transform.position.x <= 0)
                transform.Rotate(currentAngularVelocity * Time.deltaTime, 0, 0);
            else if (transform.position.x > 0)
                transform.Rotate(-currentAngularVelocity * Time.deltaTime, 0, 0);

            if (transform.position.z >= 0)
                transform.Rotate(0, 0, currentAngularVelocity * Time.deltaTime);
            else if (transform.position.z < 0)
                transform.Rotate(0, 0, -currentAngularVelocity * Time.deltaTime);
            // Face the direction of movement
            FaceMovementDirection();
            // Reset the timer after updating
            rotationTimer = 0f;
        }
    }

    void FaceMovementDirection() {
        // rotate to face movement
        if (currentVelocity.sqrMagnitude > 0.01f) {
            // Instantly set the rotation to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            transform.rotation = targetRotation;
        }
    }

    void HandleBorderDetection() {
        borderTimer += Time.deltaTime;
        if (borderTimer >= borderCheckUpdateInterval) {             //check if we are moving away from the zone and invert velocity if this is the case
            if (transform.position.x > areaSize.x && currentVelocity.x > 0) {
                currentVelocity.x *= -1;
                PickNewVelocityChangeTime(); //we pick a new change time as we changed velocity
            } else if (transform.position.x < -areaSize.x && currentVelocity.x < 0) {
                currentVelocity.x *= -1;
                PickNewVelocityChangeTime();
            }

            if (transform.position.z > areaSize.z && currentVelocity.z > 0) {
                currentVelocity.z *= -1;
                PickNewVelocityChangeTime(); //we pick a new change time as we changed velocity
            } else if (transform.position.z < -areaSize.z && currentVelocity.z < 0) {
                currentVelocity.z *= -1;
                PickNewVelocityChangeTime();
                // Reset the timer after updating
                borderTimer = 0f;
            }
        }
    }

    void Move()
    {
        transform.position = transform.position + currentVelocity * Time.deltaTime;

/*        Vector3 position = transform.position;

        float distanceToCenter = Vector3.Distance(Vector3.zero, position);
        float speed = 0.5f + distanceToCenter / areaSize.magnitude;

        int steps = Random.Range(1000, 2000);
        float increment = Time.deltaTime / steps;
        for (int i = 0; i < steps; ++i) {
            position += currentVelocity * increment * speed;
        }
        
        transform.position = position;*/
    }

    private void HandleTime()
    {
        timeToVelocityChange -= Time.deltaTime;
        if (timeToVelocityChange < 0)
        {
            PickNewVelocity();
            PickNewVelocityChangeTime();
        }

        timeToAngularVelocityChange -= Time.deltaTime;
        if (timeToAngularVelocityChange < 0)
        {
            PickNewAngularVelocity();
            PickNewAngularVelocityChangeTime();
        }
    }
}