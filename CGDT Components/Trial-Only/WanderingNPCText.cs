using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WanderingNPCText : MonoBehaviour
{


    public float speed = 3.0f;
    public float backupAmount = 3.0f;
    public float obstacleRange = 2.0f;
    public GameObject boundaryObject;
    private Collider boundaryCollider;
    private bool playerInRange = false;
    private float setSpeed;

    public string[] sentences;
    private int currentSentence;
    public Text textField;
    public float fadeTime = 10;
    private bool displayInfo;
    private bool firstLoop;
    public bool loopText = false;
    public bool displayOnlyOnce = false;
    private bool firstDisplay = true;
    public AudioSource NextUI;
    

    // Start is called before the first frame update
    void Start()
    {
        boundaryCollider = boundaryObject.GetComponent<Collider>();
        setSpeed = speed;

        firstLoop = true;
        currentSentence = 0;
        textField.color = Color.clear;
        //NextUI = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        FadeText();



        transform.Translate(0, 0, speed * Time.deltaTime);

        //check if at collider boundary
        if (boundaryCollider)
        {
            if (!boundaryCollider.bounds.Contains(transform.position))
            {
                Debug.Log("Wanderer: Boundary.");
                backupAndTurn();
            }
        }

        //check if hitting obstacle
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.75f, out hit) && (speed != 0))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (hit.distance < obstacleRange)
            {
                Debug.Log("Wanderer: Obstacle.");
                backupAndTurn();
            }
        }

        //handle text if player in range
        if(playerInRange)
        {

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
            }
            else
            {
                displayInfo = false;
            }

            //cycle through text
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("clicked");
                if (NextUI && displayInfo)
                {
                    Debug.Log("playing sound");
                    NextUI.Play();
                }
                else
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
    }

    void backupAndTurn()
    {

        float angle = Random.Range(90, 270);
        Debug.Log("Wanderer: Rotate " + angle.ToString());
        transform.Translate(0, 0, -1*backupAmount* speed * Time.deltaTime);

        transform.Rotate(0, angle, 0);
    }

    void OnMouseOver()
    {
        Debug.Log("wanderer: mouseover");

    

    }

    void OnMouseExit()
    {
        //Debug.Log("mouse exit");
        displayInfo = false;
        firstLoop = true;
    }

    void FadeText()
    {



        if (displayInfo && playerInRange)
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



    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {

            Vector3 newtarget = other.transform.position;
            newtarget.y = transform.position.y;
            transform.LookAt(newtarget);
            //transform.LookAt(other.transform);


            Debug.Log("Wanderer: player enter");
            speed = 0;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("Wanderer: player exit");
            speed = setSpeed;
            playerInRange = false;
            backupAndTurn();
        }
    }


}