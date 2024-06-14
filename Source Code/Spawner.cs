using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{   //Asteroid spawner 
    public Asteroid asteroidRef;//reference to Asteroid class 
    public float timeOfSpawn = 2.0f;//how fast asteroids spawn
    public float spawnAmount = 1.0f;//how many asteroids spawn

    public float spawnDistance = 12.0f;//spawns asteroids behind the walls 

    public float directionVariance = 15.0f;//in what directions will asteroids go

    private void Start(){
        InvokeRepeating(nameof(MakeAsteroids), this.timeOfSpawn, this.spawnAmount);
    }

    private void MakeAsteroids(){
        for (int i = 0; i < this.spawnAmount; i++){

            Vector3 spawnDirection = Random.insideUnitCircle.normalized * this.spawnDistance; //spawn asteroids on the edge of our item and outside of walls
            Vector3 spawnPoint = this.transform.position + spawnDirection;//pick a random spot on the edge of a circle 

            spawnPoint += transform.position;
             
            float variance = Random.Range(-this.directionVariance, this.directionVariance);// choose random point on main camera 
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward); // rotate around the main camera so asteroids spawn in different locations 

            Asteroid asteroid = Instantiate(this.asteroidRef, spawnPoint, rotation);
            asteroid.size = Random.Range(asteroid.MinSize, asteroid.MaxSize);//spawn difference size asteroids
            asteroid.DirectionOfAsteroid(rotation * -spawnDirection);//always point towards the location of out spawner on main camera
        }
    }
    


}