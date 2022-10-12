using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float leeway = 2.0f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minX, maxX, minY, maxY;

    private Vector3 offSet;
    private Vector3 velocity = Vector3.zero;
    private bool move;

    // Start is called before the first frame update
    void Start()
    {
        offSet = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        Vector3 targetPosition = CalculateTaretPosition();

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

    private Vector3 CalculateTaretPosition()
    {
        Vector3 targetPosition = playerTransform.position + offSet;

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
}
