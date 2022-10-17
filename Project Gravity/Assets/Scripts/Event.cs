﻿using UnityEngine;

// Add new Events as soon as needed
public abstract class Event
{
    public GameObject TargetGameObject;
    public GameObject SourceGameObject;
}

public class GravityGunEvent : Event
{
    public Vector3 hitNormal;
}