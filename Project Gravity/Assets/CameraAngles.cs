using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraAngles : MonoBehaviour
{

    [SerializeField] private Vector2 turn;
    [SerializeField] private float sensitivity;
    [SerializeField] private bool autoReturn;
    
    private float targetYRotation;
    private float targetXRotation;
    private float cameraSmoothness = 0.5f;
    
    private bool _rightMouseDown;
    public float minXRotation = -10;
    public float maxXRotation = 10;
    public float minYRotation = -10;
    public float maxYRotation = 10;
    private void Update()
    {
        if (_rightMouseDown)
        {
            turn.x += Mouse.current.delta.y.ReadValue() * sensitivity;
            turn.y += Mouse.current.delta.x.ReadValue() * sensitivity;
            transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
            
            turn.x = Mathf.Clamp(turn.x, minXRotation, maxXRotation);
            turn.y = Mathf.Clamp(turn.y, minYRotation, maxYRotation);
            
            var targetRotation = Quaternion.Euler(Vector3.up * -turn.y) * Quaternion.Euler(Vector3.right * turn.x);

            transform.rotation = targetRotation;

            //Quaternion q = ClampRotation(Quaternion.Euler(-turn.y, turn.x, 0), new Vector3(10, 10, 0));
            //transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * 0.75f);
            //transform.localRotation = q;
            //transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);

        } else if (autoReturn)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 3);
            turn = new Vector2();
        }
    }
    
    public Quaternion RotateForward()
    {
        return Quaternion.LookRotation(transform.forward, new Vector3());
    }

    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        _rightMouseDown = context.ReadValue<float>() == 1;
    }
    
    public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
 
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
 
        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
 
        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);
 
        return q;
    }
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        // if (_rightMouseDown)
        // {
        //     turn.x = Mouse.current.delta.x.ReadValue();
        //     turn.y = Mouse.current.delta.y.ReadValue();
        // }
        // else
        // {
        //     turn = new Vector2();
        // }
    }
}
