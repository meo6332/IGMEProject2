using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleDeltaTimeBroken : MonoBehaviour
{

    [SerializeField]
    Vector3 direction = Vector3.right;

    [SerializeField]
    Vector3 vehiclePosition = Vector3.zero;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    Vector3 acceleration = Vector3.zero;

    [SerializeField]
    float decelerationValue = 0.2f;

    [SerializeField]
    float turnSpeed = 1f;

    [SerializeField]
    float accelerationRate = 0.01f;

    [SerializeField]
    float maximumSpeed = 0.1f;

    [SerializeField]
    float minSpeed = 0.01f;

    [SerializeField]
    Vector2 playerInput;


    // Set up the variables needed for wrapping
    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    // Start is called before the first frame update
    void Start()
    {
        vehiclePosition = transform.position;

        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.x > 0)
        {
            // trying to move right
            // change turn speed negative
            direction = Quaternion.Euler(0, 0, (-turnSpeed) * Time.deltaTime) * direction;
        }
        else if (playerInput.x < 0)
        {
            // trying to move left
            // change turn speed positive
            direction = Quaternion.Euler(0, 0, turnSpeed * Time.deltaTime) * direction;
        }

        // Want to calculate where the new position should be 
        // Multiply direction vector by the speed, DIRECTION NEEDS BE NORMALIZED


        if (playerInput.y > 0)
        {
            acceleration = direction * accelerationRate;

            velocity = velocity + (acceleration * Time.deltaTime);
        }
        else if (playerInput.y <= 0)
        {
            velocity = velocity * (1 - (decelerationValue * Time.deltaTime));

            if (velocity.magnitude < minSpeed)
            {
                velocity = Vector3.zero;
            }
        }

        velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);

        // Draw the vehicle at this rotation
        vehiclePosition = vehiclePosition + (velocity * Time.deltaTime);

        // Have the vehicle's position now, but check if it needs to wrap
        wrapVehicle();

        transform.position = vehiclePosition;

        // Rotate the vehicle to match our direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        acceleration = Vector3.zero;
    }

    public void OnMove(InputValue value)
    {
        // Make sure to include UnityEngine.InputSystem for this

        // Make sure that Get<type> is the correct type the action is sending 
        // It will show that under properties, and action type 
        playerInput = value.Get<Vector2>();


    }

    private void wrapVehicle()
    {
        float offset = 0.2f;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        // Wrap horizontally
        if (vehiclePosition.x < -halfWidth)
        {
            // Out of bounds on left, wrap to the right
            vehiclePosition.x = halfWidth - offset;
        }
        else if (vehiclePosition.x > halfWidth)
        {
            // Out of bounds on right, wrap to the left
            vehiclePosition.x = -halfWidth + offset;
        }

        // Wrap Vertically
        if (vehiclePosition.y < -halfHeight)
        {
            // Out of bounds on bottom, wrap to the top
            vehiclePosition.y = halfHeight - offset;
        }
        else if (vehiclePosition.y > halfHeight)
        {
            // Out of bounds on top, wrap to the bottom
            vehiclePosition.y = -halfHeight + offset;
        }
    }
}
