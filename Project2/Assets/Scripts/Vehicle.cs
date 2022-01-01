using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


/// <summary>
/// This class implements the behavior used for the ship in the asteroids game,
/// including its movement, its firing, its ability to wrap around the screen, 
/// and it taking damage. 
/// </summary>
public class Vehicle : MonoBehaviour
{
    // The below values all relate to the movement of the vehicle
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

    // Fields needed for setting up a limit on the bullet rate of fire
    [SerializeField]
    float maxBulletDelay;

    private float currentBulletTime;

    // Fields used for tracking the health and the time when the ship shouldn't be hit
    public int health;

    [SerializeField]
    float maxInincibilityTime;

    public float invincibilityTimer;

    [SerializeField]
    Text livesText;

    // These last variables are needed to work with the spawn pool and shooting bullets
    private ProjectileManager bulletManager;

    // Variable for making the ship be counted as destroyed
    public bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        currentBulletTime = maxBulletDelay;

        vehiclePosition = transform.position;

        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        bulletManager = FindObjectOfType<ProjectileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
        {
            // Do not allow the ship to move if it is destroyed
            return;
        }

        // Decrement the timer used for checking bullet delay
        currentBulletTime -= Time.deltaTime;

        // Decrement the timer used for the ship's invincibility frames
        invincibilityTimer -= Time.deltaTime;
        // If the timer is less than zero then change the ship's color back to normal
        if(invincibilityTimer <= 0)
        {
            this.gameObject.GetComponent<SpriteInfo>().changeColor(Color.white);   
        }

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

    /// <summary>
    /// Used to get the value of the player turning left and right 
    /// </summary>
    /// <param name="value">
    /// Gives the vector associated with the player's movement inputs
    /// </param>
    public void OnMove(InputValue value)
    {
        // Make sure to include UnityEngine.InputSystem for this

        // Make sure that Get<type> is the correct type the action is sending 
        // It will show that under properties, and action type 
        playerInput = value.Get<Vector2>();


    }


    /// <summary>
    /// Implements the behavior of checking if the ship is out of bounds, and wrapping 
    /// it around the screen if it is
    /// </summary>
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


    /// <summary>
    /// Implements the ability for the ship to shoot bullets.
    /// Also implements the delay in the ship shooting bullets
    /// </summary>
    /// <param name="value">
    /// Unused
    /// </param>
    public void OnFire(InputValue value)
    {
        if (currentBulletTime <= 0 && !destroyed)
        {
            // Value has no value.  Just deal with spawning a bullet at the right place
            GameObject bullet = bulletManager.GetBullet();

            // Get the projectile script from the bullet
            Projectile currentProjectile = bullet.GetComponent<Projectile>();
            currentProjectile.fireBullet(direction, vehiclePosition);

            // Reset the bullet timer 
            currentBulletTime = maxBulletDelay;
        }
    }

    /// <summary>
    /// Deals with the case whenever the ship is hit by an asteroid, making it lose health
    /// and activating a brief period of invincibility.
    /// </summary>
    public void damageShip()
    {
        // Called whenever the ship get's hit by an asteroid
        if (invincibilityTimer <= 0)
        {           
            health--;

            Debug.Log("Ship has been hit.  Current Health: " + health);

            // Reset the invincibility timer
            invincibilityTimer = maxInincibilityTime;

            // Update the UI to reflect the new life count
            string healthText = health.ToString();
            livesText.text = "Lives: " + healthText;

        }

        if(health <= 0)
        {
            // The ship now needs to get destroyed
            destroyed = true;

            transform.position = new Vector3(totalCamWidth * 4, totalCamHeight * 4, 0);
        }
        
    }
}
