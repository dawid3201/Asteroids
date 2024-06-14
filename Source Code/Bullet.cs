using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public float speed = 500f; //speed of the bullets
    public float maxLifetime = 5f; //bullets dissapear after 5 seconds 

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction)
    {
        // The bullet only needs a force to be added once since they have no
        // drag to make them stop moving
        rigidbody.AddForce(direction * this.speed);

        // Destroy the bullet after it reaches it max lifetime
        Destroy(this.gameObject, this.maxLifetime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {   //Bullets is drestroyd affter hitting Asteroid
        if (collision.gameObject.CompareTag("Asteroid"))
        {           
            Destroy(gameObject);
        }
        //if bullet hits the wall it is destroyed
        //this is because there are asteroids spawners outside of the walls so they can spawn at all places on main camera
        if (collision.gameObject.CompareTag("Wall"))
        {           
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("UFO"))
        {           
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("AlienBullet"))
        {           
            Destroy(gameObject);
        }
    }

}