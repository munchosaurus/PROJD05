﻿using UnityEngine;

// Add new Events as soon as needed
public abstract class Event
{
    public GameObject TargetGameObject;
    public GameObject SourceGameObject;
}

public class GravityGunEvent : Event
{
    public Vector3 HitNormal;
}

public class TrampolineEvent : Event
{
    
}

public class ObjectFoundMagnetEvent : Event
{
    
}

public class PlayerDeathEvent : Event
{
    public float DeathTime;
}

