using UnityEngine;

public class GravityMagnet : MonoBehaviour
{
    [SerializeField] private bool triggered;

    private void TriggerMagnet(bool shouldBeOn)
    {
        triggered = shouldBeOn;
    }

    public bool IsTriggered()
    {
        return triggered;
    }
    
    
}
