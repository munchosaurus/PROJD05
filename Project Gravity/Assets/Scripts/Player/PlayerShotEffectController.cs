using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerShotEffectController : MonoBehaviour
{
    [SerializeField] private GameObject laserShotPrefab;
    [ColorUsage(true, true)]
    [SerializeField] private Color[] colors;
    [SerializeField] private float impactSize;
    [SerializeField] private List<Animator> gravityAnimators;
    private static Guid _playerShootsGuid;
    private static int gravityLayer;
    private static int groundLayer;
    private static int magnetLayer;

    void Start()
    {        
        gravityLayer = LayerMask.NameToLayer("GravityChange");
        groundLayer = LayerMask.NameToLayer("Ground");
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        foreach (var gravityGameObject in GameObject.FindGameObjectsWithTag("GravityEffectObject"))
        {
            gravityAnimators.Add(gravityGameObject.GetComponent<Animator>());
        }
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnPlayerShoots, ref _playerShootsGuid);
    }
    
    

    public void OnPlayerShoots(GravityGunEvent gravityGunEvent)
    {
        var laser = Instantiate(laserShotPrefab, gravityGunEvent.SourceGameObject.transform.position,
            Quaternion.Euler(0, 0, 0));

        laser.transform.LookAt(gravityGunEvent.Point);
        var effect = laser.GetComponent<VisualEffect>();
        effect.SetFloat("LightningLength",
            Vector3.Distance(gravityGunEvent.SourceGameObject.transform.position, gravityGunEvent.Point));
        
        if (gravityGunEvent.TargetGameObject.layer == gravityLayer)
        {
            if (gravityGunEvent.GravityWasChanged)
            {
                foreach (var gravityAnimator in gravityAnimators)
                {
                    gravityAnimator.Play("Emissive Flash",0, 0f);
                }
            }
            effect.SetVector4("FlashColour", colors[1]);
            effect.SetFloat("ImpactSize", impactSize * 1.5f);
        }
        else if (gravityGunEvent.TargetGameObject.layer == magnetLayer)
        {
            effect.SetVector4("FlashColour", colors[2]);
            effect.SetFloat("ImpactSize", impactSize * 1.5f);
        }
        else
        {
            effect.SetFloat("ImpactSize", impactSize);
        }
        
        laser.GetComponent<VisualEffect>().Play();
        

        StartCoroutine(DestroyAfterTime(laser, 0.5f));
    }
    
    private IEnumerator DestroyAfterTime(GameObject go, float length)
    {
        yield return new WaitForSeconds(length);
        Destroy(go);
    }
}