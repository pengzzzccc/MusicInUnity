using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoomForOrt = 20f;
    [SerializeField] private float minZoomForOrt = 5f;
    [SerializeField] private float maxZoomForPersp = 90f;
    [SerializeField] private float minZoomForPersp = 10f;
    [SerializeField][Range(0 ,10)] private float zoomSensitivity = 5f;
    [SerializeField][Range(0 ,10)] private float rollSensitivity = 5f;
    private PlayerAction inputAction;
    private InputAction roll;
    private InputAction click;
    private InputAction delta;
    private InputAction moveCon;
    private Camera cam;
    static private CameraControl instance;
    static public CameraControl Instance
    {
        get{
            if(instance == null)
            {
                Debug.LogError("There is no CameraControl instance in the scene.");
            }
            return instance;
        }
    }
    private float zoomSpeeds;
    private float maxForOrtl;
    private float minForOrtl;
    private float maxForPersp;
    private float minForPersp;
    private float sensitiveZoom;
    private float sensitiveRoll;

    void Awake()
    {
        inputAction = new PlayerAction();
        roll = inputAction.Camera.Roll;
        click = inputAction.Camera.Click;
        delta = inputAction.Camera.Move;
        moveCon = inputAction.Camera.MoveCon;

        cam = GetComponent<Camera>();

        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    
    void Start()
    {
        //Initialize Zoom
        zoomSpeeds = zoomSpeed;
        maxForOrtl = maxZoomForOrt;
        minForOrtl = minZoomForOrt;
        maxForPersp = maxZoomForPersp;
        minForPersp = minZoomForPersp;
        sensitiveZoom = zoomSensitivity;
        sensitiveRoll = rollSensitivity;
    }

    void Update()
    {
        ZoomCam();
        MoveCam();
    }

    void MoveCam()
    {
        // Vector2 deltaInput = delta.ReadValue<Vector2>();
        float conInY = Mathf.Clamp(DataCustom.CustomData(moveCon.ReadValue<float>(), sensitiveRoll), -sensitiveRoll, sensitiveRoll);
        float deltaY = Mathf.Clamp(delta.ReadValue<Vector2>().y, -sensitiveRoll, sensitiveRoll);

        Vector3 current = transform.parent.rotation.eulerAngles;
        
        //rotate angle limit
        if(click.ReadValue<float>() > 0)
        {
            transform.parent.Rotate(Vector3.right * deltaY);
            transform.parent.Rotate(Vector3.right * conInY);
        }
    }

    void ZoomCam()
    {
        float zoomInput = -DataCustom.CustomData(roll.ReadValue<float>(), sensitiveZoom);
        
        if(cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize + zoomInput * Time.deltaTime * zoomSpeeds
                , minForOrtl
                , maxForOrtl);
        }
        else
        {
            cam.fieldOfView = Mathf.Clamp(
                cam.fieldOfView + zoomInput * Time.deltaTime  * zoomSpeeds
                , minForPersp
                , maxForPersp);
        }
    }

    public bool isNotClick()
    {
        return click.ReadValue<float>() == 0;
    }

    
    void OnEnable()
    {
        roll.Enable();
        click.Enable();
        delta.Enable();
        moveCon.Enable();
    }

    void OnDisable()
    {
        roll.Disable();
        click.Disable();
        delta.Disable();
        moveCon.Disable();
    }
}
