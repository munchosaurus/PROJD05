using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraAngles : MonoBehaviour
{
    [SerializeField] private Vector2 turn;
    [SerializeField] private float sensitivity;
    [SerializeField] private float returnSpeed;

    private bool _rotationToggled;
    public float minXRotation;
    public float maxXRotation;
    public float minYRotation;
    public float maxYRotation;

    private void Update()
    {
        if (_rotationToggled)
        {
            Rotate();
        }
        else if (GameController.CameraAutoRotationToggled)
        {
            transform.rotation =
                Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            // Resets the rotation input
            turn = new Vector2();
        }
    }

    // Takes the mouse position and translates it to a new rotation for the camera object
    // Rotates and moves the camera object in scene
    private void Rotate()
    {
        turn.x += Mouse.current.delta.y.ReadValue() * sensitivity;
        turn.y += Mouse.current.delta.x.ReadValue() * sensitivity;

        turn.x = Mathf.Clamp(turn.x, minXRotation, maxXRotation);
        turn.y = Mathf.Clamp(turn.y, minYRotation, maxYRotation);

        var targetRotation = Quaternion.Euler(Vector3.up * -turn.y) * Quaternion.Euler(Vector3.right * turn.x);

        transform.rotation = targetRotation;
    }

    // Sets the rotation toggle to true if pressed
    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        _rotationToggled = context.ReadValue<float>() == 1;
    }

    public void RotateToDefault(InputAction.CallbackContext context)
    {
        if (context.started && !_rotationToggled && !GameController.CameraAutoRotationToggled)
        {
            turn = new Vector2();
            transform.rotation = Quaternion.identity;
        }
    }
}