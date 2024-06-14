using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScore : MonoBehaviour
{   //display socre points on the screen
    public static int enemyScoreValue = 0;
    Text enemyScore;

    void Start()
    {
        enemyScore = GetComponent<Text> ();
    }

    // Update is called once per frame
    void Update()
    {
        enemyScore.text = "Enemy Score: " + enemyScoreValue;
    }
}
