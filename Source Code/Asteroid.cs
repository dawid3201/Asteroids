using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpriteRenderer asteroidSize;
    private new Rigidbody2D rigidbody;


    private bool didWon;//if player wins the win, boolean will trun to True 

    private bool ufoWon; // boolean to check if UFO won

    public Sprite[] sprites; // different sprites for asteroid object, there are 4 in total
    public float size = 1.0f; //initially set up size of asteroid to 1, it is public so Unity allowed me to change it later on

    public float speed = 14.0f;
    public float MinSize = 0.5f;
    public float MaxSize = 1.5f;
    public float maxLifeTime = 30.0f;

    private Vector3 lastVelocity;

    private void Awake(){
        asteroidSize = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update(){
        lastVelocity = rigidbody.velocity;
    }
    private void Start(){ 
        asteroidSize.sprite = sprites[Random.Range(0, sprites.Length)];//choose random sprite for our asteroid, there are 4 in total

        this.transform.eulerAngles = new Vector3(0,0,Random.value * 360.0f);//spin asteroids by vector Z
        this.transform.localScale = Vector3.one * this.size;//Size of asteroids

        rigidbody.mass = this.size; //mass depends on size

        //Here I stopped collision between Walls and Asteroids for 1sec when Asteroids spawn as it caused
        //problems with Asteroids spawning directly on the wall and not moving at all
        //unfortunately when Asteroid is destroyed right next to Wall it sometimes goes throught it 
        turnOffCollisions();
        Invoke("reset", 1f); 
    }
    void turnOffCollisions(){
        gameObject.layer = LayerMask.NameToLayer("IgnoreAsteroids");
    }
    void reset(){
        Invoke("turnOnCollisions", 1f);
    }
    void turnOnCollisions(){
        gameObject.layer = LayerMask.NameToLayer("Asteroid");
    }

    public void DirectionOfAsteroid(Vector2 direction){
        rigidbody.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.maxLifeTime);
         if(ScoreScript.scoreValue >= 5000){
                rigidbody.AddForce(direction * 20f);
            }
        }

    private void OnCollisionEnter2D(Collision2D collision)
    {//asteroids will brake on collistion with Bullet at most 2 times. It depends on size of asteroid 
        //player system score
        if (collision.gameObject.CompareTag("Bullet")){
            if ((size * 0.7f) >= MinSize){
                DestroyAsteroids();
                DestroyAsteroids();
            }
            //Score system after hitting asteroids, we get 50points for the smallest, 25 for middle and 10 for largest
            if (size < 0.7f) {
                ScoreScript.scoreValue += 50;
            } else if (size < 1.0f) {
                ScoreScript.scoreValue += 25;
            } else {
                ScoreScript.scoreValue += 10;
            }           
            Destroy(gameObject);
        }
        //UFO score system
        if (collision.gameObject.CompareTag("AlienBullet")){
            if ((size * 0.7f) >= MinSize){
                DestroyAsteroids();
                DestroyAsteroids();
            }
            if (size < 0.7f) {
                EnemyScore.enemyScoreValue += 40;
            } else if (size < 1.0f) {
                EnemyScore.enemyScoreValue += 20;
            } else {
                EnemyScore.enemyScoreValue += 5;
            }           
            Destroy(gameObject);
        }
        //Asteroids will bounce off the walls 
        if (collision.gameObject.CompareTag("Wall")){           
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal); //after hitting any of the 4 walls, the asteroid will bounce and go in the opposite direction

            rigidbody.velocity = direction * Mathf.Max(speed, 1.0f); 
        }//if a player scores 8000 points or more they will see a Win screen
        if(ScoreScript.scoreValue >= 8000 && !didWon){
                didWon = true;
                FindObjectOfType<GameManger>().gameWon();
            }

        if(EnemyScore.enemyScoreValue >= 6000 && !ufoWon){
                ufoWon = true;
                FindObjectOfType<GameManger>().ufoWon();
            }       
    }
    //After collide with bullet asteroids will brake in half
    private Asteroid DestroyAsteroids()
    {
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        Asteroid smaller = Instantiate(this, position, transform.rotation);
        smaller.size = size * 0.5f;

        smaller.DirectionOfAsteroid(Random.insideUnitCircle.normalized);

        return smaller;
    }
}
