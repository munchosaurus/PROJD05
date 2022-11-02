using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionLogic : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactableGameObjects;
    private PlayerInput _playerInput;
    private const float DISTANCE_TO_INTERACT_THRESHOLD = 0.5f;
    private IngameMenu _menu;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
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
        return _playerInput.IsGrounded() && GetComponent<KeycardLogic>().keyCardsCompleted;
    }

    private bool IsInteractableCloseEnough(Transform interactable)
    {
        return Vector3.Distance(gameObject.transform.position, interactable.position) <
            DISTANCE_TO_INTERACT_THRESHOLD && _playerInput.IsGrounded();
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
        if (IsInteractableCloseEnough(GetClosestInteractable()))
        {
            if (GetClosestInteractable().GetComponent<InteractableObject>().interactable.interactableType == Interactable.InteractableType.Target)
            {
                if(!IsGoalReached())
                    return;
            }
            
            if (!_menu.interactText.activeSelf)
            {
                _menu.interactText.SetActive(true);
            }
        }
        else
        {
            if (_menu.interactText.activeSelf)
            {
                _menu.interactText.SetActive(false);
            }
        }
    }
    
    public void Interact()
    {
        if (IsInteractableCloseEnough(GetClosestInteractable()))
        {
            if (GetClosestInteractable().GetComponent<InteractableObject>().interactable.interactableType == Interactable.InteractableType.Target)
            {
                if (!IsGoalReached())
                {
                    return;
                }
            }
            GetClosestInteractable().GetComponent<InteractableObject>().Interact();
        }
    }
    
    
}