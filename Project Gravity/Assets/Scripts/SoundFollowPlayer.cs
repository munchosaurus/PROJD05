using UnityEngine;

public class SoundFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = playerTransform.position;
    }
}