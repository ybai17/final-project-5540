using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static float CurrentHealth {get; set;}

    public float maxHealth = 100.0f;

    public TMP_Text healthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = maxHealth;
        healthText.text = "Salary: $" + maxHealth + ",000 / yr";
        healthText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageAmt)
    {
        CurrentHealth -= damageAmt;

        healthText.text = "Salary: $" + CurrentHealth + ",000 / yr";

        if (CurrentHealth <= 0) {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

            GetComponent<FPSPlayerController>().enabled = false;

            healthText.enabled = true;
            healthText.text = "GAME OVER";
            GetComponent<AudioSource>().Play();

            Invoke("ReloadSameScene", 5);
        }
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HR")) {
            Debug.Log("Triggered with " + other.gameObject.name + " for " + other.gameObject.GetComponent<HRBehavior>().damage + " damage");
            TakeDamage(other.gameObject.GetComponent<HRBehavior>().damage);
            other.gameObject.GetComponent<HRBehavior>().CapturePlayer();
        }
    }
    */

    void ReloadSameScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
