using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class CameraFollowPlayer : MonoBehaviour
{
    
    private float leeway;
    private float smoothTime;
    private float minX, maxX, minY, maxY;
    private float maximumDistance;
    private float minimumDistance;
    private float targetX;
    private float targetY;
    private float targetZ;
    private LevelSettings _levelSettings;

    private Transform _playerTransform;
    private Vector3 _velocity = Vector3.zero;
    private bool _move;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        _levelSettings = (LevelSettings) FindObjectOfType (typeof(LevelSettings));
        SetupCameraSettings();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void SetupCameraSettings()
    {
        leeway = _levelSettings.GetLevelLeeway();
        smoothTime = _levelSettings.GetLevelSmoothTime();
        minX = _levelSettings.GetLevelMinX();
        maxX = _levelSettings.GetLevelMaxX();
        minY = _levelSettings.GetLevelMinY();
        maxY = _levelSettings.GetLevelMaxY();
        maximumDistance = _levelSettings.GetLevelCameraMaximumDistance();
        minimumDistance = _levelSettings.GetLevelCameraMinimumDistance();
        targetX = _levelSettings.GetLevelXTargetValue();
        targetY = _levelSettings.GetLevelYTargetValue();
        targetZ = _levelSettings.GetLevelZTargetValue();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        targetPosition = CalculateTargetPosition();

        if ((Vector2.Distance(targetPosition, transform.position) > leeway || Mathf.Abs(transform.position.z - targetZ) > 0.1f) && !_move)
        {
            _move = true;
        }
       
        MoveCamera(targetPosition);
    }

    private void MoveCamera(Vector3 targetPosition)
    {
        if (_move)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);

            if (Vector2.Distance(targetPosition, transform.position) <= 0 || transform.position.z <= targetZ + 0.1f )
            {
                _move = false;
            }
        }
        //else if (targetZ < maximumDistance + 0.1f && (Mathf.Abs(transform.position.x - targetX) > 0.1f || Mathf.Abs(transform.position.y - targetY) > 0.1f))
        //{
        //    targetPosition = new Vector3(targetX, targetY, targetZ);
        //    transform.position = transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //}
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, targetZ);
        
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