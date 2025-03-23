using UnityEngine;

public class SpotlightBehavior : MonoBehaviour
{
    public Color defaultColor;
    public Color detectionColor;

    //basically determines how sensitive and how quickly the spotlight changes from defaultColor to detectionColor
    public float spotlightSensitivity;

    public float defaultIntensity;
    public float detectionIntensity;

    Light light;

    bool playerInSpotlight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light>();
        light.color = defaultColor;
        light.intensity = defaultIntensity;

        playerInSpotlight = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* Unlock for chasing state-based HR chasing AI
        if (!playerInSpotlight)
        {
            //Debug.Log("_______Default spotlight");
            light.color = Color.Lerp(light.color, defaultColor, spotlightSensitivity * Time.deltaTime);
            light.intensity = Mathf.Lerp(light.intensity, defaultIntensity, spotlightSensitivity * Time.deltaTime);
        }
        */
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player entered spotlight");
            playerInSpotlight = true;

            light.color = detectionColor;
            light.intensity = detectionIntensity;

            //transform.parent.GetComponent<HRBehavior>().CapturePlayer();
        }
    }
    
    /* Unlock for chasing state-based HR chasing AI
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            //Debug.Log("Player STAYING in spotlight");
            light.color = Color.Lerp(light.color, detectionColor, spotlightSensitivity * Time.fixedDeltaTime);
            light.intensity = Mathf.Lerp(light.intensity, detectionIntensity, spotlightSensitivity * Time.fixedDeltaTime);
        }
    }
    */

    
    void OnTriggerExit(Collider other)
    {
        /* Unlock for chasing state-based HR chasing AI
        if (other.gameObject.CompareTag("Player")) {
            //Debug.Log("Player left spotlight");
            playerInSpotlight = false;
        }
        */
        light.color = defaultColor;
        light.intensity = defaultIntensity;
    }
}
