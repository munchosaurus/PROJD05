using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    [Header("Volume settings game objects")] 
    [SerializeField] private Toggle globalSoundToggle;
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private Slider dialogueVolumeSlider;
    
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text effectsVolumeText;
    [SerializeField] private TMP_Text dialogueVolumeText;
    
    [SerializeField] private AudioMixer globalMixer;

    public void LoadSoundSettings()
    {
        masterVolumeSlider.value = GameController.MasterVolumeMultiplier;
        musicVolumeSlider.value = GameController.MusicVolumeMultiplier;
        effectsVolumeSlider.value = GameController.EffectsVolumeMultiplier;
        dialogueVolumeSlider.value = GameController.DialogueVolumeMultiplier;
        globalSoundToggle.isOn = GameController.GlobalSoundIsOn;

        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100.0f) + "%";
        musicVolumeText.text = Mathf.Round(musicVolumeSlider.value * 100.0f) + "%";
        effectsVolumeText.text = Mathf.Round(effectsVolumeSlider.value * 100.0f) + "%";
        dialogueVolumeText.text = Mathf.Round(dialogueVolumeSlider.value * 100.0f) + "%";

        OnSoundToggleChanged();
    }
    void Start()
    {
        // Volume setup
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeValueChanged(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeValueChanged(); });
        effectsVolumeSlider.onValueChanged.AddListener(delegate { OnEffectsVolumeValueChanged(); });
        dialogueVolumeSlider.onValueChanged.AddListener(delegate { OnDialogueVolumeValueChanged(); });
        globalSoundToggle.onValueChanged.AddListener(delegate { OnSoundToggleChanged(); });

        masterVolumeSlider.value = GameController.MasterVolumeMultiplier;
        musicVolumeSlider.value = GameController.MusicVolumeMultiplier;
        effectsVolumeSlider.value = GameController.EffectsVolumeMultiplier;
        dialogueVolumeSlider.value = GameController.DialogueVolumeMultiplier;
        globalSoundToggle.isOn = GameController.GlobalSoundIsOn;

        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100.0f) + "%";
        musicVolumeText.text = Mathf.Round(musicVolumeSlider.value * 100.0f) + "%";
        effectsVolumeText.text = Mathf.Round(effectsVolumeSlider.value * 100.0f) + "%";
        dialogueVolumeText.text = Mathf.Round(dialogueVolumeSlider.value * 100.0f) + "%";

        OnSoundToggleChanged();
    }

    public void OnSoundToggleChanged()
    {
        GameController.GlobalSoundIsOn = globalSoundToggle.isOn;

        if (GameController.GlobalSoundIsOn)
        {
            globalMixer.SetFloat("Master", Mathf.Log(GameController.MasterVolumeMultiplier) * 20f);
        }
        else
        {
            globalMixer.SetFloat("Master", Mathf.Log(0.001f) * 20f);
        }
        
        GameLauncher.WriteSettings();
    }
    
    public void OnMasterVolumeValueChanged()
    {
        GameController.MasterVolumeMultiplier = masterVolumeSlider.value;
        globalMixer.SetFloat("Master", Mathf.Log(GameController.MasterVolumeMultiplier) * 20f);
        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100.0f) + "%";
        
        GameLauncher.WriteSettings();;
    }

    public void OnMusicVolumeValueChanged()
    {
        GameController.MusicVolumeMultiplier = musicVolumeSlider.value;
        globalMixer.SetFloat("Music", Mathf.Log(GameController.MusicVolumeMultiplier) * 20f);
        musicVolumeText.text = Mathf.Round(musicVolumeSlider.value * 100.0f) + "%";
        
        GameLauncher.WriteSettings();;
    }

    public void OnEffectsVolumeValueChanged()
    {
        GameController.EffectsVolumeMultiplier = effectsVolumeSlider.value;
        globalMixer.SetFloat("Effects", Mathf.Log(GameController.EffectsVolumeMultiplier) * 20f);
        effectsVolumeText.text = Mathf.Round(effectsVolumeSlider.value * 100.0f) + "%";
        
        GameLauncher.WriteSettings();
    }

    public void OnDialogueVolumeValueChanged()
    {
        GameController.DialogueVolumeMultiplier = dialogueVolumeSlider.value;
        globalMixer.SetFloat("Dialogue", Mathf.Log(GameController.DialogueVolumeMultiplier) * 20f);
        dialogueVolumeText.text = Mathf.Round(dialogueVolumeSlider.value * 100.0f) + "%";
        
        GameLauncher.WriteSettings();
    }
    
}
