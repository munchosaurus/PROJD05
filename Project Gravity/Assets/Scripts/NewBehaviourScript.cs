using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Vector2 newScale = new Vector2(1, 1);

    Renderer rend;

    // Something like this: https://github.com/Dsphar/Cube_Texture_Auto_Repeat_Unity/blob/master/ReCalcCubeTexture.cs

    private void Awake()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.material.mainTextureScale *= newScale;
    }

    private void Update()
    {
        rend.material.mainTextureOffset += new Vector2(0, 0.1f * Time.deltaTime);

        if (rend.material.mainTextureOffset.y >= 1.66f)
        {
            rend.material.mainTextureOffset = new Vector2(0.33f, 0.66f);
        }
    }
}
