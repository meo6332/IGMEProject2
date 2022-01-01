using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteInfo : MonoBehaviour
{
    
    public SpriteRenderer spriteRenderer;

    public bool colliding;

    // This is used only for the asteroid to determine if it's colliding with a ship
    public bool hitShip;

    private Color currentColor;

    public Bounds spriteBounds;

    // Start is called before the first frame update
    void Awake()
    {
        currentColor = Color.white;

        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteBounds = spriteRenderer.bounds;

        colliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        spriteBounds = spriteRenderer.bounds;

        if (colliding)
        {
            // Do checks to see what behavior needs to happen here
            if(this.gameObject.GetComponent<Asteroid>() != null)
            {
                // This means we're dealing with an asteroid
                // If hit ship is true, we're colliding with a ship and despawnSelf
                // should use false. Otherwise, we're colliding with a bullet and it should use true.
                this.gameObject.GetComponent<Asteroid>().despawnSelf(!hitShip);
            }
            else if(this.gameObject.GetComponent<Projectile>() != null)
            {
                // This means we're dealing with a bullet
                // Despawn it since it should be deleted
                this.gameObject.GetComponent<Projectile>().despawnSelf();
            }
            else if(this.gameObject.GetComponent<Vehicle>() != null)
            {
                // This means we're dealing with a vehicle.
                // Destroy the vehicle then check if it should respawn

                this.gameObject.GetComponent<Vehicle>().damageShip();
                changeColor(Color.red);

            }

        }
    }


    /// <summary>
    /// Updates the sprite's color to a given value.
    /// </summary>
    /// <param name="color">
    /// The color to change the sprite to.
    /// </param>
    public void changeColor(Color color)
    {
        spriteRenderer.color = color;
    }


    /// <summary>
    /// Changes the sprite's appearance to a given sprite.
    /// </summary>
    /// <param name="newAppearance">
    /// The sprite's new appearance.
    /// </param>
    public void changeSprite(Sprite newAppearance)
    {
        spriteRenderer.sprite = newAppearance;
    }


    /// <summary>
    /// Done to draw the boundaries of the sprites.
    /// </summary>
    public void OnDrawGizmos()
    {
        if (colliding)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireCube(transform.position, spriteBounds.extents); 

        //Gizmos.DrawWireSphere(transform.position, spriteBounds.extents.magnitude);
    }
}


