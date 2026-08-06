using System;
using UnityEngine;

[Serializable]
public class ToolkitPlayerData
{
    public bool flying;
    public float speed;
    public float gravity;
    public float jumpSpeed;
    public float minFall;
    public float pushForce;
    public Vector3 position;
    public Vector3 rotation;
    public float verticalLookAngle;
}
