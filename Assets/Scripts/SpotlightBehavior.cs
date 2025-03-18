using UnityEngine;

public class SpotlightBehavior : MonoBehaviour
{
    public Color defaultColor;
    public Color detectionColor;

    Light light;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light>();
        light.color = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) {
            light.color = Color.black;
        }
    }
    */

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            light.color = detectionColor;
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            light.color = defaultColor;
        }
    }
}
