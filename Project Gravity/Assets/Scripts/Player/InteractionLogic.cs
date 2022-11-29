using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionLogic : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactableGameObjects;
    private PlayerController _playerController;
    private const float DISTANCE_TO_INTERACT_THRESHOLD = 0.5f;
    private IngameMenu _menu;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        StartCoroutine(FetchInteractablesInScene());
        _menu = gameObject.GetComponentInChildren<IngameMenu>();
    }

    private void FixedUpdate()
    {
        if (interactableGameObjects.Count > 0)
        {
            ToggleInteraction();
        }
    }

    private IEnumerator FetchInteractablesInScene()
    {
        yield return new WaitForSeconds(0.5f);
        InteractableObject[] ints = FindObjectsOfType<InteractableObject>();
        for (int i = 0; i < ints.Length; i++)
        {
            interactableGameObjects.Add(ints[i].GameObject());
        }
    }

    private bool IsGoalReached()
    {
        return _playerController.IsGrounded() && GetComponent<KeycardLogic>().keyCardsCompleted;
    }

    private bool IsInteractableCloseEnough(Transform interactable)
    {
        return Vector3.Distance(gameObject.transform.position, interactable.position) <
            DISTANCE_TO_INTERACT_THRESHOLD && _playerController.IsGrounded();
    }

    Transform GetClosestInteractable()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (var t in interactableGameObjects)
        {
            if (!t.activeSelf)
            {
                continue;
            }

            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }

        return tMin;
    }

    private void ToggleInteraction()
    {
        if (IsInteractableCloseEnough(GetClosestInteractable()) && !_menu.interactText.activeSelf)
        {
            switch (GetClosestInteractable().GetComponent<InteractableObject>().interactable.interactableType)
            {
                case Interactable.InteractableType.Target:
                    Interact();
                    break;
                case Interactable.InteractableType.Keycard:
                    _menu.interactText.SetActive(true);
                    break;
                case Interactable.InteractableType.Lever:
                    //whatever
                    break;
            }
        }
    }

    public void Interact()
    {
        if (IsInteractableCloseEnough(GetClosestInteractable()))
        {
            switch (GetClosestInteractable().GetComponent<InteractableObject>().interactable.interactableType)
            {
                case Interactable.InteractableType.Target:
                    if (IsGoalReached())
                    {
                        WinningEvent winningEvent = new WinningEvent()
                        {
                        };
                        EventSystem.Current.FireEvent(winningEvent);
                    }

                    break;
                case Interactable.InteractableType.Keycard:
                    _menu.interactText.SetActive(true);
                    break;
                case Interactable.InteractableType.Lever:
                    //whatever
                    break;
            }
        }
    }
}