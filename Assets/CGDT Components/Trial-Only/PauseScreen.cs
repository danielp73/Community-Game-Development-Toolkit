using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseScreen;

    private bool isPaused = false;
    public string pauseKey = "escape";

    // Start is called before the first frame update
    void Start()
    {
        pauseScreen.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            Debug.Log("pause key pressed");
            if (!isPaused)
            {
                Debug.Log("pausing");
                //Wasn't paused
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
                isPaused = true;
            }
            else
            {
                Debug.Log("unpausing");
                //Was paused
                pauseScreen.SetActive(false);
                isPaused = false;
                Time.timeScale = 1;
            }
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(isPaused)
            {
                Debug.Log("unpausing");
                //Was paused
                pauseScreen.SetActive(false);
                isPaused = false;
                Time.timeScale = 1;
            }
        }
    }
}