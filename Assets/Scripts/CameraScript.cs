using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned to the CameraFollow script.");
            return;
        }

        Vector3 newPosition = transform.position;
        newPosition.x = player.position.x;
        newPosition.y = player.position.y;

        //newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        transform.position = newPosition;
    }
}

