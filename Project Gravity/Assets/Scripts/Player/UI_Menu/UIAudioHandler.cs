using Mono.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip sliderSound;
    private void Start()
    {
        Collection<Button> buttons = new Collection<Button>();
        foreach (Button go in Resources.FindObjectsOfTypeAll(typeof(Button)) as Button[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            // if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
            //     continue;
            buttons.Add(go);
        }
        foreach (var var in buttons)
        {
            var.onClick.AddListener(delegate { OnButtonClick(); });
        }
        
        Collection<Slider> sliders = new Collection<Slider>();
        foreach (Slider go in Resources.FindObjectsOfTypeAll(typeof(Slider)) as Slider[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            // if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
            //     continue;
            sliders.Add(go);
        }
        foreach (var var in sliders)
        {
            var.onValueChanged.AddListener(delegate { OnSliderChange(); });
        }
    }

    public void OnButtonClick()
    {
        _audioSource.PlayOneShot(buttonPress);
    }

    public void OnSliderChange()
    {
        _audioSource.PlayOneShot(sliderSound);
    }
}
