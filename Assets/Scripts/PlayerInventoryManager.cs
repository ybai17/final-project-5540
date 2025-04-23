using UnityEngine;
using System.Collections.Generic;


/// Manages a multi-slot hand inventory: picking up items into the first free slot,
/// and using (removing) items from their slot without ever stacking or duplicating.
public class PlayerInventoryManager : MonoBehaviour
{
    [Header("Inventory Slots (assign in Inspector)")]
    [Tooltip("Each slot is a empty Transform under which one item can be parented.")]
    public Transform[] handItemSlots;

    
    /// The list of item instances currently in the player's hand.
    /// Always mirrors exactly the children under handItemSlots.

    public static List<GameObject> CurrentItemsInHand { get; private set; }
        = new List<GameObject>();

    void Start()
    {
        // Deactivate all slot parents initially (they become visible once holding an item)
        foreach (var slot in handItemSlots)
        {
            if (slot != null)
                slot.gameObject.SetActive(false);
        }
    }

   
    /// Pick up the given world-space pickup. Its prefab is instantiated into
    /// the first empty slot (i.e. slot.childCount == 0).  Cleans up any null
    /// entries before proceeding so the list stays in sync.
   
    public void AcquireItem(GameObject pickup)
    {
        if (pickup == null)
        {
            Debug.LogError("[Inventory] AcquireItem: pickup argument is null.");
            return;
        }

        // Remove any destroyed items from our list
        CurrentItemsInHand.RemoveAll(item => item == null);

        // Find the first empty slot
        for (int i = 0; i < handItemSlots.Length; i++)
        {
            var slot = handItemSlots[i];
            if (slot == null) continue;

            if (slot.childCount == 0)
            {
                // Get the prefab from the pickup's KeyPickupBehavior
                var behavior = pickup.GetComponent<KeyPickupBehavior>();
                if (behavior == null || behavior.prefab == null)
                {
                    Debug.LogError($"[Inventory] Slot {i}: pickup has no prefab!");
                    return;
                }

                // Instantiate and parent under slot
                GameObject clone = Instantiate(behavior.prefab);
                clone.transform.SetParent(slot);
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localRotation = Quaternion.identity;

                // Remove any Rigidbody so it stays exactly at the slot
                var rb = clone.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                // Notify the pickup that it was collected
                behavior.PickedUp();

                // Show the slot (slot's GameObject was hidden by default)
                slot.gameObject.SetActive(true);

                // Track it
                CurrentItemsInHand.Add(clone);
                return;
            }
        }

        Debug.Log("[Inventory] All slots occupied—cannot pick up new item.");
    }

    /// <summary>
    /// Uses (consumes) the specified item. It must be one of the
    /// GameObjects previously instantiated into a slot.
    /// </summary>
    public void UseItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("[Inventory] UseItem: item argument is null.");
            return;
        }

        // Clean up any destroyed entries
        CurrentItemsInHand.RemoveAll(x => x == null);

        // Find the slot containing this item
        for (int i = 0; i < handItemSlots.Length; i++)
        {
            var slot = handItemSlots[i];
            if (slot == null || slot.childCount == 0) continue;

            var child = slot.GetChild(0).gameObject;
            if (child == item)
            {
                // Destroy the instance
                Destroy(child);

                // Hide the slot again
                slot.gameObject.SetActive(false);

                // Remove from our tracking list
                CurrentItemsInHand.Remove(item);
                return;
            }
        }

        Debug.LogWarning("[Inventory] Tried to use an item not present in any slot.");
    }
}

