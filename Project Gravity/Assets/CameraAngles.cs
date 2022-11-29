using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraAngles : MonoBehaviour
{
    //Rotation variables
    private bool _rightMouseDown = false;
    private const float InternalRotationSpeed = 1;
    private float RotationSpeed = 1f;
    private Quaternion _rotationTarget;
    private Vector2 _mouseDelta;
    private Camera _actualCamera;
    
    void Start()
    {
        //Store a reference to the camera rig
        _actualCamera = GetComponent<Camera>();

        // //Set the rotation of the camera based on the CameraAngle property
        // _actualCamera.transform.rotation = Quaternion.AngleAxis(gameObject., Vector3.right);
        //
        // //Set the position of the camera based on the look offset, angle and default zoom properties. This will make sure we're focusing on the right focal point.
        // _actualCamera.transform.position = _cameraPositionTarget;

        //Set the initial rotation value
        _rotationTarget = transform.rotation;

    }
    
    private void LateUpdate()
    {
        // //Lerp the camera rig to a new move target position
        // transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpeed);
        //
        // //Move the _actualCamera's local position based on the new zoom factor
        // _actualCamera.transform.localPosition = Vector3.Lerp(_actualCamera.transform.localPosition, _cameraPositionTarget, Time.deltaTime * _internalZoomSpeed);

        //Slerp the camera rig's rotation based on the new target
        Debug.Log(_rotationTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotationTarget, Time.deltaTime * InternalRotationSpeed);
    }
    
    /// <summary>
    /// Sets whether the player has the right mouse button down
    /// </summary>
    /// <param name="context"></param>
    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        _rightMouseDown = context.ReadValue<float>() == 1;
    }
    
    /// <summary>
    /// Sets the rotation target quaternion if the right mouse button is pushed when the player is moving the mouse
    /// </summary>
    /// <param name="context"></param>
    public void OnRotate(InputAction.CallbackContext context)
    {
        // If the right mouse is down then we'll read the mouse delta value. If it is not, we'll clear it out.
        // Note: Clearing the mouse delta prevents a 'death spin' from occuring if the player flings the mouse really fast in a direction.
        _mouseDelta = _rightMouseDown ? context.ReadValue<Vector2>() : Vector2.zero;

        _rotationTarget *= Quaternion.AngleAxis(_mouseDelta.x * Time.deltaTime * RotationSpeed, Vector3.up);

    }
}
