using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask gravityMask;
    [SerializeField] private LayerMask magnetMask;
    [SerializeField] private LayerMask lavaMask;
    [SerializeField] private LayerMask mirrorMask;
    private LineRenderer _lineRenderer;
    private GameObject aimDirector;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;
    private bool buttonPressed;
    private static int gravityLayer;
    private static int groundLayer;
    private static int magnetLayer;
    private static int lavaLayer;
    private static int playerLayer;
    private static int moveableLayer;
    private static int mirrorLayer;

    private void Awake()
    {
        aimDirector = GameObject.FindGameObjectWithTag("AimingDirector");
        _lineRenderer = GameObject.FindWithTag("LineRenderer").GetComponent<LineRenderer>();
        gravityLayer = LayerMask.NameToLayer("GravityChange");
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        lavaLayer = LayerMask.NameToLayer("Hazard");
        moveableLayer = LayerMask.NameToLayer("Moveable");
        mirrorLayer = LayerMask.NameToLayer("Mirror");
    }

    void FixedUpdate()
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        _currentDirection = GetMousePositionOnPlane() - transform.position;
        _lineRenderer.SetPosition(0, transform.position);

        if (buttonPressed)
        {
            if (!_lineRenderer.gameObject.activeSelf)
            {
                _lineRenderer.gameObject.SetActive(true);
            }

            SetCrosshair();
        }
        else
        {
            _lineRenderer.gameObject.SetActive(false);
        }
    }

    private void ShootMirrorLine(RaycastHit mirrorHit, Vector3 incomingDirection)
    {
        var mirrorDirection = Vector3.Reflect(incomingDirection, mirrorHit.normal);

        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().useWorldSpace = true;


        Physics.Raycast(mirrorHit.point, mirrorDirection, out var testHit, Mathf.Infinity, groundMask);


        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>()
            .SetPosition(0, mirrorHit.point);
        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().SetPosition(1, testHit.point);
    }

    private void SetCrosshair()
    {
        // Physics.Raycast(transform.position, _currentDirection, out var groundHit, Mathf.Infinity, groundMask);
        // Physics.Raycast(transform.position, _currentDirection, out var gravityHit, Mathf.Infinity,
        //     gravityMask);
        // Physics.Raycast(transform.position, _currentDirection, out var magnetHit, Mathf.Infinity,
        //     magnetMask);
        // Physics.Raycast(transform.position, _currentDirection, out var mirrorHit, Mathf.Infinity,
        //     mirrorMask);
        // Vector3 linePosition;
        //
        // if (mirrorHit.collider)
        // {
        //     // var mirrorDirection = Vector3.Reflect(_currentDirection, mirrorHit.normal);
        //     // mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().useWorldSpace = true;
        //     // Physics.Raycast(mirrorHit.point, mirrorDirection, out var testHit, Mathf.Infinity, groundMask);
        //     // mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>()
        //     //     .SetPosition(0, mirrorHit.point);
        //     // mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().SetPosition(1, testHit.point);
        // }
        //
        // if (gravityHit.collider)
        // {
        //     if (magnetHit.collider)
        //     {
        //         if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
        //             (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
        //         {
        //             if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
        //                 (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
        //             {
        //                 linePosition = SetGroundAim(groundHit);
        //             }
        //             else
        //             {
        //                 if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
        //                 {
        //                     if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
        //                     {
        //                         linePosition = SetMagnetAim(magnetHit);
        //                     }
        //                     else
        //                     {
        //                         linePosition = SetGroundAim(groundHit);
        //                     }
        //                 }
        //                 else
        //                 {
        //                     linePosition = SetMagnetAim(magnetHit);
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             linePosition = SetGravityAim(gravityHit);
        //         }
        //     }
        //     else
        //     {
        //         // Where no magnet exists 
        //         if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
        //             (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
        //         {
        //             linePosition = SetGroundAim(groundHit);
        //         }
        //         else
        //         {
        //             linePosition = SetGravityAim(gravityHit);
        //         }
        //     }
        // }
        // else if (magnetHit.collider)
        // {
        //     // Where no gravity exists but magnet does
        //     if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
        //         (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
        //     {
        //         linePosition = SetGroundAim(groundHit);
        //     }
        //     else
        //     {
        //         if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
        //         {
        //             if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
        //             {
        //                 linePosition = SetMagnetAim(magnetHit);
        //             }
        //             else
        //             {
        //                 linePosition = SetGroundAim(groundHit);
        //             }
        //         }
        //         else
        //         {
        //             linePosition = SetMagnetAim(magnetHit);
        //         }
        //     }
        // }
        // else
        // {
        //     linePosition = SetGroundAim(groundHit);
        // }
        var hits = InitRaycasts(transform.position, _currentDirection);
        var hit = GetRayCastHitToUse(transform.position, _currentDirection, hits);
        
        SetAimingColor(hit);
        
        _lineRenderer.SetPosition(1, hit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER);
    }

    private void SetAimingColor(RaycastHit hit)
    {
        int i = 0;
        if (hit.collider.gameObject.layer == gravityLayer)
        {
            EnableAimDirector(hit);
            i = 1;
        } else
        {
            DisableAimDirector();
            if (hit.collider.gameObject.layer == magnetLayer)
            {
                i = 2;
            }
            
        }
        _lineRenderer.material = lineMaterials[i];
    }

    private void EnableAimDirector(RaycastHit hit)
    {
        if (!aimDirector.GetComponent<SpriteRenderer>().enabled)
        {
            aimDirector.GetComponent<SpriteRenderer>().enabled = true;
        }

        aimDirector.transform.position = new Vector3(hit.point.x,
            hit.point.y - 1, 2);

        if (hit.normal.x != 0)
        {
            if (hit.point.x > transform.position.x)
            {
                aimDirector.transform.position = new Vector3(hit.point.x - 0.5f,
                    hit.point.y, 2);
            }
            else
            {
                aimDirector.transform.position = new Vector3(hit.point.x + 0.5f,
                    hit.point.y, 2);
            }
        }
        else if (hit.normal.y != 0)
        {
            if (hit.point.y > transform.position.y)
            {
                aimDirector.transform.position = new Vector3(hit.point.x,
                    hit.point.y - 0.5f, 2);
            }
            else
            {
                aimDirector.transform.position = new Vector3(hit.point.x,
                    hit.point.y + 0.5f, 2);
            }
        }

        aimDirector.transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
    }

    private void DisableAimDirector()
    {
        if (aimDirector.GetComponent<SpriteRenderer>().enabled)
        {
            aimDirector.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void TriggerGravityGunEvent(RaycastHit hit)
    {
        Event gravityGunEvent = new GravityGunEvent()
        {
            TargetGameObject = hit.transform.gameObject,
            SourceGameObject = gameObject,
            HitNormal = hit.normal
        };
        EventSystem.Current.FireEvent(gravityGunEvent);
    }

    private RaycastHit GetRayCastHitToUse(Vector3 position, Vector3 currentDirection, List<RaycastHit> hits)
    {
        var closestHit = hits[0];
        if (hits.Count > 1)
        {
            for (int i = 1; i < hits.Count; i++)
            {
                if (!((float) Math.Round(Vector3.Distance(position, hits[i].point), 2) >
                      (float) Math.Round(Vector3.Distance(position, closestHit.point), 2)))
                {
                    closestHit = hits[i];
                }
            }
        }

        if (closestHit.collider.gameObject.layer == mirrorLayer)
        {
            var hit = SetMirrorLine(closestHit, currentDirection);
            return hit;
        }

        if (closestHit.collider.gameObject.layer != magnetLayer) return closestHit;
        if (closestHit.transform.parent == null) return closestHit;
        if (closestHit.transform.GetComponentInParent<DynamicObjectMovement>() == null) return closestHit;
        if (!closestHit.transform.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
        {
            closestHit = hits[0];
        }

        return closestHit;
    }

    private RaycastHit SetMirrorLine(RaycastHit mirrorHit, Vector3 incomingDirection)
    {
        var mirrorDirection = Vector3.Reflect(incomingDirection, mirrorHit.normal);

        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().useWorldSpace = true;        
            
        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>()
            .SetPosition(0, mirrorHit.point);
            
        var rayCastHits = InitRaycasts(mirrorHit.collider.transform.position, mirrorDirection);
        RaycastHit toUse = GetRayCastHitToUse(mirrorHit.point, mirrorDirection, rayCastHits);
        mirrorHit.collider.transform.Find("MirrorLine").GetComponent<LineRenderer>().SetPosition(1, toUse.point);
        return toUse;
    }

    private List<RaycastHit> InitRaycasts(Vector3 startingPos, Vector3 direction)
    {
        Physics.Raycast(startingPos, direction, out var groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(startingPos, direction, out var gravityHit, Mathf.Infinity,
            gravityMask);
        Physics.Raycast(startingPos, direction, out var magnetHit, Mathf.Infinity,
            magnetMask);
        Physics.Raycast(startingPos, direction, out var lavaHit, Mathf.Infinity,
            lavaMask);
        Physics.Raycast(startingPos, direction, out var mirrorHit, Mathf.Infinity,
            mirrorMask);
        var hits = new List<RaycastHit>();

        // Ground needs to be added first for the sake of logic later
        hits.Add(groundHit);

        if (gravityHit.collider)
        {
            hits.Add(gravityHit);
        }

        if (magnetHit.collider)
        {
            hits.Add(magnetHit);
        }

        if (lavaHit.collider)
        {
            hits.Add(lavaHit);
        }

        if (mirrorHit.collider)
        {
            hits.Add(mirrorHit);
        }

        return hits;
    }

    public void ShootGravityGun()
    {
        var hits = InitRaycasts(transform.position, _currentDirection);

        TriggerGravityGunEvent(GetRayCastHitToUse(transform.position, _currentDirection, hits));
        DisableAimDirector();
    }

    public void Aim(InputAction.CallbackContext val)
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (val.performed)
        {
            buttonPressed = true;
        }
        else if (!val.performed)
        {
            buttonPressed = false;
        }

        if (val.canceled)
        {
            _lineRenderer.gameObject.SetActive(false);
            ShootGravityGun();
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 1));
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }
}