using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public float pitchMin = -90f;
    public float pitchMax = 90f;
    Transform playerBody;
    float pitch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerBody = transform.parent.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X")* mouseSensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // yaw rotate arouond y axis. applies to the player
        if (playerBody)
        {
            playerBody.Rotate(Vector3.up * moveX);
        }

        // pitch around x axis to look up and down. applies to the camera
        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);//limit player's vision
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        Debug.Log("moveX: " + moveX);
    }
}
