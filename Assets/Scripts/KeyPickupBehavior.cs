using UnityEngine;

public class KeyPickupBehavior : MonoBehaviour, KeyObjectInterface
{
    public GameObject prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickedUp()
    {
        Destroy(gameObject);
    }
}
