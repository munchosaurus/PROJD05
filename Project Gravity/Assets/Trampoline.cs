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
    private Vector3 initialSpot;
    private bool isMoving;
    


    private void Start()
    {
        initialSpot = board.transform.position;
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

        if (isMoving)
        {
            board.position += new Vector3(0, 10f * Time.fixedDeltaTime, 0);
        }
        else if (board.position != initialSpot)
        {
            board.position -= new Vector3(0, 10f * Time.fixedDeltaTime, 0);
        }
    }
    
    public void DetectPlayer()
    {
        RaycastHit hit;

        if (Physics.BoxCast(startingPoint.position, _playerCheckDimensions, transform.up, out hit, transform.rotation, 
                transform.localScale.y/5, playerLayer))
        {
            ExtDebug.DrawBoxCastBox(startingPoint.position, _playerCheckDimensions, transform.rotation, transform.up, transform.localScale.y/5, Color.red);
            Debug.Log(hit.normal);
            if (hit.normal.x != 0)
            {
                if (hit.normal.x > 0)
                {
                    _playerInput.velocity.y -= trampolinePower;
                }
                else
                {
                    _playerInput.velocity.y += trampolinePower;
                }
            } else if (hit.normal.y != 0)
            {
                if (hit.normal.y > 0)
                {
                    _playerInput.velocity.y -= trampolinePower;
                }
                else
                {
                    _playerInput.velocity.y += trampolinePower;
                    StartCoroutine(ShootBoard());
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
