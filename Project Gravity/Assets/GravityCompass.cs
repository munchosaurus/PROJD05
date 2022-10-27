using UnityEngine;

public class GravityCompass : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateToPlane(), Time.deltaTime * Constants.GRAVITY_ARROW_ROTATION_SPEED);
    }
    
    public Quaternion RotateToPlane()
    {
        return Quaternion.LookRotation(transform.forward, GravityController.GetCurrentFacing());
    }
    
}
