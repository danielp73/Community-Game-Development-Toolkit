using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class ExampleSaveController : MonoBehaviour
{

    private string rootName1;
    //private string rootName2;
    //private string rootName3;

    // Start is called before the first frame update
    void Start()
    {
        rootName1 = gameObject.name;
        //rootName2 = gameObject.name;
        //rootName3 = gameObject.name;
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
        QuickSaveReader reader1 = QuickSaveReader.Create(rootName1);
        float x1 = reader1.Read<float>("x1");
        float y1 = reader1.Read<float>("y1");
        float z1 = reader1.Read<float>("z1");

        float x2 = reader1.Read<float>("x2");
        float y2 = reader1.Read<float>("y2");
        float z2 = reader1.Read<float>("z2");

        float x3 = reader1.Read<float>("x3");
        float y3 = reader1.Read<float>("y3");
        float z3 = reader1.Read<float>("z3");

        transform.localScale = new Vector3(x3, y3, z3);

        transform.position = new Vector3(x1, y1, z1);

        transform.eulerAngles = new Vector3(x2, y2, z2);
        
    }

    public void SaveTransform()
    {
        QuickSaveWriter.Create(rootName1)
                        .Write("x1", transform.position.x)
                        .Write("y1", transform.position.y)
                        .Write("z1", transform.position.z)


                        .Write("x2", transform.eulerAngles.x)
                        .Write("y2", transform.eulerAngles.y)
                        .Write("z2", transform.eulerAngles.z)

                        .Write("x3", transform.localScale.x)
                        .Write("y3", transform.localScale.y)
                        .Write("z3", transform.localScale.z)

                        .Commit();
      }                       
}
