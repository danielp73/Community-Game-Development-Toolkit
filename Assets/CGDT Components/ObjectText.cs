using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectText : MonoBehaviour
{
    public string[] sentences;
    private int currentSentence;
    public Text textField;
    public float fadeTime = 10;
    private bool displayInfo;
    private bool firstLoop;
    public bool loopText = true;
    public bool displayOnlyOnce = false;
    private bool firstDisplay = true;
    public AudioSource NextUI;
    

    void start()
    {
        firstLoop = true;
        currentSentence = 0;
        textField.color = Color.clear;
        //NextUI = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        FadeText();
    }

    void OnMouseOver()
    {
        Debug.Log("text mouseover");

        //check if we're showing text only once per game
        if (displayOnlyOnce && !firstDisplay)
        {
            displayInfo = false;
            return;
        }

        //check if this is the first loop while pointing to the object
        //and if we're looping the text per mouse over
        if (firstLoop || loopText)
        {
            displayInfo = true;
        } else
        {
            displayInfo = false;
        }





        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked");
            if (NextUI && displayInfo)
            {
                Debug.Log("playing sound");
                NextUI.Play();
            } else
            {
                Debug.Log("not playing sound");
            }
            currentSentence++;
            if (currentSentence >= sentences.Length)
            {
                Debug.Log("got to end");
                currentSentence = 0;
                firstLoop = false;
                firstDisplay = false;

            }
        }

    }

    void OnMouseExit()
    {
        //Debug.Log("mouse exit");
        displayInfo = false;
        firstLoop = true;
    }

    void FadeText()
    {
    


        if (displayInfo)
        {
            textField.text = sentences[currentSentence];
            textField.color = Color.Lerp(textField.color, Color.white, fadeTime * Time.deltaTime);
        }

        else
        {
            textField.color = Color.Lerp(textField.color, Color.clear, fadeTime * Time.deltaTime);
            //if (textField.color == Color.clear)
            //{
            //    currentSentence = 0;
            //}
        }
    }
}
