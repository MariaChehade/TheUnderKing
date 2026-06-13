using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        Camera.main.orthographicSize = 2.5f;
    }
    
    void LateUpdate()
    {
        transform.position = new Vector3(
            player.position.x,
            player.position.y,
            -10f
        );
    }
}