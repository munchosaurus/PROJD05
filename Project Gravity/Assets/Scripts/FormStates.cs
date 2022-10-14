using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FormStates : MonoBehaviour
{
    [Header("Add default form to index 0 of array")] [SerializeField]
    private Form[] allForms;

    [Header("Add seconds (as float) for form switch cooldown")] [SerializeField]
    private float coolDown;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;
    private Form _currentForm;
    private int _cooldownCounter;
    private GameObject _parentGameObject;
    private PlayerController2D _playerController2D;
    private Rigidbody _rigidbody;

    private void Start()
    {
        // Initializes and stores references to the different mesh attributes to be altered throughout play.
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _parentGameObject = gameObject.transform.parent.gameObject;
        _playerController2D = _parentGameObject.GetComponent<PlayerController2D>();
        _rigidbody = _parentGameObject.GetComponent<Rigidbody>();
        _cooldownCounter = (int) coolDown * 60;
        ChangeForm(0);
    }

    void FixedUpdate()
    {
        if (_cooldownCounter <= coolDown * 60)
        {
            _cooldownCounter += 1;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_currentForm.formName.Equals(allForms[0].formName))
            {
                ChangeForm(1);
            }
            else
            {
                ChangeForm(0);
            }
            _cooldownCounter = 0;
        }
    }

    private void ChangeForm(int formIndex)
    {
        _currentForm = allForms[formIndex];
        _meshFilter.sharedMesh = _currentForm.formMesh;
        _meshCollider.sharedMesh = _currentForm.formMesh;
        _meshRenderer.material = _currentForm.formMaterial;
        if (_currentForm.canMove)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _playerController2D.RotateToPlane();
        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }

    public Form GetCurrentForm()
    {
        return _currentForm;
    }
}