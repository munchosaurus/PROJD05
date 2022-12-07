using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask gravityMask;
    [SerializeField] private LayerMask magnetMask;
    [SerializeField] private LayerMask lavaMask;
    [SerializeField] private LayerMask mirrorMask;
    [SerializeField] public Color magnetHitColor;
    [SerializeField] public Color groundHitColor;
    [SerializeField] public Color gravityHitColor;
    [SerializeField] public Color alternativeMagnetHitColor;
    [SerializeField] public Color alternativeGroundHitColor;
    [SerializeField] public Color alternativeGravityHitColor;
    [SerializeField] private PlayerAimEffectController _playerAimEffectController;
    [SerializeField] private AudioClip playerAims;
    [SerializeField] private AudioClip playerShoots;
    [SerializeField] private AudioSource playerShotAudioSource;
    [SerializeField] private float gunDelay;
    private PlayerInput playerInput;
    //private LineRenderer _lineRenderer;
    private GameObject aimDirector;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;
    private bool buttonPressed;
    private static int gravityLayer;
    private static int mirrorLayer;
    private static int mirrorsHit;
    private static int lastMirrorHit;
    private static int magnetLayer;
    private static int groundLayer;
    private static int lavaLayer;
    private static int playerLayer;
    private static int moveableLayer;


    private void Awake()
    {
        aimDirector = GameObject.FindGameObjectWithTag("AimingDirector");
        //_lineRenderer = GameObject.FindWithTag("LineRenderer").GetComponent<LineRenderer>();
        gravityLayer = LayerMask.NameToLayer("GravityChange");
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        lavaLayer = LayerMask.NameToLayer("Hazard");
        moveableLayer = LayerMask.NameToLayer("Moveable");
        mirrorLayer = LayerMask.NameToLayer("Mirror");
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void FixedUpdate()
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        _currentDirection = GetMousePositionOnPlane() - transform.position;
        //_lineRenderer.SetPosition(0, transform.position);

        if (buttonPressed)
        {
            playerShotAudioSource.clip = playerAims;
            playerShotAudioSource.Play();
            // if (!_lineRenderer.gameObject.activeSelf)
            // {
            //     _lineRenderer.gameObject.SetActive(true);
            // }
            
            if (!_playerAimEffectController.gameObject.activeSelf)
            {
                _playerAimEffectController.gameObject.SetActive(true);
            }

            SetCrosshair();
        }
        else
        {
            _playerAimEffectController.gameObject.SetActive(false);
        }
    }

    private void SetCrosshair()
    {
        var hits = InitRaycasts(transform.position, _currentDirection);
        var hit = GetFinalGravityGunHit(transform.position, _currentDirection, hits);

        var lineSpot = GetSingleRay(transform.position, hits);

        SetAimingColor(hit);

        //_lineRenderer.SetPosition(1, lineSpot.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER);
        _playerAimEffectController.SetAim(transform.position,lineSpot.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER);
    }

    private void SetAimingColor(RaycastHit hit)
    {
        int i = 0;
        if (hit.collider.gameObject.layer == gravityLayer)
        {
            EnableAimDirector(hit);
            i = 1;
            _playerAimEffectController.SetColors(gravityHitColor, alternativeGravityHitColor);
        }
        else
        {
            DisableAimDirector();
            if (hit.collider.gameObject.layer == magnetLayer)
            {
                i = 2;
                _playerAimEffectController.SetColors(magnetHitColor, alternativeMagnetHitColor);
            }
            else
            {
                _playerAimEffectController.SetColors(groundHitColor, alternativeGroundHitColor);
            }
        }
        
        
        
        //lr.material = lineMaterials[i];
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
            Point = hit.point,
            TargetGameObject = hit.transform.gameObject,
            SourceGameObject = gameObject,
            HitNormal = hit.normal
        };
        EventSystem.Current.FireEvent(gravityGunEvent);
    }

    private RaycastHit GetFinalGravityGunHit(Vector3 position, Vector3 currentDirection, List<RaycastHit> hits)
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
            return SetMirrorLine(closestHit, currentDirection);
        }

        // if (_lineRenderer.positionCount > 2)
        // {
        //     _lineRenderer.positionCount--;
        // }

        if (closestHit.collider.gameObject.layer != magnetLayer) return closestHit;
        if (closestHit.transform.parent == null) return closestHit;
        if (closestHit.transform.GetComponentInParent<DynamicObjectMovement>() == null) return closestHit;
        if (!closestHit.transform.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
        {
            closestHit = hits[0];
        }

        return closestHit;
    }

    private RaycastHit GetSingleRay(Vector3 position, List<RaycastHit> hits)
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

        var rayCastHits = InitRaycasts(mirrorHit.collider.transform.position, mirrorDirection);
        RaycastHit toUse = GetFinalGravityGunHit(mirrorHit.point, mirrorDirection, rayCastHits);

        // if (_lineRenderer.positionCount < 3)
        // {
        //     _lineRenderer.positionCount++;
        // }
        //
        // _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, toUse.point);

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
        StartCoroutine(StartShotWithDelay());
        DisableAimDirector();
    }

    private IEnumerator StartShotWithDelay()
    {
        playerShotAudioSource.loop = false;
        playerShotAudioSource.PlayOneShot(playerShoots);
        var hits = InitRaycasts(transform.position, _currentDirection);
        yield return new WaitForSeconds(gunDelay);
        TriggerGravityGunEvent(GetFinalGravityGunHit(transform.position, _currentDirection, hits));
    } 

    public void Aim(InputAction.CallbackContext val)
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (val.performed)
        {
            playerShotAudioSource.loop = true;
            buttonPressed = true;
        }
        else if (!val.performed)
        {
            playerShotAudioSource.loop = false;
            playerShotAudioSource.Stop();
            buttonPressed = false;
        }

        if (val.canceled)
        {
            _playerAimEffectController.gameObject.SetActive(false);
            //_lineRenderer.gameObject.SetActive(false);
            ShootGravityGun();
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        if (playerInput.currentControlScheme == "Mouse")
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 1));
            xy.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(FindObjectOfType<GamepadCursor>().VirtualMouse.position.ReadValue());
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 1));
            xy.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }
    }
}