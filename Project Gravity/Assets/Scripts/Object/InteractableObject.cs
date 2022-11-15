using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    public Interactable interactable;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (interactable.shouldRotate)
        {
            transform.Rotate(new Vector3(-interactable.rotationSpeed, 0f, 0));
        }
    }

    public void Interact()
    {
        switch (interactable.interactableType)
        {
            case Interactable.InteractableType.Target:
                break;
            case Interactable.InteractableType.Keycard:
                FindObjectOfType<KeycardLogic>().AddCollectedKeyCard(gameObject.GetInstanceID());
                break;
            case Interactable.InteractableType.Lever:
                //TODO ENTER HERE WHAT TO DO
                break;
        }
    }

}
