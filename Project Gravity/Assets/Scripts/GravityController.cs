using System;
using UnityEngine;


public static class GravityController
{
    private static bool _isHorizontal = true;
    private static Vector3 _currentFacing;
    private static Guid _gravityGunEventGuid;

    // Gets initiated by the EventSystem.
    public static void Init()
    {
        _currentFacing = new Vector3(0f, 1f, 0f);
        _isHorizontal = true;
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnGravityGunHit, ref _gravityGunEventGuid);
    }

    public static bool IsGravityHorizontal()
    {
        return _isHorizontal;
    }

    public static void SetHorizontal(bool horizontal)
    {
        _isHorizontal = horizontal;
    }

    public static Vector3 GetCurrentFacing()
    {
        return _currentFacing;
    }

    public static void SetCurrentFacing(Vector3 newFacing)
    {
        _currentFacing = newFacing;
    }

    public static void SetNewGravity()
    {
        Physics.gravity = -_currentFacing * Constants.GRAVITY;
    }

    public static float GetGravity()
    {
        return Constants.GRAVITY;
    }

    private static void OnGravityGunHit(GravityGunEvent gravityGunEvent)
    {
        try
        {
            SetCurrentFacing(gravityGunEvent.hitNormal);
            SetNewGravity();
            gravityGunEvent.SourceGameObject.GetComponent<PlayerMovement>().RotateToPlane();
            SetHorizontal(_isHorizontal = _currentFacing.y != 0);
        }
        catch
        {
            Debug.Log("Failed to properly run listening method OnGravityGunHit");
        }
        
    } 
}