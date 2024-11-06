using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class BoxPlayerMove : MonoBehaviour
{
    //movement
    [SerializeField] private float gravity = -40f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyotetime = 0.1f;
    [SerializeField] private float jumpbuffer = 0.1f;
    [SerializeField] private float maxFailspeed = -30f;

    //input
    [SerializeField][Range(0, 10)] private float Sensitivity = 9.61f;
    [SerializeField][Range(0, 0.1f)] private float dedzone = 0.01f;

    //gizmos
    [SerializeField] private float maxTailTime = 10f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float tailRadius = 0.1f;
    [SerializeField] private Color sphereColor = Color.red;
    [SerializeField] private Color tailColor = Color.green;
    private List<Vector3> jumpPositions = new List<Vector3>();
    private Vector3 lastJumpPosition;
    private Vector3 lastJumpPositionForArray;
    private bool hasJumped = false;

    //input control
    private BoxPlayerAction inputActions;
    private InputAction movement;
    private InputAction jumping;
    private InputAction rotatCon;
    private InputAction rotatMos;
    private Rigidbody myrigidbody;

    //control bool
    private bool jumpbufferbool = false;
    private bool onGround;
    

    //read value
    private Vector3 moveVector;
    private Vector3 mousePos;
    private Vector3 conPos;
    private float jumpSpeed;

    //phisics control
    private float maxGroundSlope = 60f;

    //state
    enum jumpState
    {
        isGround,
        isJump,
    }
    private jumpState state;


    void Awake()
    {
        //input control
        inputActions = new BoxPlayerAction();

        movement = inputActions.player.Move;
        rotatCon = inputActions.player.conRat;
        rotatMos = inputActions.player.mosRat;

        jumping = inputActions.player.jump;

        //jump check
        state = jumpState.isGround;

        //rigidbody control
        myrigidbody = GetComponent<Rigidbody>();
        myrigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        myrigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        myrigidbody.interpolation = RigidbodyInterpolation.Interpolate;

    }

    void Start()
    {
        jumpSpeed = jumpForce;
    }

    void Update()
    {
        //data reading

        moveVector = new Vector3(movement.ReadValue<Vector2>().x, 0, movement.ReadValue<Vector2>().y);
        mousePos = rotatMos.ReadValue<Vector2>();
        conPos = rotatCon.ReadValue<Vector2>();

        //gizmo draw
        UpdateJumpPositions();
    }

    void FixedUpdate()
    {
        if(CameraControl.Instance.isNotClick())
        {
            Movecontrol();
            JumpControl();
            Rotatcontrol();
        }
    }

    void Movecontrol()
    {
        //movement
        Vector3 current = myrigidbody.velocity;

        moveVector = transform.TransformDirection(moveVector);

        myrigidbody.velocity = new Vector3(moveVector.x * moveSpeed, Mathf.Clamp(current.y, maxFailspeed, float.MaxValue), moveVector.z * moveSpeed);
        
    }

    void Rotatcontrol()
    {
        //rotation
        Vector3 current = transform.rotation.eulerAngles;
        
        float mouseA = mousePos.x;
        float contlA = conPos.x;

        float mouseY = Mathf.LerpAngle(current.y, mouseA, rotationSpeed * Time.fixedDeltaTime);
        float contlY = Mathf.LerpAngle(current.y, DataCustom.CustomData(contlA, Sensitivity), rotationSpeed * Time.fixedDeltaTime);
        

        Vector3 mouTar = new Vector3(current.x, current.y + mouseY, current.z);
        Vector3 conTar = new Vector3(current.x, current.y + contlY, current.z);

        if(Mouse.current != null && Mouse.current.position.ReadValue().magnitude > dedzone)
        {
            transform.rotation = Quaternion.Euler(mouTar);
        }
        if(Gamepad.current != null && Gamepad.current.rightStick.ReadValue().magnitude > dedzone)
        {
            transform.rotation = Quaternion.Euler(conTar);
        }
    }

    void JumpControl()
    {
        switch (state)
        {
            case jumpState.isGround:
                bool jumpCheck = jumping.IsPressed();
                
                if(onGround && jumpCheck || jumpbufferbool)
                {
                    if(!(onGround && jumpCheck) && jumpbufferbool)
                    {
                        Debug.Log("Jumpbuffer!!!");
                    }
                    myrigidbody.velocity = new Vector3(myrigidbody.velocity.x, Mathf.Clamp(jumpSpeed, maxFailspeed, float.MaxValue), myrigidbody.velocity.z);

                    //Jump graphics drawing controls
                    gizmoData();

                    state = jumpState.isJump;
                    onGround = false;
                    jumpbufferbool = false;
                }

                break;
            case jumpState.isJump:
                //apply gravity
                jumpSpeed += gravity * Time.fixedDeltaTime;
                myrigidbody.velocity = new Vector3(myrigidbody.velocity.x, Mathf.Clamp(jumpSpeed, maxFailspeed, float.MaxValue), myrigidbody.velocity.z);

                //jumpbuffer
                jumpbufferCheck();
                
                if(onGround)
                {
                    state = jumpState.isGround;
                    jumpSpeed = jumpForce;
                }
                break;
        }
        
        //coyotetime
        coyotetimeCheck();

    }

    void coyotetimeCheck()
    {
        if(!onGround)
        {
            Timer cbuffer = new Timer(coyotetime);
            cbuffer.cycle = false;

            if(!cbuffer.Tick() && jumping.IsPressed())
            {
                // Debug.Log("Coyotetime!!!");
                state = jumpState.isJump;
            }
        }
    }

    void jumpbufferCheck()
    {
         if(jumping.IsPressed())
        {
            Timer jbuffer = new Timer(jumpbuffer);
            jbuffer.cycle = false;

            if(!jbuffer.Tick() && onGround)
            {
                jumpbufferbool = true;
            }
        }
    }

    void UpdateJumpPositions()
    {
        lastJumpPositionForArray = transform.position;
        jumpPositions.Add(lastJumpPositionForArray);
        
        if (jumpPositions.Count > maxTailTime/Time.deltaTime)
        {
            jumpPositions.RemoveAt(0);
        }
    }

    void gizmoData()
    {
        hasJumped = true;
        lastJumpPosition = transform.position;
    }

    void OnDrawGizmos()
    {
        

        for (int i = 0; i < jumpPositions.Count - 1; i++)
        {
            Gizmos.color = tailColor;
            Gizmos.DrawLine(jumpPositions[i], jumpPositions[i + 1]);
            Gizmos.DrawWireSphere(jumpPositions[i], tailRadius);
        }
        
        if (hasJumped)
        {
            Gizmos.color = sphereColor;
            Gizmos.DrawWireSphere(lastJumpPosition, sphereRadius);
        }
    }

    void OnEnable()
    {
        movement.Enable();
        jumping.Enable();
        rotatCon.Enable();
        rotatMos.Enable();
    }

    void OnDisable()
    {
        movement.Disable();
        jumping.Disable();
        rotatCon.Disable();
        rotatMos.Disable();
    }

    void OnCollisionEnter(Collision collision)
    {
        onGround = OnGround(collision);
    }

    private bool OnGround(Collision collision) 
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < maxGroundSlope)
            {
                return true;
            }
        }

        return false;
    }

    public bool getOnGround()
    {
        return onGround;
    }

}
