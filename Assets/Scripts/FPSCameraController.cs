using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    Transform player;
    float pitch;

    public float mouseSens = 100;
    public float minPitch = -90;
    public float maxPitch = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        if (player)
            player.Rotate(Vector3.up, moveX);

        pitch -= moveY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void CrosshairEffect()
    {
        
    }
}
