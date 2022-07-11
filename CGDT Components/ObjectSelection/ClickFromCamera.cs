using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class ClickFromCamera : MonoBehaviour
{
    public float rotateSpeed = 20f;
    public float moveSpeed = 5f;
    public float scaleSpeed = 1f;

    private GameObject selected;
    private GameObject oldSelected;
    
    private Renderer selectionRenderer;
    private Renderer oldSelectionRenderer;

    private Camera _camera;

    private Color oldColor = Color.white;
    private Color newColor = Color.red;

    private string rootName;


    void Start()
    {
        //disable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        //gets the camera attached to the player
        _camera = GameObject.Find("Camera").GetComponent<Camera>();

        //GameObject[] obs = (GameObject[]) Object.FindObjectsOfType(typeof(GameObject));
        //for (int i = 0; i < obs.Length; i++)
        //{
        //    rootName = obs[i].name;
        //    if (rootName != null)
        //    {
        //        ReadObjectTransform(rootName);
        //    }
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //if clicked
        {
            //cast a ray from the center of the camera's screen
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0); //middle of camera view
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("raycast hit: " + hit.transform.gameObject.name);

                if (Input.GetMouseButtonDown(0)) //if clicked
                {
                    Debug.Log("clicked on: " + hit.transform.gameObject.name);

                    selected = hit.transform.gameObject;

                    if (selected != oldSelected) //if the object wasn't already selected
                    {
                        if (oldSelected != null) //if there was a previous object
                        { //returns previous selection to original color
                            oldSelectionRenderer = oldSelected.GetComponent<Renderer>();
                            oldSelectionRenderer.material.color = oldColor;
                        }
                        //change the selected object to red
                        selectionRenderer = selected.GetComponent<Renderer>();
                        selectionRenderer.material.color = newColor; //sets a new color

                        oldSelected = selected; //set the newly red object as the previously selected
                    }
                }
            }
        }

        //code to rotate selected object
        if (selected)
        {

            rootName = selected.name;

            //rotate right
            if (Input.GetKey(KeyCode.L))
            {
                selected.transform.RotateAround(_camera.transform.position, _camera.transform.up, rotateSpeed * Time.deltaTime);

                //save
                SaveObjectTransform(rootName);
            }
            //rotate left
            if (Input.GetKey(KeyCode.J))
            {
                selected.transform.RotateAround(_camera.transform.position, _camera.transform.up, -rotateSpeed * Time.deltaTime);


                //save
                SaveObjectTransform(rootName);
            }
            //move away from player
            if (Input.GetKey(KeyCode.I))
            {
               selected.transform.position += _camera.transform.forward * moveSpeed * Time.deltaTime;


                //save
                SaveObjectTransform(rootName);
            }
            //move towards player
            if (Input.GetKey(KeyCode.K))
            {
                selected.transform.position += _camera.transform.forward * -moveSpeed * Time.deltaTime;

                //save
                SaveObjectTransform(rootName);

            }
            //move object up
            if (Input.GetKey(KeyCode.U))
            {
                selected.transform.position += _camera.transform.up * moveSpeed * Time.deltaTime;


                //save
                SaveObjectTransform(rootName);
            }
            //move object down
            if (Input.GetKey(KeyCode.M))
            {
               selected.transform.position += _camera.transform.up * -moveSpeed * Time.deltaTime;

                //save
                SaveObjectTransform(rootName);
            }
            
            Vector3 temp;
            //make object bigger
            if (Input.GetKey(KeyCode.O))
            {
                temp = selected.transform.localScale;
                temp.x += scaleSpeed * Time.deltaTime;
                temp.y += scaleSpeed * Time.deltaTime;
                temp.z += scaleSpeed * Time.deltaTime;
                selected.transform.localScale = temp;


                //save
                SaveObjectTransform(rootName);
            }
            //make object smaller
            if (Input.GetKey(KeyCode.P))
            {
                temp = selected.transform.localScale;
                temp.x -= scaleSpeed * Time.deltaTime;
                temp.y -= scaleSpeed * Time.deltaTime;
                temp.z -= scaleSpeed * Time.deltaTime;
                selected.transform.localScale = temp;

                //save
                SaveObjectTransform(rootName);
            }
            //objects rotate on x axis
            if (Input.GetKey(KeyCode.X))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.right, Time.deltaTime * rotateSpeed);


                //save
                SaveObjectTransform(rootName);
            }
            //objects rotate on y axis
            if (Input.GetKey(KeyCode.Y))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.up, Time.deltaTime * rotateSpeed);



                //save
                SaveObjectTransform(rootName);
            }
            //objects rotate on z axis
            if (Input.GetKey(KeyCode.Z))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.forward, Time.deltaTime * rotateSpeed);


                //save
                SaveObjectTransform(rootName);
            }

            //save 
        }


        if (Input.GetKey(KeyCode.Alpha1))
        {
            string rootName = selected.name;
            Debug.Log("[pressed 1] saving transform of: " + rootName);
            SaveObjectTransform(rootName);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            string rootName = selected.name;
            Debug.Log("[pressed 2] reading transform of: " + rootName);
            ReadObjectTransform(rootName);
        }



    }
    void SaveObjectTransform(string rootName)
    {
        Debug.Log("Saving: " + rootName);
        QuickSaveWriter.Create(rootName)
                        .Write("positionX", selected.transform.position.x)
                        .Write("positionY", selected.transform.position.y)
                        .Write("positionZ", selected.transform.position.z)


                        .Write("rotationX", selected.transform.eulerAngles.x)
                        .Write("rotationY", selected.transform.eulerAngles.y)
                        .Write("rotationZ", selected.transform.eulerAngles.z)

                        .Write("scaleX", selected.transform.localScale.x)
                        .Write("scaleY", selected.transform.localScale.y)
                        .Write("scaleZ", selected.transform.localScale.z)

                        .Commit();

    }
    void ReadObjectTransform(string rootName)
    {
        Debug.Log("reading transform of: " + rootName);

        QuickSaveReader reader = QuickSaveReader.Create(rootName);
        float positionX = reader.Read<float>("positionX");
        float positionY = reader.Read<float>("positionY");
        float positionZ = reader.Read<float>("positionZ");

        float rotationX = reader.Read<float>("rotationX");
        float rotationY = reader.Read<float>("rotationY");
        float rotationZ = reader.Read<float>("rotationZ");

        float scaleX = reader.Read<float>("scaleX");
        float scaleY = reader.Read<float>("scaleY");
        float scaleZ = reader.Read<float>("scaleZ");

        selected.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        selected.transform.position = new Vector3(positionX, positionY, positionZ);

        selected.transform.eulerAngles = new Vector3(rotationX, rotationY, rotationZ);
    }
}
