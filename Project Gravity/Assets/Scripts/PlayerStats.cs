using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float playerMovementAcceleration;
    [SerializeField] private float playerMovementDecelleration;
    [SerializeField] private float jumpForceMultiplier;

    [Header("Add a float to multiply with the player movement acceleration for when player is in air")] [SerializeField]
    private float airMovementMultiplier;

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