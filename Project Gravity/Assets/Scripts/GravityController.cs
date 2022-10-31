using System;
using UnityEngine;


public static class GravityController
{
    private static Vector3 _currentFacing;
    private static Guid _gravityGunEventGuid;

    // Gets initiated by the EventSystem.
    public static void Init()
    {
        _currentFacing = new Vector3(0f, -1f, 0f);
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnGravityGunHit, ref _gravityGunEventGuid);
    }

    public static bool IsGravityHorizontal()
    {
        return Physics.gravity.y == 0;
    }

    public static Vector3 GetCurrentFacing()
    {
        return _currentFacing;
    }

    public static void SetCurrentFacing(Vector3 newFacing)
    {
        _currentFacing = newFacing;
    }

    public static void SetNewGravity(Vector3 direction)
    {
        Physics.gravity = direction * Constants.GRAVITY;
    }

    public static float GetGravity()
    {
        return Constants.GRAVITY;
    }

    private static void OnGravityGunHit(GravityGunEvent gravityGunEvent)
    {
        try
        {
            SetCurrentFacing(-gravityGunEvent.hitNormal);
            SetNewGravity(-gravityGunEvent.hitNormal);
            gravityGunEvent.SourceGameObject.GetComponent<PlayerInput>().RotateToPlane();
        }
        catch
        {
            Debug.Log("Failed to properly run listening method OnGravityGunHit");
        }
        
    } 
}