using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    Transform player;
    float pitch;
    UIManager uiManager;
    public bool debugLogs;
    PlayerInventoryManager inventoryManager;

    public float mouseSens = 100;
    public float minPitch = -90;
    public float maxPitch = 90;

    public float crosshairMaxDistInteractable = 5;

    public static bool IsLookingAtItem {get; set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.transform;

        inventoryManager = transform.parent.GetComponent<PlayerInventoryManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiManager = FindAnyObjectByType<UIManager>();

        IsLookingAtItem = false;
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

    void FixedUpdate()
    {
        CrosshairCheck();
    }

    void CrosshairCheck()
    {
        RaycastHit raycast;

        if (Physics.Raycast(transform.position, transform.forward, out raycast, crosshairMaxDistInteractable)) {

            if (debugLogs)
                Debug.Log("Looking at: " + raycast.collider.name);

            if (raycast.collider.CompareTag("KeyPickup")) {
                uiManager.CrosshairItem();
                IsLookingAtItem = true;
                uiManager.InteractableTextShow("Press E to pick up");

                if (Input.GetKeyDown(KeyCode.E) && !PlayerInventoryManager.CurrentItemInHand) {
                    if (debugLogs)
                        Debug.Log("ACQUIRING " + raycast.collider.gameObject);
                    inventoryManager.AcquireItem(raycast.collider.gameObject);
                }
            } else if (raycast.collider.CompareTag("LockedDoor")) {
                if (PlayerInventoryManager.CurrentItemInHand) {
                    if (PlayerInventoryManager.CurrentItemInHand.CompareTag("KeyInHand")) {
                        uiManager.CrosshairItem();
                        uiManager.InteractableTextShow("Click to use Key");

                        if (Input.GetButton("Fire1")) {
                            raycast.collider.gameObject.GetComponent<LockedDoorBehavior>().OpenLock();
                            inventoryManager.UseItem();
                        }
                    } else {
                        uiManager.InteractableTextShow("Hmm, not the right item...");
                    }
                } else {
                    uiManager.InteractableTextShow("Need an item for this...");
                }
            } else {
                uiManager.CrosshairDefault();
                IsLookingAtItem = false;
                uiManager.InteractableTextHide();
            }
        } else {
            uiManager.CrosshairDefault();
            IsLookingAtItem = false;
            uiManager.InteractableTextHide();
        }
    }
}
