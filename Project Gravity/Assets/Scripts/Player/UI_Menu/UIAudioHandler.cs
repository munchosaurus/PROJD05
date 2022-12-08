using System.Collections;
using System.Collections.Generic;
using Mono.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip buttonPress;
    private void Start()
    {
        Collection<Button> buttons = new Collection<Button>();
        foreach (Button go in Resources.FindObjectsOfTypeAll(typeof(Button)) as Button[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                continue;
            buttons.Add(go);
        }
        foreach (var var in buttons)
        {
            var.onClick.AddListener(delegate { OnButtonClick(); });
        }
    }

    public void OnButtonClick()
    {
        _audioSource.PlayOneShot(buttonPress);
    }
}
