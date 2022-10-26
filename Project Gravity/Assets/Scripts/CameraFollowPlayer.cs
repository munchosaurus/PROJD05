using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float leeway = 2.0f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minX, maxX, minY, maxY;
    [SerializeField] private float maximumDistance;
    [SerializeField] private float minimumDistance;
    [SerializeField] private float targetZ;
    [SerializeField] private float targetX;
    [SerializeField] private float targetY;
    private Vector3 offSet;
    private Vector3 velocity = Vector3.zero;
    private bool move;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        // JUST FOR TESTING, EACH LEVEL WILL HAVE ITS OWN LOAD OF TARGET FOR CAMERA
        targetZ = minimumDistance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (targetZ <= maximumDistance + 1)
        {
            targetPosition = new Vector3(targetX, targetY, targetZ);
        }
        else
        {
            targetPosition = CalculateTargetPosition();
        }

        if (Vector2.Distance(targetPosition, transform.position) > leeway && !move)
        {
            
            move = true;
        }
       
        MoveCamera(targetPosition);
    }

    private void MoveCamera(Vector3 targetPosition)
    {
        if (move)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
            {
                move = false;
            }
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, targetZ);
        
        if (targetPosition.x < minX)
            targetPosition.x = minX;
        if (targetPosition.x > maxX)
            targetPosition.x = maxX;
        if (targetPosition.y < minY)
            targetPosition.y = minY;
        if (targetPosition.y > maxY)
            targetPosition.y = maxY;

        
        return targetPosition;
    }

    public void Zoom(InputAction.CallbackContext val)
    {
        if (val.ReadValue<Vector2>().y > 0)
        {
            if (targetZ < minimumDistance)
            {
                targetZ += 0.5f;
            }
        }
        else if (val.ReadValue<Vector2>().y < 0)
        {
            if (targetZ > maximumDistance)
            {
                targetZ -= 0.5f;
            }
        }
    }
}