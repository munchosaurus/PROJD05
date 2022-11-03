using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform startingPoint;
    [SerializeField] private float trampolinePower;
    [SerializeField] private float trampolineCooldown;
    [SerializeField] private float counter;
    [SerializeField] private Transform board;
    private Vector3 _playerCheckDimensions;
    private PlayerInput _playerInput;

    private void Start()
    {
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
    }
    
    
    public void DetectPlayer()
    {
        RaycastHit hit;

        if (Physics.BoxCast(startingPoint.position, _playerCheckDimensions, transform.up, out hit, transform.rotation, 
                transform.localScale.y/5, playerLayer))
        {
            ExtDebug.DrawBoxCastBox(startingPoint.position, _playerCheckDimensions, transform.rotation, transform.up, transform.localScale.y/5, Color.red);

            _playerInput.velocity = transform.up * trampolinePower;
            StartCoroutine(ShootBoard());
            counter = 0;
            
        }
    }

    private IEnumerator ShootBoard()
    {
        GetComponent<Animator>().SetBool("isMoving", true);
        yield return new WaitForSeconds(GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
        GetComponent<Animator>().SetBool("isMoving", false);
    }
    
}
