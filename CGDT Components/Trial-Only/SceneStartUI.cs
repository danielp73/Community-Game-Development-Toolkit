using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneStartUI : MonoBehaviour
{
    public GameObject gameStartImage;
    public bool gameStart = true;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        gameStartImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //check if game over here!
        //then set gameEnd to true
    }

    private void OnGUI()
    {
        Event e = Event.current;

        if (gameStart)
        {
            if (e.keyCode == KeyCode.Return)
            {
                gameStartImage.SetActive(false);
                gameStart = false;
                Time.timeScale = 1;
            }
        }
    }
}