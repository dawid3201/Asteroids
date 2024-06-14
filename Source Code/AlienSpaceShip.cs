using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSpaceShip : MonoBehaviour
{
    public float speed = 5f;
    public Player target;
    public Vector3 respawnPosition;
    public GameObject alienBulletPrefab;
    public float shootingRate = 2f; // Shoot every 2 seconds
    public float asteroidDestructionRange = 5f;
    public float playerDetectionRange = 10f;

    private float initialHealth = 100f;
    private float health;
    private float shootingTimer;

    public float adaptiveShootingRateMultiplier = 0.9f; // Reduce shooting rate by 10% (increase shooting speed) when adapting
    public float adaptiveShootingAccuracyMultiplier = 0.5f; // Reduce shooting inaccuracy by 50% when adapting
    public int successfulDodgesThreshold = 5; // Number of successful dodges needed to trigger adaptive behavior

    private int successfulDodgesCounter = 0;
    private bool isAdaptiveShootingActive = false;

    private void Start()
    {
        target = FindObjectOfType<Player>();
        health = initialHealth;
        shootingTimer = 0f;
    }

    private void Update()
    {
        if (target != null && Vector2.Distance(target.transform.position, transform.position) <= playerDetectionRange)
        {
            MoveTowardsPlayer();
            Shoot();
        }
        else
        {
            GameObject nearestAsteroid = FindNearestAsteroid(asteroidDestructionRange);
            if (nearestAsteroid != null)
            {
                MoveTowardsNearestAsteroid(nearestAsteroid);
                ShootAtAsteroid(nearestAsteroid);
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    // This method handles the UFO shooting behavior, with optional adaptive shooting rate and accuracy adjustments.
    private void Shoot()
    {
        // Increment the shooting timer by the time elapsed since the last frame.
        shootingTimer += Time.deltaTime;

        // Set the current shooting rate to the default shooting rate.
        float currentShootingRate = shootingRate;

        // If adaptive shooting is active, apply the adaptive shooting rate multiplier.
        if (isAdaptiveShootingActive)
        {
            currentShootingRate *= adaptiveShootingRateMultiplier;
        }

        // Check if the shooting timer has reached or exceeded the current shooting rate.
        if (shootingTimer >= currentShootingRate)
        {
            // Reset the shooting timer.
            shootingTimer = 0f;
            // Instantiate a new bullet GameObject using the alienBulletPrefab, at the UFO's position, with no rotation.
            GameObject bullet = Instantiate(alienBulletPrefab, transform.position, Quaternion.identity);
            // Get the AlienBullet script component attached to the instantiated bullet GameObject.
            AlienBullet alienBulletScript = bullet.GetComponent<AlienBullet>();
            // Calculate the direction vector from the UFO's position to the target's position, and normalize it.
            Vector2 direction = (target.transform.position - transform.position).normalized;

            // If adaptive shooting is active, apply the adaptive shooting accuracy multiplier.
            if (isAdaptiveShootingActive)
            {
                // Calculate a random angle offset based on the accuracy multiplier.
                float angleOffset = Random.Range(-adaptiveShootingAccuracyMultiplier, adaptiveShootingAccuracyMultiplier);
                // Apply the angle offset to the direction vector.
                direction = Quaternion.Euler(0, 0, angleOffset) * direction;
            }

            // Fire the bullet in the calculated direction using the Fire method of the AlienBullet script.
            alienBulletScript.Fire(direction);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
            collision.rigidbody.velocity = Vector2.zero;

            health -= 25f;

            if (health <= 0f)
            {
                ScoreScript.scoreValue += 150;
                StartCoroutine(Respawn());
            }

            // Destroy the bullet
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator Respawn()
    {
        transform.position = new Vector2(40000f, 40000f);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;

        yield return new WaitForSeconds(10f);

        health = initialHealth;

        // Get the main camera's position and size
        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate the boundaries of the camera view
        float xMin = cameraPosition.x - cameraWidth / 2f;
        float xMax = cameraPosition.x + cameraWidth / 2f;
        float yMin = cameraPosition.y - cameraHeight / 2f;
        float yMax = cameraPosition.y + cameraHeight / 2f;

        // Generate a random position within the boundaries of the camera view
        Vector2 respawnPosition = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

        // Make sure the respawn position is not too close to the center of the camera
        float minDistanceFromCenter = cameraWidth / 4f;
        while (Vector2.Distance(respawnPosition, cameraPosition) < minDistanceFromCenter)
        {
            respawnPosition = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }

        // Set the alien's position to the respawn position
        transform.position = respawnPosition;
    }
    
    // This method finds the nearest asteroid within a specified search range around the UFO object.
    private GameObject FindNearestAsteroid(float searchRange)
    {
        // Retrieve all colliders within the search range using a 2D physics overlap circle.
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, searchRange);
        // Initialize the minimum distance to infinity for comparison purposes.
        float minDistance = Mathf.Infinity;
        // Initialize the nearest asteroid GameObject as null.
        GameObject nearestAsteroid = null;

        // Iterate through all colliders found in the search range.
        foreach (Collider2D collider in collidersInRange)
        {
            // Check if the current collider has an "Asteroid" tag.
            if (collider.gameObject.CompareTag("Asteroid"))
            {
                // Calculate the distance between the UFO and the current asteroid.
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                // If the current distance is less than the minimum distance found so far,
                // update the minimum distance and set the nearest asteroid to the current collider's GameObject.
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAsteroid = collider.gameObject;
                }
            }
        }

        // Return the nearest asteroid GameObject found, or null if none were found within the search range.
        return nearestAsteroid;
    }


    private void MoveTowardsNearestAsteroid(GameObject asteroid)// if UFO object finds an asteroid it will move towards it 
    {
        transform.position = Vector3.MoveTowards(transform.position, asteroid.transform.position, speed * Time.deltaTime);
    }

    // This method makes the UFO shoot at a specified asteroid while moving towards it.
    private void ShootAtAsteroid(GameObject asteroid)
    {
        // Increment the shooting timer by the time elapsed since the last frame.
        shootingTimer += Time.deltaTime;

        // Check if the shooting timer has reached or exceeded the shooting rate.
        if (shootingTimer >= shootingRate)
        {
            // Reset the shooting timer.
            shootingTimer = 0f;
            // Instantiate a new bullet GameObject using the alienBulletPrefab, at the UFO's position, with no rotation.
            GameObject bullet = Instantiate(alienBulletPrefab, transform.position, Quaternion.identity);
            // Get the AlienBullet script component attached to the instantiated bullet GameObject.
            AlienBullet alienBulletScript = bullet.GetComponent<AlienBullet>();
            // Calculate the direction vector from the UFO's position to the asteroid's position, and normalize it.
            Vector2 direction = (asteroid.transform.position - transform.position).normalized;
            // Fire the bullet in the calculated direction using the Fire method of the AlienBullet script.
            alienBulletScript.Fire(direction);
        }
    }


    public void IncrementSuccessfulDodgesCounter()
    {
        successfulDodgesCounter++;

        if (successfulDodgesCounter >= successfulDodgesThreshold)
        {
            isAdaptiveShootingActive = true;
        }
    }

}
