using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class FacilitatorHandler : MonoBehaviour
{
    [SerializeField] private LevelContainer currentLevelContainer;
    [SerializeField] private AudioClip[] levelTakesLongClips;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private LevelTimer _levelTimer;
    [SerializeField] private AudioSource mainTheme;
    [SerializeField] private float musicSoundDamper;
    private bool longClipPlayed;
    private bool strictClipPlayed;
    private float musicStartVolume;
    private float clipLength;
    private Random rnd;
    private static int latestLongTimeClip;

    private void Start()
    {
        currentLevelContainer = FindObjectOfType<LevelSelector>()
            .levelContainers[SceneManager.GetActiveScene().buildIndex - 1];

        try
        {
            _levelTimer = FindObjectOfType<LevelTimer>();
        }
        catch (Exception e)
        {
            Debug.Log("Level timer not found" + e);
        }
        
        try
        {
            mainTheme = GameObject.Find("MainThemeSpeaker").GetComponent<AudioSource>();
            musicStartVolume = mainTheme.volume;
        }
        catch (Exception e)
        {
            Debug.Log("Main theme speaker not found" + e);
        }
        rnd = new Random();
    }

    void FixedUpdate()
    {
        if (_levelTimer == null)
            return;

        if (_levelTimer.GetTimePassed() > currentLevelContainer.strictClipTime && currentLevelContainer.strictClip != null && !longClipPlayed)
        {
            if (mainTheme != null)
            {
                mainTheme.volume *= musicSoundDamper;
                StartCoroutine(ReturnVolumeToMaximum(currentLevelContainer.strictClip.length));
            }
            
            _audioSource.PlayOneShot(currentLevelContainer.strictClip);
            longClipPlayed = true;
        }
        
        
        if (_levelTimer.GetTimePassed() > currentLevelContainer.playLongClipTime && !strictClipPlayed)
        {
            int i;
            do
            {
                i = rnd.Next(levelTakesLongClips.Length);
            } while (i == latestLongTimeClip);

            latestLongTimeClip = i;
            
            if (mainTheme != null)
            {
                mainTheme.volume *= musicSoundDamper;
                StartCoroutine(ReturnVolumeToMaximum(levelTakesLongClips[latestLongTimeClip].length));
            }
            
            _audioSource.PlayOneShot(levelTakesLongClips[latestLongTimeClip]);
            strictClipPlayed = true;
        }
    }

    private IEnumerator ReturnVolumeToMaximum(float length)
    {
        yield return new WaitForSeconds(length+0.1f);
        mainTheme.volume = musicStartVolume;
    }
}
