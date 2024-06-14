using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   //Unity will call it automaticaly everysingle frame that game is running 

    public Bullet bulletRef; //reference to bullet 
    
    public GameManger GameManger;
    private bool isDead;
    private bool didWon;

    public LivesScript LivesScript;
    public ScoreScript ScoreScript;

    private bool moving;//moving 
    private float movingBack;//slowing down
    private float turning;
    private new Rigidbody2D rigidbody;
    //PUBLIC so it is easier to change in editor
    public float speed = 1.0f; //Speed when we move forward
    public float turnSpeed = 1.0f; //Speed when we turn

    //sound effects
    [SerializeField] private AudioSource deathSoundEffect; 
    [SerializeField] private AudioSource spawnSoundEffect; 

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();//gives us access to object player
    }
    //Player Movement
    private void Update()
    {
        moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));{
            movingBack = -1.0f;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            turning = 1.0f; 
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            turning = -1.0f; 
        }
        else{
            turning = 0.0f;
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){//call our method Shoot ||Space or left mouse button has to be pressed and let down
            Shoot();
        }
        //Phase 2
        if(ScoreScript.scoreValue >= 5000){
            GameManger.fastMessage();
        }
    }

    private void FixedUpdate() //This function only gets called when player is moving
    {
        if (moving){
            rigidbody.AddForce(this.transform.up * this.speed);//addind force will move this object
        }
        if(movingBack != 0.0f){
            rigidbody.AddForce(this.transform.up * 0.0f);//if we want to slow down or stop 
        }
        if(turning != 0.0f){
            rigidbody.AddTorque(turning * this.turnSpeed);//I used torque because I want player to rotate only when button is clicked, not all the time. So I give force when plaer click a button
        }
    }
    private void Shoot(){
        Bullet bullet = Instantiate(this.bulletRef, this.transform.position, this.transform.rotation);//refers to the player, bullet will come from our ship
        bullet.Fire(this.transform.up);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        List<string> tagsToCheck = new List<string> { "Asteroid", "AlienBullet", "UFO"}; // list of tags to check

        if (tagsToCheck.Contains(collision.gameObject.tag)) 
        {
            transform.position = new Vector2(40000f, 40000f); //move player far away from the map
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
            deathSoundEffect.Play();
            turnOffCollisions();
            Invoke("reset", 3f);
            
            //decrease int lives if player dies
            int lives = LivesScript.liveScore--;
            if(lives == 0 && !isDead){
                isDead = true;
                Destroy(gameObject);
                LivesScript.liveScore = 0;
                GameManger.gameOver();
            }
        }
    }


    //turn collisions off for player after the ship is spawned again
    void turnOffCollisions(){
        gameObject.layer = LayerMask.NameToLayer("Ignore");
    }
    //move player back to the middle of a map
    void reset(){
        transform.position = new Vector2(0f, 0f);
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        spawnSoundEffect.Play();
        Invoke("turnOnCollisions", 3f);
    }
    //turn collisions on again
    void turnOnCollisions(){
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

}
