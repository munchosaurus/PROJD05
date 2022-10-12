using UnityEngine;

[CreateAssetMenu(fileName = "New form", menuName = "ScriptableObjects/Forms")]
public class Form : ScriptableObject
{
    public string formName;
    public Mesh formMesh;
    public Material formMaterial;
}