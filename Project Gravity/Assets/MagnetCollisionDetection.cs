using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Vector3 = UnityEngine.Vector3;

public class MagnetCollisionDetection : MonoBehaviour
{
    private static int magnetLayer;
    [SerializeField] public bool upTriggered, downTriggered, leftTriggered, rightTriggered;
    private Dictionary<Vector3, bool> sides;

    private void Awake()
    {
        magnetLayer = LayerMask.NameToLayer("GravityMagnet");
        sides = new Dictionary<Vector3, bool>();
        SetupSides();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == magnetLayer)
        {
            Debug.Log(GetClosestSide(other.transform.position));
        }
    }

    private Vector3 GetClosestSide(Vector3 otherPos)
    {
        Vector3 pos = new Vector3();

        foreach (var pair in sides)
        {
            if (pair.Value)
            {
                if (pos == Vector3.zero)
                {
                    pos = pair.Key;
                }
                else if(Vector3.Distance(pair.Key, otherPos) < Vector3.Distance(pos, otherPos))
                {
                    pos = pair.Key;
                }
            }

        }

        return pos;

    }

    private void SetupSides()
    {
        sides.Add(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z), upTriggered);
        sides.Add(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z), downTriggered);
        sides.Add(new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z), leftTriggered);
        sides.Add(new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z), rightTriggered);
    }
}