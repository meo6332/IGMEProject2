using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileManager : MonoBehaviour
{

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Queue<GameObject> bulletPool = new Queue<GameObject>();

    [SerializeField]
    int poolSize = 10;


    // Start is called before the first frame update
    void Awake()
    {
        // Start by making bullets to put in the pool, and making them inactive
        for(int j = 0; j < poolSize; j++)
        {
            GameObject bullet = Instantiate(bulletPrefab);

            bulletPool.Enqueue(bullet);

            bullet.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Retrieves a bullet from the bullet pool, if there is one.
    /// Will make the bullet active.
    /// </summary>
    /// <returns>
    /// A bullet GameObject from the bullet pool
    /// </returns>
    public GameObject GetBullet()
    {
        GameObject current;

        if(bulletPool.Count > 0)
        {
            // In this case, have a bullet to get from the pool, return that
            current = bulletPool.Dequeue();
            current.SetActive(true);   
        }
        else
        {
            // Not enough bullets, make a new one
            current = Instantiate(bulletPrefab);
        }

        return current;
    }


    /// <summary>
    /// Puts a bullet back into the bulletPool and sets it to be inactive.
    /// </summary>
    /// <param name="bullet">
    /// The bullet to be returned to the bullet pool.
    /// </param>
    public void ReturnBullet(GameObject bullet)
    {
        // This will take a bullet and add it back to the pool while disabling it
        bulletPool.Enqueue(bullet);
        bullet.SetActive(false);
    }

    


}
