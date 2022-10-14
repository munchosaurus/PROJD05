using System;
using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody _rigidbody;

    private void Start()
    {
        // Initializes and stores references to the different mesh attributes to be altered throughout play.
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _rigidbody = gameObject.transform.parent.gameObject.GetComponent<Rigidbody>();
        _currentForm = allForms[0];
        ChangeForm();
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
                _currentForm = allForms[1];
            }
            else
            {
                _currentForm = allForms[0];
            }

            ChangeForm();
            _cooldownCounter = 0;
        }
    }

    private void ChangeForm()
    {
        _meshFilter.sharedMesh = _currentForm.formMesh;
        _meshCollider.sharedMesh = _currentForm.formMesh;
        _meshRenderer.material = _currentForm.formMaterial;
        if (_currentForm.canMove)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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