using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Slider healthSlider;
    public int startingHealth = 100;
    public bool IsAlive { get; private set; } = true;
    private int currentHealth;
    public int damageValue = 5; 

    //public AudioClip deathSFX;

    void Start()
    {
        currentHealth = startingHealth;
        IsAlive = true;
        UpdateHealthSlider();
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;  // Do not apply damage if already dead

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
        UpdateHealthSlider();
        Debug.Log("Damage taken, current health: " + currentHealth);

        if (currentHealth <= 0 && IsAlive)
        {
            Die();
        }
    }

    public void TakeHealth(int health)
    {
        if (!IsAlive) return;  

        currentHealth += health;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
        UpdateHealthSlider();
        Debug.Log("Health recovered, current health: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Enemy dies...");
        IsAlive = false;

        // Play death sound if available.
        var audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.Play();
        }

      
        transform.Rotate(-90, 0, 0, Space.Self);

       
        Destroy(gameObject, 1.5f);
    }

    void UpdateHealthSlider()
    {
        if (healthSlider)
        {
            healthSlider.value = currentHealth;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) 
        {
            TakeDamage(damageValue);
        }
    }
}
