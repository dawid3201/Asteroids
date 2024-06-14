using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour
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
    private void Update()
    {
        if (IsOffScreen())
        {
            AlienSpaceShip alienSpaceShip = FindObjectOfType<AlienSpaceShip>();
            if (alienSpaceShip != null)
            {
                alienSpaceShip.IncrementSuccessfulDodgesCounter();
            }

            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {   //Bullets is drestroyd affter hitting Asteroid
        if (collision.gameObject.CompareTag("Asteroid"))
        {           
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {   EnemyScore.enemyScoreValue += 70;        
            Destroy(gameObject);
        }
        //if bullet hits the wall it is destroyed
        //this is because there are asteroids spawners outside of the walls so they can spawn at all places on main camera
        if (collision.gameObject.CompareTag("Wall"))
        {           
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {           
            Destroy(gameObject);
        }
    }
    // This method checks if the UFO's position is off the screen (outside the camera's viewport).
    private bool IsOffScreen()
        {
            // Get a reference to the main camera.
            Camera mainCamera = Camera.main;
            // Convert the UFO's world position to a viewport position (normalized coordinates ranging from 0 to 1).
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            
            // Check if the x or y coordinate of the screenPoint is outside the range of 0 to 1.
            // If so, the UFO is considered off the screen and the method returns true, otherwise, it returns false.
            return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
        }



}