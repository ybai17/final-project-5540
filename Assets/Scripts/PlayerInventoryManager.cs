using UnityEngine;
using System.Collections.Generic;

public class PlayerInventoryManager : MonoBehaviour
{
    [Header("Inventory Slots (assign in Inspector)")]
    public Transform[] handItemSlots;    // Multiple slot pivots

    // Track items in hand per slot
    public static List<GameObject> CurrentItemsInHand { get; private set; } = new List<GameObject>();

    void Start()
    {
        // Deactivate all slot parents initially
        foreach (var slot in handItemSlots)
        {
            if (slot != null)
                slot.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Attempts to place pickup into the first available slot.
    /// </summary>
    public void AcquireItem(GameObject pickup)
    {
        if (pickup == null)
        {
            Debug.LogError("[Inventory] Pickup is null!");
            return;
        }

        // Find first empty slot
        for (int i = 0; i < handItemSlots.Length; i++)
        {
            var slot = handItemSlots[i];
            if (slot == null) continue;

            // Consider empty if no children
            if (slot.childCount == 0)
            {
                // Instantiate the item prefab at slot
                var prefab = pickup.GetComponent<KeyPickupBehavior>()?.prefab;
                if (prefab == null)
                {
                    Debug.LogError("[Inventory] No prefab found on pickup!");
                    return;
                }

                GameObject clone = Instantiate(prefab,
                    slot.position,
                    slot.rotation);

                // Mark original as picked up
                pickup.GetComponent<KeyPickupBehavior>()?.PickedUp();

                // Remove physics if any
                var rb = clone.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                // Parent under slot and activate
                clone.transform.SetParent(slot);
                slot.gameObject.SetActive(true);

                // Track in inventory list
                CurrentItemsInHand.Add(clone);

                return;
            }
        }

        Debug.Log("[Inventory] No available slot to pick up item.");
    }

    /// <summary>
    /// Uses (removes) the specified item from inventory.
    /// </summary>
    public void UseItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("[Inventory] No item specified to use.");
            return;
        }

        // Find item in slots
        for (int i = 0; i < handItemSlots.Length; i++)
        {
            var slot = handItemSlots[i];
            if (slot == null) continue;

            if (slot.childCount > 0)
            {
                var child = slot.GetChild(0).gameObject;
                if (child == item)
                {
                    // Destroy and clear
                    Destroy(child);
                    slot.gameObject.SetActive(false);
                    CurrentItemsInHand.Remove(item);
                    return;
                }
            }
        }

        Debug.LogWarning("[Inventory] Tried to use item not in any slot.");
    }
}
