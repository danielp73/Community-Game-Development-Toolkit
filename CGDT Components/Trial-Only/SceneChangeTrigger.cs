using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public GameObject gameTransitionImage;
    public string sceneToLoad;
    private bool gameTransition = false;

    // Start is called before the first frame update
    void Start()
    {
        gameTransitionImage.SetActive(false);
        if (sceneToLoad == "")
        {
            sceneToLoad = SceneManager.GetActiveScene().path;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("game end: trigger enter");
        if (other.gameObject.tag == triggerTag)
        {
            Debug.Log("game end: tag match");
            if (gameTransitionImage)
            {
                gameTransition = true;
                gameTransitionImage.SetActive(true);
                Time.timeScale = 0;
            } else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnGUI()
    {
        if (gameTransition)
        {
            Event e = Event.current;
            if (e.keyCode == KeyCode.Return)
            {
                gameTransition = false;
                SceneManager.LoadScene(sceneToLoad);
                Time.timeScale = 1;
            }
        }

    }
}
