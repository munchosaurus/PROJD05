using UnityEngine;


[CreateAssetMenu(fileName = "New interactable", menuName = "ScriptableObjects/Interactables")]
public class Interactable : ScriptableObject
{
    //public string name;
    public bool shouldRotate;
    public float rotationSpeed;
    public InteractableType interactableType;
    public enum InteractableType
    {
        Target,
        Lever,
        Keycard,
    };


}