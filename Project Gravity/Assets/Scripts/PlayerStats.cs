using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    [Header("Main stats")] [SerializeField]
    private float jumpForce;

    [SerializeField] private float jumpCooldown;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float playerMovementAcceleration;
    [SerializeField] private float playerMovementDecelleration;
    [SerializeField] private float jumpForceMultiplier;
    [SerializeField] private float airMovementMultiplier;

    [Header("Alternative stats")] [SerializeField]
    private float jumpForceAlternative;

    [SerializeField] private float jumpCooldownAlternative;
    [SerializeField] private float maxVelocityAlternative;
    [SerializeField] private float playerMovementAccelerationAlternative;
    [SerializeField] private float playerMovementDecellerationAlternative;
    [SerializeField] private float jumpForceMultiplierAlternative;
    [SerializeField] private float airMovementMultiplierAlternative;

    public float GetJumpForceAlternative()
    {
        return jumpForceAlternative;
    }

    public float GetJumpCooldownAlternative()
    {
        return jumpCooldownAlternative;
    }

    public float GetMaxVelocityAlternative()
    {
        return maxVelocityAlternative;
    }

    public float GetPlayerMovementAccelerationAlternative()
    {
        return playerMovementAccelerationAlternative;
    }

    public float GetPlayerMovementDecellerationAlternative()
    {
        return playerMovementDecellerationAlternative;
    }

    public float GetJumpForceMultiplierAlternative()
    {
        return jumpForceMultiplierAlternative;
    }


    public float GetAirMovementMultiplierAlternative()
    {
        return airMovementMultiplierAlternative;
    }
    
    public float GetJumpForce()
    {
        return jumpForce;
    }

    public float GetJumpCooldown()
    {
        return jumpCooldown;
    }

    public float GetMaxVelocity()
    {
        return maxVelocity;
    }

    public float GetPlayerMovementAcceleration()
    {
        return playerMovementAcceleration;
    }

    public float GetPlayerMovementDecelleration()
    {
        return playerMovementDecelleration;
    }

    public float GetJumpForceMultiplier()
    {
        return jumpForceMultiplier;
    }


    public float GetAirMovementMultiplier()
    {
        return airMovementMultiplier;
    }
}