using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateEmbers : MonoBehaviour
{
    [SerializeField] private string emberRotationTag; //"EmberRotation"
    [SerializeField] private string gravityChangeLayer; //"GravityChange"
    private static Guid _gravityGunEventGuid;
    private static Vector3 _facing;
    private static Quaternion _rotationQuaternion;
    private GameObject[] _embers;
    private static int _gravityLayer;

    private void Start()
    {
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnGravityGunHit, ref _gravityGunEventGuid);
        _facing = GravityController.GetCurrentFacing();
        _embers = GameObject.FindGameObjectsWithTag(emberRotationTag);
        _gravityLayer = LayerMask.NameToLayer(gravityChangeLayer);
    }

    void FixedUpdate()
    {
        if (_embers.Length > 0)
        {
            foreach (var ember in _embers)
            {
                ember.transform.rotation = _rotationQuaternion;
            }
        }
    }

    private void OnGravityGunHit(GravityGunEvent gravityGunEvent)
    {
        if (gravityGunEvent.TargetGameObject.layer == _gravityLayer)
        {
            _facing = GravityController.GetCurrentFacing();
            _rotationQuaternion = FindRotation();
        }
    }

    private Quaternion FindRotation()
    {
        return Quaternion.LookRotation(transform.forward, -_facing);
    }
}