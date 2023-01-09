using UnityEngine;
using Random = System.Random;

public class MainMenuCameraRotation : MonoBehaviour
{
    [Header("Keeping these for troubleshooting")] [SerializeField]
    private float rotationSpeedX;

    [SerializeField] private float rotationSpeedY;
    [SerializeField] private Vector2 turn;

    [Header("Settings")] [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float startingRotationSpeed;
    [SerializeField] private int framesBeforeRotationSwitch;
    [SerializeField] private float rotationChangeValue;

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
                rotationSpeedX -= rotationChangeValue;
            }
            else if (rotationSpeedX < -maxRotationSpeed)
            {
                rotationSpeedX += rotationChangeValue;
            }
            else
            {
                rotationSpeedX = NextFloat(rotationSpeedX - rotationChangeValue, rotationSpeedX + rotationChangeValue);
            }

            if (rotationSpeedY > maxRotationSpeed)
            {
                rotationSpeedY -= rotationChangeValue;
            }
            else if (rotationSpeedY < -maxRotationSpeed)
            {
                rotationSpeedY += rotationChangeValue;
            }
            else
            {
                rotationSpeedY = NextFloat(rotationSpeedY - rotationChangeValue, rotationSpeedY + rotationChangeValue);
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
        turn.x += rotationSpeedX;
        turn.y += rotationSpeedY;

        var targetRotation = Quaternion.Euler(Vector3.up * -turn.y) * Quaternion.Euler(Vector3.right * turn.x);

        transform.rotation = targetRotation;
    }
}