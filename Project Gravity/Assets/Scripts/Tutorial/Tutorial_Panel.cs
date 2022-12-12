using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Tutorial_Panel : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;
    [SerializeField] AudioClip alternateClip;
    [SerializeField] bool hasAlternateStuff = false;
    [SerializeField] TMP_Text theText;
    [SerializeField, TextArea] string alternateTextString;
    [SerializeField] Image[] theImages;
    [SerializeField] Sprite[] alternateImageSprites;

    private void OnEnable()
    {
        if (hasAlternateStuff && GameController.CurrentControlSchemeIndex == 1)
        {
            theText.text = alternateTextString;
            for (int i = 0; i < theImages.Length; i++)
            {
                theImages[i].sprite = alternateImageSprites[i];
            }

            audioSource.PlayOneShot(alternateClip);
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
