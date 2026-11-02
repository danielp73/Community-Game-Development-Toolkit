using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    private bool wasHovered;
    public AudioSource NextUI;
    

    void Start()
    {
        firstLoop = true;
        currentSentence = 0;
        textField.color = Color.clear;
        //NextUI = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        bool hovered = IsPointerOverObject();
        if (hovered)
        {
            HandlePointerHover();
        }
        else if (wasHovered)
        {
            ResetHover();
        }
        wasHovered = hovered;
        FadeText();
    }

    private bool IsPointerOverObject()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return false;

        // Check the cameras that render this object, with the top camera first.
        Camera[] cameras = Camera.allCameras;
        System.Array.Sort(cameras, (a, b) => b.depth.CompareTo(a.depth));
        foreach (Camera camera in cameras)
        {
            if (camera.targetTexture != null) continue;
            Vector2 pointer = Cursor.lockState == CursorLockMode.Locked
                ? camera.pixelRect.center
                : mouse.position.ReadValue();
            if (!camera.pixelRect.Contains(pointer)) continue;
            int mask = camera.cullingMask & camera.eventMask & Physics.DefaultRaycastLayers;
            Ray ray = camera.ScreenPointToRay(pointer);
            Vector3 direction = camera.transform.InverseTransformDirection(ray.direction);
            float distance = Mathf.Abs(direction.z) > Mathf.Epsilon
                ? Mathf.Abs((camera.farClipPlane - camera.nearClipPlane) / direction.z)
                : Mathf.Infinity;

            bool hit3D = Physics.Raycast(ray, out RaycastHit hit, distance, mask);
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, distance, mask);
            if (hit3D || hit2D.collider != null)
            {
                // A nearer collider blocks interaction with objects behind it.
                GameObject target = hit2D.collider != null && (!hit3D || hit2D.distance < hit.distance)
                    ? hit2D.collider.gameObject
                    : hit.collider.gameObject;
                return target == gameObject;
            }
            // An opaque camera obscures cameras below it, even without a hit.
            if (camera.clearFlags == CameraClearFlags.Skybox || camera.clearFlags == CameraClearFlags.SolidColor)
                return false;
        }
        return false;
    }

    void HandlePointerHover()
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





        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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

    void ResetHover()
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
