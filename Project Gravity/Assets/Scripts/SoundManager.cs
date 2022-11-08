using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Hit sounds")] [SerializeField]
    private AudioClip groundHitClip;

    [SerializeField] private AudioClip lavaHitClip;
    [SerializeField] private AudioClip gravityHitClip;

    [Header("Magnet activation sounds")] [SerializeField]
    private AudioClip magnetActivationClip;

    [SerializeField] private AudioClip magnetDeactivationClip;

    [Header("Collision sounds")] [SerializeField]
    private AudioClip playerOnGroundSound;

    [SerializeField] private AudioClip playerOnMagnetSound;
    [SerializeField] private AudioClip playerOnLavaSound;
    [SerializeField] private AudioClip playerOnTrampolineSound;


    private static int gravityLayer;
    private static int groundLayer;
    private static int magnetLayer;
    private static int lavaLayer;

    [Header("Speaker prefab")] [SerializeField]
    private GameObject speakerPrefab;

    private static Guid _gravityGunEventGuid;
    private static Guid _trampolineEventGuid;

    private void Awake()
    {
        gravityLayer = LayerMask.NameToLayer("GravityChange");
        groundLayer = LayerMask.NameToLayer("Ground");
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        lavaLayer = LayerMask.NameToLayer("Hazard");
    }

    private void Start()
    {
        EventSystem.Current.RegisterListener<GravityGunEvent>(PlayGunHitSound, ref _gravityGunEventGuid);
        EventSystem.Current.RegisterListener<TrampolineEvent>(PlayTrampolineSound, ref _trampolineEventGuid);
    }

    private void PlayTrampolineSound(TrampolineEvent trampolineEvent)
    {
        var speaker = Instantiate(speakerPrefab, trampolineEvent.SourceGameObject.transform.position,
            Quaternion.identity);
        speaker.GetComponent<AudioSource>().PlayOneShot(playerOnTrampolineSound);
        StartCoroutine(DestroyAfterTime(speaker, playerOnTrampolineSound.length));
    }

    private void PlayGunHitSound(GravityGunEvent gravityGunEvent)
    {
        var speaker = Instantiate(speakerPrefab, gravityGunEvent.TargetGameObject.transform.position,
            Quaternion.identity);
        float clipLength;
        if (gravityGunEvent.TargetGameObject.layer == gravityLayer)
        {
            clipLength = gravityHitClip.length;
            speaker.GetComponent<AudioSource>().PlayOneShot(gravityHitClip);
            StartCoroutine(DestroyAfterTime(speaker, clipLength));
        }
        else if (gravityGunEvent.TargetGameObject.layer == magnetLayer)
        {
            PlayMagnetToggle(gravityGunEvent, speaker);
        } else if (gravityGunEvent.TargetGameObject.layer == lavaLayer)
        {
            clipLength = lavaHitClip.length;
            speaker.GetComponent<AudioSource>().PlayOneShot(lavaHitClip);
            StartCoroutine(DestroyAfterTime(speaker, clipLength));
        }
        else
        {
            clipLength = groundHitClip.length;
            speaker.GetComponent<AudioSource>().PlayOneShot(groundHitClip);
            StartCoroutine(DestroyAfterTime(speaker, clipLength));
        }
    }

    private void PlayMagnetToggle(GravityGunEvent gravityGunEvent, GameObject sp)
    {
        float clipLength;
        if (gravityGunEvent.TargetGameObject.transform.GetComponent<GravityMagnet>() != null)
        {
            if (gravityGunEvent.TargetGameObject.transform.GetComponent<GravityMagnet>().triggered)
            {
                clipLength = magnetDeactivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetDeactivationClip);
            }
            else
            {
                clipLength = magnetActivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetActivationClip);
            }

            StartCoroutine(DestroyAfterTime(sp, clipLength));
        }
        else if (gravityGunEvent.TargetGameObject.transform.GetComponentInParent<DynamicObjectMovement>() != null)
        {
            if (gravityGunEvent.TargetGameObject.transform.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
            {
                clipLength = magnetDeactivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetDeactivationClip);
            }
            else
            {
                clipLength = magnetActivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetActivationClip);
            }

            StartCoroutine(DestroyAfterTime(sp, clipLength));
        }
    }

    private IEnumerator DestroyAfterTime(GameObject go, float length)
    {
        yield return new WaitForSeconds(length);
        Destroy(go);
    }
}