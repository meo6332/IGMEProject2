using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// This class is used to implement the functionality needed for a projectile
/// moving around the screen, and both spawning and despawning the projectile.
/// </summary>
public class Projectile : MonoBehaviour
{
    // Variables related to the projectile's movement
    [SerializeField]
    Vector3 bulletPosition = Vector3.zero;

    [SerializeField]
    Vector3 direction;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    float bulletSpeed = 2.0f;

    public bool fired = false;

    // Set up the variables needed for disabling at screen edge
    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    // Set up the variable needed to communicate with projectile manager
    ProjectileManager manager;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the camera details
        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        // Find the projectile manager
        manager = FindObjectOfType<ProjectileManager>();

        // Set the bullet's default position
        transform.position = new Vector3(totalCamWidth * 3, totalCamHeight * 3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            // Need to move the bullet
            bulletPosition = bulletPosition + (velocity * Time.deltaTime);

            wrapBullet();

            transform.position = bulletPosition;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    /// <summary>
    /// This function handles whenever the bullet is fired, setting it's new direction
    /// and velocity, as well as setting flags to allow it to move.
    /// </summary>
    /// <param name="newDirection">
    /// The new direction that the bullet will be traveling in.
    /// </param>
    /// <param name="newPosition">
    /// The updated initial position of the bullet it will start moving from
    /// </param>
    public void fireBullet(Vector3 newDirection, Vector3 newPosition)
    {
        // First update the bullet to be positioned at the ship
        bulletPosition = newPosition;

        this.transform.position = bulletPosition;

        // Make the direction for the bullet and determine the constant velocity
        direction = newDirection;

        velocity = direction * bulletSpeed;

        // Set the firing flag to true
        fired = true;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }


    /// <summary>
    /// This function handles despawning the bullet, stopping it from moving,
    /// moving it offscreen, and then returning it to the ProjectileManager's queue of bullets.
    /// </summary>
    public void despawnSelf()
    {
        // This function will despawn the bullet and return it to the manager's queue

        fired = false;
        // Put the bullet offscreen so that issues don't arrise from it colliding with asteroids
        transform.position = new Vector3(totalCamWidth * 3, totalCamHeight * 3, 0);
        manager.ReturnBullet(this.gameObject);

    }


    /// <summary>
    /// This function checks for if the bullet has collided with the edge of the screen, and
    /// if so, it will despawn the bullet.
    /// </summary>
    private void wrapBullet()
    {
        bool despawnCheck = false;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        // Wrap horizontally
        if (bulletPosition.x < -halfWidth)
        {
            despawnCheck = true;
        }
        else if (bulletPosition.x > halfWidth)
        {
            despawnCheck = true;
        }

        // Wrap Vertically
        if (bulletPosition.y < -halfHeight)
        {
            despawnCheck = true;
        }
        else if (bulletPosition.y > halfHeight)
        {
            despawnCheck = true;
        }

        if (despawnCheck)
        {
            // If this is true then the bullet needs to get returned to the manager's queue
            despawnSelf();       
            
        }
    }
}
