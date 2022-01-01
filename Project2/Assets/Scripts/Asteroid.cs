using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField]
    GameObject childPrefab;

    [SerializeField]
    Vector3 asteroidPosition;

    [SerializeField]
    Vector3 direction;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    float speed = 2.0f;

    // Used to say if this asteroid object is in motion or not
    public bool moving = false;

    // Set up the variables needed for Wrapping
    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    // The below variables are given by the asteroid spawner
    [SerializeField]
    int arrayIndex;

    [SerializeField]
    AsteroidSpawner spawner;

    // Exists to say if the asteroid is a child asteroid or not.
    // If this is true, the asteroid is a child asteroid.
    public bool childFlag;


    void Awake()
    {
        moving = false;

        speed = 2.0f;

        direction = Vector3.right;

        // Get the camera info
        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        // Set an initial position to prevent false collisions at start
        transform.position = new Vector3(totalCamWidth * 2, totalCamHeight * 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // If I am moving, then I need to run the calculations of moving the asteroid
            asteroidPosition = asteroidPosition + (velocity * Time.deltaTime);

            wrapAsteroid();

            // Check if, after calling the wrap function, we still need to move
            if (moving)
            {
                transform.position = asteroidPosition;
            }
        }
    }


    /// <summary>
    /// Sets up this asteroid's variables that deal with it's connection
    /// to the AsteroidSpawner that made it
    /// </summary>
    /// <param name="index">
    /// The index of this asteroid in the asteroidList
    /// </param>
    /// <param name="mySpawner"> 
    /// The script that created this asteroid
    /// </param>
    public void connectToSpawner(int index, AsteroidSpawner mySpawner)
    {
        arrayIndex = index;
        spawner = mySpawner;
    }


    /// <summary>
    /// This is called to handle whenever the asteroid is spawned, setting up
    /// it's random angle to travel at while setting it at its new position.
    /// </summary>
    /// <param name="newX">
    /// The x coordinate of the asteroid's new position
    /// </param>
    /// <param name="newY">
    /// The y coordinate of the asteroid's new position
    /// </param>
    public void spawnSelf(float newX, float newY)
    {
        // First set the new position for this asteroid
        asteroidPosition.x = newX;
        asteroidPosition.y = newY;

        // Generate the random angle and set it equal to the direction
        Vector2 randomAngle = Random.insideUnitCircle.normalized;

        direction.x = randomAngle.x;
        direction.y = randomAngle.y;

        // Have the direction, now get the velocity based on that
        velocity = direction * speed;

        // Set this to be moving, and set self to active
        moving = true;

    }


    /// <summary>
    /// Handles the case of the asteroid being despawned.
    /// </summary>
    /// <param name="bulletHit">
    /// If true, then the asteroid despawned because it collided with a bullet.
    /// </param>
    public void despawnSelf(bool bulletHit)
    {
        // Function handling the asteroid needing to be despawned
        moving = false;

        spawner.despawnAsteroid(this.gameObject, bulletHit);
    }


    /// <summary>
    /// This is the function that handles the case of a child asteroid spawning itself and beginning to move.
    /// It sets up the needed variables based on the given direction and position.
    /// </summary>
    /// <param name="newDirection">
    /// The direction the asteroid will now be moving in.
    /// </param>
    /// <param name="newPosition">
    /// The initial position that the asteroid will appear at before moving 
    /// </param>
    public void childSpawn(Vector3 newDirection, Vector3 newPosition)
    {
        // This function exists for when a child spawns itself 
        direction = newDirection;
        asteroidPosition = newPosition;

        velocity = direction * speed;

        moving = true;
    }


    /// <summary>
    /// This function handles the behavior of the asteroid splitting itself, if it
    /// is not a child asteroid.
    /// </summary>
    public void splitSelf()
    {
        // This function handles splitting the asteroid on collision with a bullet

        if (!childFlag)
        {
            // SPLITTING FUNCTIONALITY

            // First, generate the random angles for the two children
            var angle = Random.Range(-35f, 35f);
            var quaternion = Quaternion.Euler(0, 0, angle);

            var firstDirection = quaternion * direction;

            angle = Random.Range(-35f, 35f);
            quaternion = Quaternion.Euler(0, 0, angle);

            var secondDirection = quaternion * direction;

            // Now instantiate the new children
            GameObject firstChild = Instantiate(childPrefab, asteroidPosition, Quaternion.identity);
            firstChild.GetComponent<Asteroid>().connectToSpawner(-1, spawner);
            firstChild.GetComponent<Asteroid>().childSpawn(firstDirection, asteroidPosition);

            spawner.addToCollision(firstChild);

            GameObject secondChild = Instantiate(childPrefab, asteroidPosition, Quaternion.identity);
            secondChild.GetComponent<Asteroid>().connectToSpawner(-1, spawner);
            secondChild.GetComponent<Asteroid>().childSpawn(secondDirection, asteroidPosition);

            spawner.addToCollision(secondChild);

        }

    }


    /// <summary>
    /// Handles the behavior that occurs whenever the asteroid hits the edge of the screen.
    /// If it has hit the edge of the screen, the asteroid will despawn without splitting.
    /// </summary>
    private void wrapAsteroid()
    {
        float offset = 1.0f;
        bool despawnCheck = false;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        // Wrap horizontally
        if (asteroidPosition.x < -halfWidth - offset)
        {
            despawnCheck = true;
        }
        else if (asteroidPosition.x > halfWidth + offset)
        {
            despawnCheck = true;
        }

        // Wrap Vertically
        if (asteroidPosition.y < -halfHeight - offset)
        {
            despawnCheck = true;
        }
        else if (asteroidPosition.y > halfHeight + offset)
        {
            despawnCheck = true;
        }

        if (despawnCheck)
        {
            // If this is true then the bullet needs to get returned to the manager's queue
            despawnSelf(false);
            
        }
    }

}
