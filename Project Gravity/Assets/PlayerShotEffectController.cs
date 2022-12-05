using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerShotEffectController : MonoBehaviour
{
    [SerializeField] private GameObject laserShotPrefab;
    private static Guid _playerShootsGuid;

    void Start()
    {
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnPlayerShoots, ref _playerShootsGuid);
    }

    public void OnPlayerShoots(GravityGunEvent gravityGunEvent)
    {
        var laser = Instantiate(laserShotPrefab, gravityGunEvent.SourceGameObject.transform.position,
            Quaternion.Euler(0, 0, 0));

        laser.transform.LookAt(gravityGunEvent.Point);
        laser.GetComponent<VisualEffect>().SetFloat("LightningLength",
            Vector3.Distance(gravityGunEvent.SourceGameObject.transform.position, gravityGunEvent.Point));
        laser.GetComponent<VisualEffect>().Play();

        StartCoroutine(DestroyAfterTime(laser, 0.5f));
    }
    
    private IEnumerator DestroyAfterTime(GameObject go, float length)
    {
        yield return new WaitForSeconds(length);
        Destroy(go);
    }
}