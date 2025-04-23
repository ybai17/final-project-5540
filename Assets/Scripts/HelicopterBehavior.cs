using UnityEngine;

public class HelicopterBehavior : MonoBehaviour
{
    public Transform rotor;
    public float rotorSpeed = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotor.Rotate(Vector3.up * rotorSpeed);
    }
}
