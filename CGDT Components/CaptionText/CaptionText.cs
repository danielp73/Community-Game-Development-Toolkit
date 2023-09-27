using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class CaptionText : MonoBehaviour
{
    private GameObject loadedPanelPrefab;
    public bool loopSentences = false;

    public float playerInteractionZone;
    public bool backgroundbox= true;
    public UnityEngine.Color backgroundBoxColor = UnityEngine.Color.white;
    public UnityEngine.Color textColor = UnityEngine.Color.grey;

    public string[] sentences = { "Enter sentences", "here" };
    private bool onDisplay = false;
    private int currentSentence = 0;
    private SphereCollider sc;


    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<SphereCollider>())
        {
            Debug.Log("PanelText: no sphere collider");
            sc = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
            sc.isTrigger = true;
        } else
        {
            sc = GetComponent<SphereCollider>();
        }
        playerInteractionZone = sc.radius;
        Debug.Log("PanelText: collider radius is " + playerInteractionZone);
    }

    // Update is called once per frame
    void Update()
    {
        if(sentences == null)
        {
            Debug.Log("no sentences");
        }


        if(onDisplay)
        {
            if(Input.GetMouseButtonDown(0))
            {
                currentSentence++;
                if(currentSentence>=sentences.Length)
                {
                    if(loopSentences)
                    {
                        currentSentence = 0;
                    } else
                    {
                        turnOff();
                    }
                } else
                {
                    loadedPanelPrefab.transform.GetComponentInChildren<Text>().text = sentences[currentSentence];
                }
    
            }
        }

        sc.radius = playerInteractionZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            turnOn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        turnOff();
    }

    private void turnOff()
    {
        Destroy(loadedPanelPrefab);
        onDisplay = false;
        currentSentence = 0;
    }

    private void turnOn()
    {
        loadedPanelPrefab = Instantiate(Resources.Load("CaptionPanel")) as GameObject;
        loadedPanelPrefab.transform.GetComponentInChildren<Text>().text = sentences[currentSentence];
        loadedPanelPrefab.transform.GetComponentInChildren<Text>().color = textColor;
        loadedPanelPrefab.transform.GetComponentInChildren<Image>().color = backgroundBoxColor;
        loadedPanelPrefab.transform.GetComponentInChildren<Image>().enabled = backgroundbox;
        onDisplay = true;
    }
}
