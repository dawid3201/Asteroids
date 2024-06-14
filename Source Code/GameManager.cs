using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWonUI;
    public GameObject pauseMenu;
    public GameObject message;

    //UFO
    public GameObject ufoWonUI;

    public LivesScript LivesScript;
    public ScoreScript ScoreScript;


  
    // Set pause Menu to false so it does not appear at the start
    public bool isPaused = false;
    void Start()
    {
        pauseMenu.SetActive(false);
        message.SetActive(false);
    }

    //if player press ESC on keyboard, the game will pause
    //if game is paused and player press ESC, the game will continue to play
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                ResumeGame();
            }else{
                PausedGame();
            }
        }
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }
    //Display about 2nd phase
    public void fastMessage(){
        message.SetActive(true);

    }
    
    //Restart button - game lost
    public void restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LivesScript.liveScore = 3;
        ScoreScript.scoreValue = 0;

    }
    //Play again button - game won
    public void playAgain(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LivesScript.liveScore = 3;
        ScoreScript.scoreValue = 0;
        Time.timeScale = 1f;
    }
    //win screen
    public void gameWon()
    {
        gameWonUI.SetActive(true);
        Time.timeScale = 0f;
        
    }

    public void ufoWon()
    {
        ufoWonUI.SetActive(true);
        Time.timeScale = 0f;
        
    }
    
    //For Paused Screen
    public void PausedGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    //to reasume game after it was paused
    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    //Quit button
    public void QuitGame(){
        Application.Quit();
    }
}
