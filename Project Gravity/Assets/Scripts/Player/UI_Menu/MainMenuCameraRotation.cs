using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MainMenuCameraRotation : MonoBehaviour
{
    [Header("Keeping these for troubleshooting")]
    [SerializeField] private float rotationSpeedX;
    [SerializeField] private float rotationSpeedY;
    [SerializeField] private Vector2 turn;

    [Header("Settings")] [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float startingRotationSpeed;
    [SerializeField] private int framesBeforeRotationSwitch;
    
    private static Random rnd = new Random();
    private int _counter;

    private void Start()
    {
        rotationSpeedX = startingRotationSpeed;
        rotationSpeedY = -startingRotationSpeed;
    }

    private void FixedUpdate()
    {
        if (_counter <= framesBeforeRotationSwitch)
        {
            _counter++;
        }
        else
        {
            if (rotationSpeedX > maxRotationSpeed)
            {
                rotationSpeedX -= 0.1f;
            }
            else if (rotationSpeedX < -maxRotationSpeed)
            {
                rotationSpeedX += 0.1f;
            }
            else
            {
                rotationSpeedX = NextFloat(rotationSpeedX - 0.1f, rotationSpeedX + 0.1f);
            }
            
            if (rotationSpeedY > maxRotationSpeed)
            {
                rotationSpeedY -= 0.1f;
            }
            else if (rotationSpeedY < -maxRotationSpeed)
            {
                rotationSpeedY += 0.1f;
            }
            else
            {
                rotationSpeedY = NextFloat(rotationSpeedY - 0.1f, rotationSpeedY + 0.1f);
            }
            _counter = 0;
        }

        Rotate();
    }

    static float NextFloat(float min, float max)
    {
        double val = (rnd.NextDouble() * (max - min) + min);
        return (float) val;
    }

    private void Rotate()
    {
        turn.x += rotationSpeedX / 2;
        turn.y += rotationSpeedY / 2;

        var targetRotation = Quaternion.Euler(Vector3.up * -turn.y) * Quaternion.Euler(Vector3.right * turn.x);

        transform.rotation = targetRotation;
    }
}