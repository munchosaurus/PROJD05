using System;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    private PlayerMovement _playerController;
    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        _playerController = gameObject.GetComponent<PlayerMovement>();
    }

    private void TriggerGravityGunEvent(RaycastHit hit)
    {
        Event gravityGunEvent = new GravityGunEvent()
        {
            TargetGameObject = hit.transform.gameObject,
            SourceGameObject = gameObject,
            hitNormal = hit.normal
        };
        EventSystem.Current.FireEvent(gravityGunEvent);
    }

    public void ShootGravityGun()
    {
        RaycastHit groundHit;
        RaycastHit hit;
        Vector3 direction = GetMousePositionOnPlane() - transform.position;

        Physics.Raycast(transform.position, direction, out groundHit, Mathf.Infinity, groundMask);
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity,
                _playerController.gravityChangeLayer,
                QueryTriggerInteraction.Collide) && GravityController.GetCurrentFacing() !=
            -hit.normal * GravityController.GetGravity())
        {
            if (Vector3.Distance(transform.position, hit.point) <=
                Vector3.Distance(transform.position, groundHit.point))
            {
                TriggerGravityGunEvent(hit);
            }
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }


    // For troubleshooting
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, GetMousePositionOnPlane() - transform.position, Color.green);
    }
}