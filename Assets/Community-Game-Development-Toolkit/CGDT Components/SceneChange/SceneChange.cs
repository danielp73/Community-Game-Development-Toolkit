using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneChange : MonoBehaviour
{

    //public string scenePath = "Assets/Community-Game-Development-Toolkit/Example Scenes/Forest.unity";
    public string scenePath;
    public int sceneIndex;
    public bool isVisible = true;
    public bool isSceneChange = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!GetComponent<BoxCollider>())
        {
            Debug.Log("no box collider");
            BoxCollider bc =  gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
            bc.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().enabled = isVisible;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("scene change: " + scenePath);
        SceneManager.LoadScene(scenePath);
    }

}
