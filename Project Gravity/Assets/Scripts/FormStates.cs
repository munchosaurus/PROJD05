using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormStates : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;
    private Form _currentForm;
    [Header("Add default form to index 0 of array")]
    [SerializeField] private Form[] allForms;
    [Header("Add seconds (as float) for form switch cooldown")]
    [SerializeField] private float coolDown;
    private int _cooldownCounter;

    private void Start()
    {
        // Initializes and stores references to the different mesh attributes to be altered throughout play.
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _currentForm = allForms[0];
        _meshFilter.sharedMesh = _currentForm.formMesh;
        _meshCollider.sharedMesh = _currentForm.formMesh;
        _meshRenderer.material = _currentForm.formMaterial;
    }

    void FixedUpdate()
    {
        if (_cooldownCounter < coolDown*60)
        {
            _cooldownCounter += 1;
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ChangeForm();
            _cooldownCounter = 0;
        }
    }
    
    private void ChangeForm()
    {
        for (int i = 0; i < allForms.Length; i++)
        {
            if (_currentForm.formName.Equals(allForms[i].formName))
            {
                continue;
            }
            _currentForm = allForms[i];
            break;
        }
        _meshFilter.sharedMesh = _currentForm.formMesh;
        _meshCollider.sharedMesh = _currentForm.formMesh;
        _meshRenderer.material = _currentForm.formMaterial;
    }
}