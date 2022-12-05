using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAimEffectController : MonoBehaviour
{
    [SerializeField] private GameObject[] anchorPositions = new GameObject[4];
    [SerializeField] private VisualEffect _visualEffect;

    public void SetAim(Vector3 origin, Vector3 hitPosition)
    {
        anchorPositions[0].transform.position = origin;
        anchorPositions[3].transform.position = hitPosition;
        anchorPositions[1].transform.localPosition = anchorPositions[3].transform.localPosition * 0.33f;
        anchorPositions[2].transform.localPosition = anchorPositions[3].transform.localPosition * 0.66f;
    }

    public void SetColors(Vector4 c, Vector4 c2)
    {
        _visualEffect.SetVector4("Colour",c);
        _visualEffect.SetVector4("Colour_Alt",c2);
    }
    
    
}
