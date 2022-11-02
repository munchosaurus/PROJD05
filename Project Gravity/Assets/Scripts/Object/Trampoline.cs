using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform startingPoint;
    private Vector3 _playerCheckDimensions;
    private PlayerInput _playerInput;
    [SerializeField] private float trampolinePower;
    [SerializeField] private float trampolineCooldown;
    [SerializeField] private float counter;
    [SerializeField] private Transform board;
    [SerializeField] private float trampolineMovementSpeed;
    private Vector3 initialSpot;
    private bool isMoving;
    


    private void Start()
    {
        initialSpot = board.transform.position;
        Debug.Log(initialSpot);
        _playerCheckDimensions = new Vector3(0.05f, 0.01f, 0.5f);
        _playerInput = FindObjectOfType<PlayerInput>();
        counter = trampolineCooldown;
    }

    void FixedUpdate()
    {
        if (counter <= trampolineCooldown)
        {
            counter += 1 * Time.fixedDeltaTime;
        }
        else
        {
            DetectPlayer();
        }
        
        // if (isMoving)
        // {
        //     switch (transform.localEulerAngles.z)
        //     {
        //         case 0:
        //             board.position += new Vector3(0, trampolineMovementSpeed * Time.fixedDeltaTime, 0);
        //             break;
        //         case 90: 
        //             board.position += new Vector3(-trampolineMovementSpeed * Time.fixedDeltaTime, 0, 0);
        //             break;
        //         case 180:
        //             board.position += new Vector3(0, -trampolineMovementSpeed * Time.fixedDeltaTime, 0);
        //             break;
        //         case 270:
        //             board.position += new Vector3(trampolineMovementSpeed * Time.fixedDeltaTime, 0, 0);
        //             break;
        //     }
        // }
        // else if (board.position != initialSpot)
        // {
        //     board.position = Vector3.MoveTowards(transform.position, initialSpot, 2 * Time.fixedDeltaTime);
        //     
            // switch (transform.localEulerAngles.z)
            // {
            //     case 0:
            //         board.position -= new Vector3(0, trampolineMovementSpeed * Time.fixedDeltaTime, 0);
            //         break;
            //     case 90: 
            //         board.position -= new Vector3(-trampolineMovementSpeed * Time.fixedDeltaTime, 0 , 0);
            //         break;
            //     case 180:
            //         board.position -= new Vector3(0, -trampolineMovementSpeed * Time.fixedDeltaTime, 0);
            //         break;
            //     case 270:
            //         board.position -= new Vector3(trampolineMovementSpeed * Time.fixedDeltaTime, 0, 0);
            //         break;
            // }
    //     }
    //
    }
    
    
    public void DetectPlayer()
    {
        RaycastHit hit;

        if (Physics.BoxCast(startingPoint.position, _playerCheckDimensions, transform.up, out hit, transform.rotation, 
                transform.localScale.y/5, playerLayer))
        {
            ExtDebug.DrawBoxCastBox(startingPoint.position, _playerCheckDimensions, transform.rotation, transform.up, transform.localScale.y/5, Color.red);
            if (hit.normal.x != 0)
            {
                if (hit.normal.x > 0)
                {
                    _playerInput.velocity.x -= trampolinePower;
                    StartCoroutine(ShootBoard());
                }
                else
                {
                    _playerInput.velocity.x += trampolinePower;
                    StartCoroutine(ShootBoard());
                }
            } 
            if (hit.normal.y != 0)
            {
                if (hit.normal.y > 0)
                {
                    _playerInput.velocity.y -= trampolinePower;
                    StartCoroutine(ShootBoard());
                }
                else if (hit.normal.y < 0)
                {
                    GetComponent<Animator>().Play("Trampoline");
                    _playerInput.velocity.y += trampolinePower;
                    //StartCoroutine(ShootBoard());
                }
            }
            
            counter = 0;
            
        }
    }

    private IEnumerator ShootBoard()
    {
        isMoving = true;
        yield return new WaitForSeconds(0.2f);
        isMoving = false;
    }
    
}
