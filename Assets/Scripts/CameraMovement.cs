using UnityEngine;

public class CameraMovememnt : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraLoc = transform.position;
        cameraLoc.x = player.transform.position.x;
        cameraLoc.y = player.transform.position.y;
        cameraLoc.z = player.transform.position.z;

        transform.position = cameraLoc;
    }
}
