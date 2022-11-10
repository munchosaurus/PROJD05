using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLogic : MonoBehaviour
{
    [SerializeField] private IngameMenu menu;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] private LayerMask hazardMask;
    [SerializeField] private float collisionVelocityThreshold;
    private PlayerInput _playerInput;
    private static Guid _playerDeathEventGuid;
    void Start()
    {
        menu = FindObjectOfType<IngameMenu>();
        _playerInput = gameObject.GetComponent<PlayerInput>();
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(MuteMovementSound, ref _playerDeathEventGuid);
    }

    private void FixedUpdate()
    {
        CheckForHazards();
    }

    
    private void CheckForHazards()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, Quaternion.identity,
                transform.localScale.y / 2, hazardMask, QueryTriggerInteraction.Collide))
        {
            if (menu != null && (_playerInput.velocity.y < -collisionVelocityThreshold || Physics.gravity.y < 0))
            {
                Event playerDeathEvent = new PlayerDeathEvent()
                {
                    TargetGameObject = hit.transform.gameObject,
                    SourceGameObject = gameObject,
                    DeathTime = Constants.PLAYER_DEATH_TIME
                };
                EventSystem.Current.FireEvent(playerDeathEvent);
                return;
            }
        }

        if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, Quaternion.identity,
                transform.localScale.y / 2, hazardMask, QueryTriggerInteraction.Collide))
        {
            if (menu != null && (_playerInput.velocity.y > collisionVelocityThreshold || Physics.gravity.y > 0))
            {
                Event playerDeathEvent = new PlayerDeathEvent()
                {
                    TargetGameObject = hit.transform.gameObject,
                    SourceGameObject = gameObject,
                    DeathTime = Constants.PLAYER_DEATH_TIME
                };
                EventSystem.Current.FireEvent(playerDeathEvent);
                return;
            }
        }

        if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, Quaternion.identity,
                transform.localScale.x / 2, hazardMask, QueryTriggerInteraction.Collide))
        {
            if (menu != null  && (_playerInput.velocity.x > collisionVelocityThreshold || Physics.gravity.x > 0))
            {
                Event playerDeathEvent = new PlayerDeathEvent()
                {
                    TargetGameObject = hit.transform.gameObject,
                    SourceGameObject = gameObject,
                    DeathTime = Constants.PLAYER_DEATH_TIME
                };
                EventSystem.Current.FireEvent(playerDeathEvent);
                return;
            }
        }

        if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, Quaternion.identity,
                transform.localScale.x / 2, hazardMask, QueryTriggerInteraction.Collide))
        {
            if (menu != null && (_playerInput.velocity.x < -collisionVelocityThreshold || Physics.gravity.x < 0))
            {
                Event playerDeathEvent = new PlayerDeathEvent()
                {
                    TargetGameObject = hit.transform.gameObject,
                    SourceGameObject = gameObject,
                    DeathTime = Constants.PLAYER_DEATH_TIME
                };
                EventSystem.Current.FireEvent(playerDeathEvent);
            }
        }
    }

    private void MuteMovementSound(PlayerDeathEvent playerDeathEvent)
    {
        GetComponent<AudioSource>().mute = true;
    }
}
