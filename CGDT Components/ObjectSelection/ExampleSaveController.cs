using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class ExampleSaveController : MonoBehaviour
{

    private string rootName;

    // Start is called before the first frame update
    void Start()
    {
        rootName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            Debug.Log("saving transform");
            SaveTransform();
        }

        if(Input.GetKey(KeyCode.Alpha2))
        {
            Debug.Log("reading transform");
            ReadTransform();
        }
        
    }

    

    //private void OnMouseUp()
    //{
    //    QuickSaveReaderExample();
    //}

    public void ReadTransform()
    {
        QuickSaveReader reader = QuickSaveReader.Create(rootName);
        float x = reader.Read<float>("x");
        float y = reader.Read<float>("y");
        float z = reader.Read<float>("z");

        transform.position = new Vector3(x, y, z);
    }

    public void SaveTransform()
    {
        QuickSaveWriter.Create(rootName)
                        .Write("x", transform.position.x)
                        .Write("y", transform.position.y)
                        .Write("z", transform.position.z)
                        .Commit();
    }
}
