using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMoveGrounded : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -10f;
    public float jumpSpeed = 15.0f;
    public float minFall = -1.5f;
    public float pushForce = 3.0f;
        
    private float _vertSpeed;

    private CharacterController _charController;

    private ToolkitInput input;

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;

        input = GetComponent<ToolkitInput>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = input.Move;
        if(move.SqrMagnitude() > .01)
        {
            Debug.Log(move);
        }
        Vector3 movement = new Vector3(move.x, 0, move.y) * speed;

        movement = Vector3.ClampMagnitude(movement, speed);

        //check if character is on the ground
        if (_charController.isGrounded)
        {
            //check if we've pressed 'jump'
            if (input.JumpPressed)
            {
                //we're on the ground and pressed jump
                //so add jumpSpeed (a positive vertical speed)
                //to our vertical speed
                _vertSpeed += jumpSpeed;
            }
            else
            {
                //we're on the ground but didn't press jump
                //keep a minimum vertical speed (negative)
                //so we stay on the ground
                _vertSpeed = minFall;
            }
        }
        else
        {
            //character is not on the ground. don't check
            //if we pressed jump. add gravity (negative to our
            //vertical speed
            _vertSpeed += gravity * Time.deltaTime;
        }

        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        _charController.Move(movement);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            Debug.Log("pushing");
            body.linearVelocity = hit.moveDirection * pushForce;
        }
    }


}