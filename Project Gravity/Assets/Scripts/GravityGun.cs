using System;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    private PlayerMovement _playerController;

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
        RaycastHit hit;
        Vector3 direction = GetMousePositionOnPlane() - transform.position;

        // Might want to take a proper look at the maxDistance
        // TODO: Look at the maxdistance
        
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, _playerController.gravityChangeLayer,
                QueryTriggerInteraction.Collide) && hit.normal == hit.collider.transform.right)
        {
            Debug.Log("Ska trigga event");
            TriggerGravityGunEvent(hit);
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