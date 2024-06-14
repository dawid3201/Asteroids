using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesScript : MonoBehaviour
{   
    //Display text on the screen
    public static int liveScore = 5;
    Text life;

    void Start()
    {
        life = GetComponent<Text> ();
    }

    // Update is called once per frame
    void Update()
    {
        life.text = "Lives: " + liveScore;
    }
}
