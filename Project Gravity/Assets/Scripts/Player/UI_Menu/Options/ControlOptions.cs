using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private TMP_Text movementControlSchemeText;
    [SerializeField] private TMP_Text gravityGunControlSchemeText;
    [SerializeField] private Image movementControlImage;
    [SerializeField] private Image gravityGunControlImage;
    [TextArea] public string[] movementTexts;
    [TextArea] public string[] gravityGunTexts;
    [SerializeField] private Sprite[] movementImages;
    [SerializeField] private Sprite[] gravityGunImages;
    [SerializeField] private TMP_Dropdown controlChoiceDropdown;

    void Start()
    {
        // Controls
        controlChoiceDropdown.onValueChanged.AddListener(delegate { OnControlSchemeChanged(); });
        controlChoiceDropdown.value = GameController.CurrentControlSchemeIndex;
        SetControlImagesAndTexts();
    }

    public void OnControlSchemeChanged()
    {
        GameController.CurrentControlSchemeIndex = controlChoiceDropdown.value;
        SetControlImagesAndTexts();
        
        GameLauncher.WriteSettings();
    }
    
    public void SetControlImagesAndTexts()
    {
        movementControlSchemeText.text = movementTexts[GameController.CurrentControlSchemeIndex];
        gravityGunControlSchemeText.text = gravityGunTexts[GameController.CurrentControlSchemeIndex];
        movementControlImage.sprite = movementImages[GameController.CurrentControlSchemeIndex];
        gravityGunControlImage.sprite = gravityGunImages[GameController.CurrentControlSchemeIndex];
    }
}
