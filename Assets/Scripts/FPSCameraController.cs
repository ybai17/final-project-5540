using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    Transform player;
    float pitch;
    UIManager uiManager;
    public bool debugLogs;
    PlayerInventoryManager inventoryManager;

    public float mouseSens = 500;
    public float minPitch = -90;
    public float maxPitch = 90;

    public float crosshairMaxDistInteractable = 5;

    public static bool IsLookingAtItem { get; private set; }

    void Start()
    {
        player = transform.parent;
        inventoryManager = transform.parent.GetComponent<PlayerInventoryManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiManager = FindAnyObjectByType<UIManager>();

        IsLookingAtItem = false;
    }

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
        RaycastHit hit;
        bool didHit = Physics.Raycast(transform.position, transform.forward, out hit, crosshairMaxDistInteractable);

        if (!didHit)
        {
            uiManager.CrosshairDefault();
            IsLookingAtItem = false;
            uiManager.InteractableTextHide();
            return;
        }

        // Debug
        if (debugLogs)
            Debug.Log("Looking at: " + hit.collider.name + " (tag=" + hit.collider.tag + ")");

        // 1) Pickupable key
        if (hit.collider.CompareTag("KeyPickup"))
        {
            uiManager.CrosshairItem();
            IsLookingAtItem = true;
            uiManager.InteractableTextShow("Press E to pick up");

            if (Input.GetKeyDown(KeyCode.E) && !PlayerInventoryManager.CurrentItemInHand)
            {
                if (debugLogs)
                    Debug.Log("ACQUIRING " + hit.collider.gameObject.name);
                inventoryManager.AcquireItem(hit.collider.gameObject);
            }
        }
        // 2) Locked door that needs key
        else if (hit.collider.CompareTag("LockedDoor"))
        {
            if (PlayerInventoryManager.CurrentItemInHand)
            {
                if (PlayerInventoryManager.CurrentItemInHand.CompareTag("KeyInHand"))
                {
                    uiManager.CrosshairItem();
                    IsLookingAtItem = true;
                    uiManager.InteractableTextShow("Click to use Key");

                    if (Input.GetButton("Fire1"))
                    {
                        hit.collider.GetComponent<LockedDoorBehavior>().OpenLock();
                        inventoryManager.UseItem();
                    }
                }
                else
                {
                    uiManager.CrosshairDefault();
                    IsLookingAtItem = false;
                    uiManager.InteractableTextShow("Hmm, not the right item...");
                }
            }
            else
            {
                uiManager.CrosshairDefault();
                IsLookingAtItem = false;
                uiManager.InteractableTextShow("Need an item for this...");
            }
        }
        // 3) Generic door you can open/close
        else if (hit.collider.CompareTag("door"))
        {
            uiManager.CrosshairItem();
            IsLookingAtItem = true;
            uiManager.InteractableTextShow("Click to open door");

            if (Input.GetButton("Fire1"))
            {
                var door = hit.collider.GetComponentInParent<Door>();
                if (door != null)
                    door.MoveMyDoor(); 
            }
        }
        // 4) Nothing interactable
        else
        {
            uiManager.CrosshairDefault();
            IsLookingAtItem = false;
            uiManager.InteractableTextHide();
        }
    }
}
