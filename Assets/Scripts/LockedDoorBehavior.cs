using UnityEngine;

public class LockedDoorBehavior : MonoBehaviour, LockedObjectInterface
{
    public static bool IsLocked {get; set;}

    public GameObject openDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLock()
    {
        Instantiate(openDoor, transform.position, Quaternion.Euler(0, 90, 0));
        Destroy(gameObject);
    }
}
