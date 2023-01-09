using System;
using UnityEngine;

public class WinningConfettiHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    private static Guid _playerSucceedsGuid;

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
    }

    public void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        particleSystem.Play();
    }
}