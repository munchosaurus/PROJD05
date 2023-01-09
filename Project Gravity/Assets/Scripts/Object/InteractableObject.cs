using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Interactable interactable;
    
    void FixedUpdate()
    {
        if (interactable.shouldRotate)
        {
            transform.Rotate(new Vector3(-interactable.rotationSpeed, 0f, 0));
        }
    }

    /*
     * Useless shell, no keycards used in levels.
     */
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
