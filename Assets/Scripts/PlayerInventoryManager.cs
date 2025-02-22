using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    GameObject handItemParent;
    public static GameObject CurrentItemInHand {get; set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handItemParent = transform.GetChild(0).GetChild(0).gameObject;

        Debug.Log("handItemParent: " + handItemParent);

        handItemParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcquireItem(GameObject pickup)
    {
        GameObject handItemClone = Instantiate(pickup.GetComponent<KeyPickupBehavior>().prefab,
                                    handItemParent.transform.position,
                                    transform.rotation);

        pickup.GetComponent<KeyPickupBehavior>().PickedUp();

        Destroy(handItemClone.GetComponent<Rigidbody>());

        handItemClone.transform.SetParent(handItemParent.transform);

        handItemParent.SetActive(true);

        CurrentItemInHand = handItemClone;
    }

    public void UseItem()
    {
        
        Destroy(handItemParent.transform.GetChild(0).gameObject);
        CurrentItemInHand = null;
    }

}
