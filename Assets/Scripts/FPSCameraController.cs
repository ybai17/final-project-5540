using UnityEngine;
using System.Linq;

public class FPSCameraController : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSens = 500f;
    float mouseSensMultiplier;
    public float minPitch = -90f;
    public float maxPitch = 90f;

    [Header("Interaction Settings")]
    public float crosshairMaxDistInteractable = 5f;
    public bool debugLogs = false;

    private Transform player;
    private float pitch;
    private UIManager uiManager;
    private PlayerInventoryManager inventoryManager;

    public static bool IsLookingAtItem { get; private set; }

    void Start()
    {
        mouseSensMultiplier = PlayerPrefs.GetFloat("MouseSensitivity");

        player = transform.parent;
        inventoryManager = player.GetComponent<PlayerInventoryManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiManager = FindAnyObjectByType<UIManager>();
        IsLookingAtItem = false;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * mouseSens * mouseSensMultiplier * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSens * mouseSensMultiplier * Time.deltaTime;

        player.Rotate(Vector3.up, moveX);
        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void FixedUpdate() => CrosshairCheck();

    private void CrosshairCheck()
    {
        // Raycast forward from camera
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, crosshairMaxDistInteractable))
        {
            ClearUI();
            return;
        }

        if (debugLogs)
            Debug.Log($"[CrosshairCheck] Hit: {hit.collider.name} (tag={hit.collider.tag})");

        // Remove destroyed items from inventory
        PlayerInventoryManager.CurrentItemsInHand.RemoveAll(item => item == null);

        // Key pickup
        if (hit.collider.CompareTag("KeyPickup"))
        {
            uiManager.CrosshairItem();
            IsLookingAtItem = true;
            uiManager.InteractableTextShow("Press E to pick up");
            if (Input.GetKeyDown(KeyCode.E))
                inventoryManager.AcquireItem(hit.collider.gameObject);
            return;
        }

        // Door interaction (requires optional key)
        var door = hit.collider.GetComponentInParent<DoorScript.Door>();
        if (door != null)
        {
            bool canOpen = door.requiredKeyPrefab == null
                || PlayerInventoryManager.CurrentItemsInHand.Any(item =>
                {
                    var pickup = item.GetComponent<KeyPickupBehavior>();
                    return pickup != null && pickup.prefab == door.requiredKeyPrefab;
                });

            if (canOpen)
            {
                uiManager.CrosshairItem();
                IsLookingAtItem = true;
                uiManager.InteractableTextShow("Press E to open door");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.ToggleDoor();
                    // consume the key if needed
                    if (door.requiredKeyPrefab != null)
                    {
                        var keyItem = PlayerInventoryManager.CurrentItemsInHand
                            .First(item =>
                            {
                                var pickup = item.GetComponent<KeyPickupBehavior>();
                                return pickup != null && pickup.prefab == door.requiredKeyPrefab;
                            });
                        inventoryManager.UseItem(keyItem);
                    }
                }
            }
            else
            {
                uiManager.CrosshairDefault();
                IsLookingAtItem = false;
                uiManager.InteractableTextShow($"Need {door.requiredKeyPrefab.name}");
            }
            return;
        }

        // Nothing interactable
        ClearUI();
    }

    private void ClearUI()
    {
        uiManager.CrosshairDefault();
        IsLookingAtItem = false;
        uiManager.InteractableTextHide();
    }
}