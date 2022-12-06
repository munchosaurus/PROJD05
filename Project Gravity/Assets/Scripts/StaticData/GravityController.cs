using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public static class GravityController
{
    private static Vector3 _currentFacing;
    private static Guid _gravityGunEventGuid;
    static int gravityLayer = LayerMask.NameToLayer("GravityChange");
    public static float GravityMultiplier;

    // Gets initiated by the EventSystem.
    public static void Init()
    {
        _currentFacing = new Vector3(0f, -1f, 0f);
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnGravityGunHit, ref _gravityGunEventGuid);
    }

    public static void SetUp()
    {
        GravityMultiplier = 1;
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
        Physics.gravity = direction * Constants.GRAVITY * GravityMultiplier * GameController.GlobalSpeedMultiplier;
    }

    private static void OnGravityGunHit(GravityGunEvent gravityGunEvent)
    {
        if (gravityGunEvent.TargetGameObject.layer == gravityLayer)
        {
            try
            {
                if (Physics.gravity != -gravityGunEvent.HitNormal * Constants.GRAVITY * GravityMultiplier * GameController.GlobalSpeedMultiplier)
                {
                    CompletionLogger.gravityChanges++;
                    SetCurrentFacing(-gravityGunEvent.HitNormal);
                    SetNewGravity(-gravityGunEvent.HitNormal);
                    gravityGunEvent.SourceGameObject.GetComponent<PlayerController>().RotateToPlane();
                }
            }
            catch
            {
                Debug.Log("Failed to properly run listening method OnGravityGunHit");
            }
        }
    } 
}