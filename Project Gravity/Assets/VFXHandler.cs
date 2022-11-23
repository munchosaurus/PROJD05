using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    private static Guid _playerDeathGuid;
    private static readonly int Dead = Animator.StringToHash("dead");

    void Start()
    {
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathGuid);
    }


    public void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        playerDeathEvent.SourceGameObject.GetComponent<Animator>().SetBool("dead", true);
        playerDeathEvent.SourceGameObject.transform.Find("VFX_Fire").gameObject.SetActive(true);
    }

    // IEnumerator PlayDeathVFX(PlayerDeathEvent playerDeathEvent)
    // {
    //     
    //     
    //     yield return new WaitForSeconds(playerDeathEvent.DeathTime);
    //     playerDeathEvent.SourceGameObject.transform.Find("VFX_Fire").gameObject.SetActive(false);
    // }
}
