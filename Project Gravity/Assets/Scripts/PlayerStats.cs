﻿using System.Reflection;
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
    [Header("Add seconds (as float) for form switch cooldown")]
    [SerializeField] private float formSwitchCooldown;
    [Header("Add default form to index 0 of array")] 
    [SerializeField] private Form[] allPlayerForms;

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

    public float GetFormSwitchCooldown()
    {
        return formSwitchCooldown;
    }

    public float GetAirMovementMultiplier()
    {
        return airMovementMultiplier;
    }

    public Form[] GetAllForms()
    {
        return allPlayerForms;
    }
}