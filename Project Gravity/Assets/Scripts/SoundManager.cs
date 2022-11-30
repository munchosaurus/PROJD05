using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Hit sounds")] [SerializeField]
    private AudioClip groundHitClip;

    [SerializeField] private AudioClip lavaHitClip;
    [SerializeField] private AudioClip gravityHitClip;
    [SerializeField] private AudioClip mirrorHitClip;

    [Header("Magnet activation sounds")] [SerializeField]
    private AudioClip magnetActivationClip;

    [SerializeField] private AudioClip magnetDeactivationClip;

    [Header("Collision sounds")] [SerializeField]
    private AudioClip playerCollidesWithGroundClip;

    [SerializeField] private AudioClip playerCollidesWithObjectClip;
    [SerializeField] private AudioClip playerCollidesWithMagnetClip;
    [SerializeField] private AudioClip playerCollidesWithGravityPlateClip;
    [SerializeField] private AudioClip playerCollidesWithLavaClip;
    [SerializeField] private AudioClip playerCollidesWithTrampolineSound;
    [SerializeField] private AudioClip objectCollidesWithGroundClip;

    private static int gravityLayer;
    private static int groundLayer;
    private static int magnetLayer;
    private static int lavaLayer;
    private static int playerLayer;
    private static int moveableLayer;
    private static int mirrorLayer;

    [Header("Speaker prefab")] [SerializeField]
    private GameObject speakerPrefab;

    private static Guid _gravityGunEventGuid;
    private static Guid _trampolineEventGuid;
    private static Guid _playerDeathEventGuid;
    private static Guid _collisionEventGuid;

    private void Awake()
    {
        gravityLayer = LayerMask.NameToLayer("GravityChange");
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        lavaLayer = LayerMask.NameToLayer("Hazard");
        moveableLayer = LayerMask.NameToLayer("Moveable");
        mirrorLayer = LayerMask.NameToLayer("Mirror");
    }

    private void Start()
    {
        EventSystem.Current.RegisterListener<GravityGunEvent>(PlayGunHitSound, ref _gravityGunEventGuid);
        EventSystem.Current.RegisterListener<TrampolineEvent>(PlayTrampolineSound, ref _trampolineEventGuid);
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathEventGuid);
        EventSystem.Current.RegisterListener<CollisionEvent>(PlayCollisionSound, ref _collisionEventGuid);
    }

    private void PlayCollisionSound(CollisionEvent collisionEvent)
    {
        if (collisionEvent.SourceGameObject.CompareTag("Player"))
        {
            PlayPlayerCollisionSound(collisionEvent);
        }
        else if (collisionEvent.SourceGameObject.GetComponent<DynamicObjectMovement>())
        {
            PlayObjectCollisionSound(collisionEvent);
        }
    }

    private void PlayObjectCollisionSound(CollisionEvent collisionEvent)
    {
        
        try
        {
            collisionEvent.SourceGameObject.GetComponent<AudioSource>().PlayOneShot(objectCollidesWithGroundClip);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void PlayPlayerCollisionSound(CollisionEvent collisionEvent)
    {
        var sources = collisionEvent.SourceGameObject.GetComponentsInChildren<AudioSource>();
        AudioClip audioClip = null;
        if (collisionEvent.Layers.Contains(lavaLayer))
        {
            return;
        }

        if (collisionEvent.Layers.Contains(moveableLayer))
        {
            audioClip = playerCollidesWithObjectClip;
        }
        else if (collisionEvent.Layers.Contains(magnetLayer))
        {
            audioClip = playerCollidesWithMagnetClip;
        } else if (collisionEvent.Layers.Contains(gravityLayer))
        {
            audioClip = playerCollidesWithGravityPlateClip;
        }
        else if (collisionEvent.Layers.Contains(groundLayer))
        {
            audioClip = playerCollidesWithGroundClip;
        }

        foreach (var source in sources)
        {
            if (source.gameObject.name == ("CollisionSoundPlayer"))
            {
                source.PlayOneShot(audioClip);
            }
        }
    }

    private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        var speaker = Instantiate(speakerPrefab, playerDeathEvent.SourceGameObject.transform.position,
            Quaternion.identity);
        speaker.GetComponent<AudioSource>().PlayOneShot(playerCollidesWithLavaClip);
        StartCoroutine(DestroyAfterTime(speaker, playerCollidesWithLavaClip.length));
    }

    private void PlayTrampolineSound(TrampolineEvent trampolineEvent)
    {
        var speaker = Instantiate(speakerPrefab, trampolineEvent.SourceGameObject.transform.position,
            Quaternion.identity);
        speaker.GetComponent<AudioSource>().PlayOneShot(playerCollidesWithTrampolineSound);
        StartCoroutine(DestroyAfterTime(speaker, playerCollidesWithTrampolineSound.length));
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
        }
        else if (gravityGunEvent.TargetGameObject.layer == lavaLayer)
        {
            clipLength = lavaHitClip.length;
            speaker.GetComponent<AudioSource>().PlayOneShot(lavaHitClip);
            StartCoroutine(DestroyAfterTime(speaker, clipLength));
        } 
        else if (gravityGunEvent.TargetGameObject.layer == mirrorLayer)
        {
            clipLength = mirrorHitClip.length;
            speaker.GetComponent<AudioSource>().PlayOneShot(mirrorHitClip);
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
                clipLength = magnetActivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetActivationClip);
            }
            else
            {
                clipLength = magnetDeactivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetDeactivationClip);
            }

            StartCoroutine(DestroyAfterTime(sp, clipLength));
        }
        else if (gravityGunEvent.TargetGameObject.transform.GetComponentInParent<DynamicObjectMovement>() != null)
        {
            if (gravityGunEvent.TargetGameObject.transform.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
            {
                clipLength = magnetActivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetActivationClip);
            }
            else
            {
                clipLength = magnetDeactivationClip.length;
                sp.GetComponent<AudioSource>().PlayOneShot(magnetDeactivationClip);
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