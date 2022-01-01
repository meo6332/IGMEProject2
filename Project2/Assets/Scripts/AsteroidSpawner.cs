using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject asteroidPrefab;

    private Queue<GameObject> asteroidPool = new Queue<GameObject>();

    // This is the amount of parent asteroids there should be
    [SerializeField]
    int asteroidTotal;

    [SerializeField]
    private int asteroidCount;

    // These variables deal with the delta time and timer for spawning asteroids
    [SerializeField]
    float initialTimerValue;

    private float currentTimer;

    // Set up the variables needed for spawning on the edge of the screen
    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    // The object that's needed so the spawner knows when to tell it to add new asteroids to collision
    [SerializeField]
    GameObject collisionManager;


    // The possible Sprites that the asteroids can be
    [SerializeField]
    Sprite spriteAsteroid1;

    [SerializeField]
    Sprite spriteAsteroid2;

    [SerializeField]
    Sprite spriteAsteroid3;


    // Start is called before the first frame update
    void Awake()
    { 

        // Get the camera info
        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        Vector3 offscreenPosition = new Vector3(totalCamWidth * 2, totalCamHeight * 2, 0);

        // Set the current timer equal to inital time to start 
        currentTimer = initialTimerValue;

        // Begin populating the asteroids list
        for (int i = 0; i < asteroidTotal; i++)
        {
            GameObject current = Instantiate(asteroidPrefab, offscreenPosition, Quaternion.identity);

            current.GetComponent<Asteroid>().connectToSpawner(i, this);

            current.GetComponent<Asteroid>().childFlag = false;

            current.SetActive(false);

            asteroidPool.Enqueue(current);
        }

        // Set the initial count of asteroids to 0
        asteroidCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Use a float that I subtract deltatime from the timer 
        if(currentTimer <= 0)
        {
            // Spawn an asteroid in here using spawnAsteroid if there's less than the max amount
            if(asteroidPool.Count > 0)
            {
                // If you get here, then there's at least one asteroid that can be spawned
                GameObject current = asteroidPool.Dequeue();
                spawnAsteroid(current);
                
            }

            currentTimer = initialTimerValue;
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }

    }


    /// <summary>
    /// Handles the functionality needed whenever spawning a given asteroid.
    /// It will set the asteroid to be active, and generate it's random position along
    /// the edges of the screen.
    /// </summary>
    /// <param name="current">
    /// The asteroid gameObject to spawn.
    /// </param>
    public void spawnAsteroid(GameObject current)
    {
        Asteroid asteroidScript = current.GetComponent<Asteroid>();

        current.SetActive(true);

        // Decide which of the four bounds the asteroid will be sent from
        float whichSide = Random.Range(0.0f, 1.0f);

        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;
        float offset = 0.2f;
        float secondOffset = 1.0f;
        float newY;
        float newX;

        if(whichSide < 0.25)
        {
            // Means you're spawning from the top
            newY = halfHeight - offset;
            newX = Random.Range(-halfWidth + secondOffset, halfWidth - secondOffset);
        }
        else if(whichSide < 0.5)
        {
            // Means you're spawning from the right
            newY = Random.Range(-halfHeight + secondOffset, halfHeight - secondOffset);
            newX = halfWidth + offset;
        }
        else if(whichSide < 0.75)
        {
            // Means you're spawning from the bottom
            newY = -halfHeight + offset;
            newX = Random.Range(-halfWidth + secondOffset, halfWidth - secondOffset);
        }
        else
        {
            // means you're spawning from the left
            newY = Random.Range(-halfHeight + secondOffset, halfHeight - secondOffset);
            newX = halfWidth + offset;
        }

        changeAsteroidSprite(current);

        asteroidScript.spawnSelf(newX, newY);

        // Increment the asteroid count
        asteroidCount++;
    }


    /// <summary>
    /// Handles the behavior of despawning an asteroid, disabling it and adding it
    /// back to the asteroid pool if it is not a child.
    /// </summary>
    /// <param name="current">
    /// The asteroid to despawn.
    /// </param>
    /// <param name="bulletHit">
    /// A flag determining if the asteroid is despawning because it hit a bullet.
    /// </param>
    public void despawnAsteroid(GameObject current, bool bulletHit)
    {
        bool isChild = current.GetComponent<Asteroid>().childFlag;

        if (!isChild)
        {
            // Only add an asteroid to the spawn pool if it is not a child
            asteroidPool.Enqueue(current);
        }
        else
        {
            // We know the asteroid is a child, remove it from Collision Manager's list of asteroids
            collisionManager.GetComponent<CollisionManager>().removeFromAsteroidList(current);
        }

        // Only check for splitting if the object isn't a child
        if (bulletHit && !isChild)
        {
            Debug.Log("Bullet hit an asteroid");
            // Since a bullet hit the asteroid, cause it to split

            current.GetComponent<Asteroid>().splitSelf();

            // First stage asteroid is being destroyed, so increase score by 20
            collisionManager.GetComponent<CollisionManager>().increaseScore(20);
        }
        else if(bulletHit && isChild)
        {
            // A second stage asteroid got hit, increase the score by 50
            collisionManager.GetComponent<CollisionManager>().increaseScore(50);
        }

        // Set the current asteroid's position to be off screen so there won't be any false collisions
        current.transform.position = transform.position = new Vector3(totalCamWidth * 2, totalCamHeight * 2, 0);

        current.SetActive(false);

        if (isChild)
        {
            Destroy(current);
        }

        // Reset the flag for colliding with the ship in sprite info
        current.GetComponent<SpriteInfo>().hitShip = false;

        // Decrement the asteroid count
        asteroidCount--;
    }


    /// <summary>
    /// Adds a given asteroid to the collisionManager.
    /// </summary>
    /// <param name="asteroid">
    /// The asteroid to add to the CollisionManager.
    /// </param>
    public void addToCollision(GameObject asteroid)
    {
        collisionManager.GetComponent<CollisionManager>().addToAsteroidList(asteroid);
    }


    /// <summary>
    /// Randomly changes the asteroid sprite between one of three potential
    /// options whenever it is spawned.
    /// </summary>
    /// <param name="current">
    /// The asteroid whose sprite should be changed.
    /// </param>
    private void changeAsteroidSprite(GameObject current)
    {
        // Will change the given asteroid's sprite to a random one
        float spriteDecision = Random.Range(0.0f, 1.0f);

        if(spriteDecision < 0.33)
        {
            // Pick the first sprite
            current.GetComponent<SpriteInfo>().changeSprite(spriteAsteroid1);
        }
        else if(spriteDecision < 0.66)
        {
            // Pick the second sprite
            current.GetComponent<SpriteInfo>().changeSprite(spriteAsteroid2);
        }
        else
        {
            // Pick the third sprite
            current.GetComponent<SpriteInfo>().changeSprite(spriteAsteroid3);
        }

    }
}