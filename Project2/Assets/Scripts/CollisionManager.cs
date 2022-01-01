using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionManager : MonoBehaviour
{

    // True if aabb is on, false otherwise
    [SerializeField]
    bool aabbOn;

    [SerializeField]
    GameObject ship;

    [SerializeField]
    List<GameObject> asteroidList = new List<GameObject>();

    CollisionDetection detection;

    [SerializeField]
    Text uiText;

    [SerializeField]
    List<GameObject> bulletList = new List<GameObject>();

    private int score;

    // Start is called before the first frame update
    void Start()
    {
        aabbOn = true;

        detection = GetComponent<CollisionDetection>();

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

        // First populate the asteroidList
        foreach (GameObject o in objects)
        {
            // Check if the name is one of the asteroids
            if (o.name.Contains("asteroid(Clone)"))
            {
                asteroidList.Add(o);
            }
        }

        Debug.Log("Asteroid List Size: " + asteroidList.Count);

        foreach (GameObject o in objects)
        {
            // Check if the name is one of the asteroids
            if (o.name.Contains("Bullet(Clone)"))
            {
                bulletList.Add(o);
            }
        }

        Debug.Log("BulletList length: " + bulletList.Count);
    }

    // Update is called once per frame
    void Update()
    {

        // If check was ever true, set ship to colliding
        bool shipCheck = false;

        if (aabbOn)
        {
            // If true, then do the aabb collision detection
            // between each asteroid and the ship
            foreach(GameObject asteroid in asteroidList)
            {
                Asteroid currentScript = asteroid.GetComponent<Asteroid>();

                // Only do collision detection for moving asteroids
                if (currentScript.moving)
                {

                    bool check = detection.AABBCollision(asteroid, ship);

                    // Set the colliding value equal to the results of check
                    asteroid.GetComponent<SpriteInfo>().colliding = check;

                    asteroid.GetComponent<SpriteInfo>().hitShip = check;

                    if (check)
                    {
                        shipCheck = true;
                    }
                    else
                    {
                        // If the asteroid didn't hit a ship, ensure that it knows that
                        // so it does the proper behavior if it hits a bullet
                        asteroid.GetComponent<SpriteInfo>().hitShip = false;
                    }
                }
                
            }

            bool[] asteroidCollideCheck = new bool[asteroidList.Count];

            int asteroidNum = 0;

            bool[] bulletCollideCheck = new bool[bulletList.Count];

            int bulletNum;

            for (int i = 0; i < bulletList.Count; i++)
            {
                bulletCollideCheck[i] = false;
            }

            for (int i = 0; i < asteroidList.Count; i++)
            {
                asteroidCollideCheck[i] = false;
            }

            // Do the collision check for each bullet with each astroid
            foreach (GameObject asteroid in asteroidList)
            {
                

                bulletNum = 0;
                foreach(GameObject bullet in bulletList)
                {
                    bool check = false;

                    if (bullet.GetComponent<Projectile>().fired && asteroid.GetComponent<Asteroid>().moving)
                    {
                        // only check for the collision detection if the bullet is fired
                        check = detection.AABBCollision(bullet, asteroid);
                    }

                    // Set the colliding value equal to the results of check
                    if (!bulletCollideCheck[bulletNum])
                    {
                        // If the value is false, check if it needs to be swapped to true
                        bulletCollideCheck[bulletNum] = check;
                    }

                    if (!asteroidCollideCheck[asteroidNum])
                    {
                        // If this is false, then the asteriod needs to be flagged to be colliding or not
                        asteroidCollideCheck[asteroidNum] = check;
                    }

                    bulletNum++;
                }
                asteroidNum++;
            }

            bulletNum = 0;
            foreach (GameObject bullet in bulletList)
            {
                bullet.GetComponent<SpriteInfo>().colliding = bulletCollideCheck[bulletNum];

                bulletNum++;
            }

            asteroidNum = 0;
            foreach (GameObject asteroid in asteroidList)
            {
                bool currentState = asteroid.GetComponent<SpriteInfo>().colliding;
                if (!currentState)
                {
                    // If this is false, check if the asteroid is colliding with any bullet
                    asteroid.GetComponent<SpriteInfo>().colliding = asteroidCollideCheck[asteroidNum];
                }

                // If current state is true, don't mess with it as then the asteroid is colliding with the ship
                asteroidNum++;
            }

        } // End of AABB collision checks
        else
        {
            foreach (GameObject asteroid in asteroidList)
            {
                Asteroid currentScript = asteroid.GetComponent<Asteroid>();

                // Only do collision detection for moving asteroids
                if (currentScript.moving)
                {

                    bool check = detection.CircleCollision(asteroid, ship);

                    // Set the colliding value equal to the results of check
                    asteroid.GetComponent<SpriteInfo>().colliding = check;

                    asteroid.GetComponent<SpriteInfo>().hitShip = check;

                    if (check)
                    {
                        shipCheck = true;
                    }
                    else
                    {
                        // If the asteroid didn't hit a ship, ensure that it knows that
                        // so it does the proper behavior if it hits a bullet
                        asteroid.GetComponent<SpriteInfo>().hitShip = false;
                    }
                }
            }

            bool[] asteroidCollideCheck = new bool[asteroidList.Count];

            int asteroidNum = 0;

            bool[] bulletCollideCheck = new bool[bulletList.Count];

            int bulletNum;

            for (int i = 0; i < bulletList.Count; i++)
            {
                bulletCollideCheck[i] = false;
            }

            for (int i = 0; i < asteroidList.Count; i++)
            {
                asteroidCollideCheck[i] = false;
            }

            // Do the collision check for each bullet with each astroid
            foreach (GameObject asteroid in asteroidList)
            {


                bulletNum = 0;
                foreach (GameObject bullet in bulletList)
                {
                    bool check = false;

                    if (bullet.GetComponent<Projectile>().fired)
                    {
                        // only check for the collision detection if the bullet is fired
                        check = detection.CircleCollision(bullet, asteroid);
                    }

                    // Set the colliding value equal to the results of check
                    if (!bulletCollideCheck[bulletNum])
                    {
                        // If the value is false, check if it needs to be swapped to true
                        bulletCollideCheck[bulletNum] = check;
                    }

                    if (!asteroidCollideCheck[asteroidNum])
                    {
                        // If this is false, then the asteriod needs to be flagged to be colliding or not
                        asteroidCollideCheck[asteroidNum] = check;
                    }

                    bulletNum++;
                }
                asteroidNum++;
            }

            bulletNum = 0;
            foreach (GameObject bullet in bulletList)
            {
                bullet.GetComponent<SpriteInfo>().colliding = bulletCollideCheck[bulletNum];

                bulletNum++;
            }

            asteroidNum = 0;
            foreach (GameObject asteroid in asteroidList)
            {
                bool currentState = asteroid.GetComponent<SpriteInfo>().colliding;
                if (!currentState)
                {
                    // If this is false, check if the asteroid is colliding with any bullet
                    asteroid.GetComponent<SpriteInfo>().colliding = asteroidCollideCheck[asteroidNum];
                }

                // If current state is true, don't mess with it as then the asteroid is colliding with the ship
                asteroidNum++;
            }
        } // End of Circle Collision checks

        ship.GetComponent<SpriteInfo>().colliding = shipCheck;           

    }


    /// <summary>
    /// Adds an asteroid to the asteroid list.
    /// </summary>
    /// <param name="asteroid">
    /// The asteroid to add to the asteroid list.
    /// </param>
    public void addToAsteroidList(GameObject asteroid)
    {
        // Adds the specified asteroid to the asteroid list
        asteroidList.Add(asteroid);
    }


    /// <summary>
    /// Removes a given asteroid from the asteroid list
    /// </summary>
    /// <param name="asteroid">
    /// The asteroid to remove from the asteroid list
    /// </param>
    public void removeFromAsteroidList(GameObject asteroid)
    {
        asteroidList.Remove(asteroid);
    }


    /// <summary>
    /// Increases the score by a given ammount.
    /// </summary>
    /// <param name="amount">
    /// The amount to increase the score by.
    /// </param>
    public void increaseScore(int amount)
    {
        // Increases the score displayed by the given amount
        score += amount;

        string updatedText = "Score: ";

        updatedText += score.ToString();

        uiText.text = updatedText;
    }

}
