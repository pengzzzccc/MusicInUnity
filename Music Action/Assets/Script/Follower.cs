using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Follower : MonoBehaviour
{
    // [SerializeField] private Transform target;
    [SerializeField] private float mixMoveSpeed = 5f;
    [SerializeField][Range(0,50)] private float limit = 30f;
    private Transform targets = null;
    private float speed;
    private float angle;

    void Start()
    {
        // targets = target;
        targets = FindObjectOfType<BoxPlayerMove>().transform;
        if(targets == null)
        {
            Debug.LogWarning("No Player !!");
        }
        speed = mixMoveSpeed;
        angle = limit;
    }

    void Update()
    {
        if(CameraControl.Instance.isNotClick())
        {
            follow();
        }
        Clamp();
    }
    
    void follow()
    {
        Vector3 current = transform.rotation.eulerAngles;
        Vector3 target = targets.rotation.eulerAngles;
        
        //Euler angle transformation
        float currentX = current.x;
        float targetY = Mathf.LerpAngle(current.y, target.y, Time.deltaTime * speed);
        float targetZ = Mathf.LerpAngle(current.z, target.z, Time.deltaTime * speed);

        Vector3 targetRotate = new Vector3(currentX, targetY, targetZ);
        
        //application
        transform.rotation = Quaternion.Euler(targetRotate); 
        transform.position = Vector3.Lerp(transform.position, targets.position, Time.deltaTime * speed);
    } 

    void Clamp()
    {
        Vector3 current = transform.rotation.eulerAngles;
        float now = DataCustom.CheckAngle(current.x);
        if(now > angle)
            transform.eulerAngles = new Vector3(angle, current.y, current.z);
        if(now < -angle)
            transform.eulerAngles = new Vector3(-angle, current.y, current.z);
    }

}
