using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerSalary : MonoBehaviour
{
    // public static float CurrentSalary {get; set;}

    // public float maxSalary = 100.0f;
    public int startingSalary = 100;
    public TMP_Text salaryText;
    public Slider salarySlider;
    public bool IsAlive { get; private set; } = true;
    private int currentSalary;
    public int damageValue = 5; 
 

    //public AudioClip deathSFX;

    void Start()
    {
        currentSalary = startingSalary;
        IsAlive = true;
        UpdateSalarySlider();
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;  // Do not apply damage if already dead

        currentSalary -= damage;
        currentSalary = Mathf.Clamp(currentSalary, 0, startingSalary);
        UpdateSalarySlider();
        Debug.Log("Damage taken, current salary: " + currentSalary);
        salaryText.text =  currentSalary + "%";

        if (currentSalary <= 0 && IsAlive)
        {
            Die();
        }
    }

    public void TakeSalary(int salary)
    {
        if (!IsAlive) return;  

        currentSalary += salary;
        currentSalary = Mathf.Clamp(currentSalary, 0, startingSalary);
        UpdateSalarySlider();
        Debug.Log("Health recovered, current health: " + currentSalary);
    }

    void Die()
    {
        Debug.Log("Player dies...");
        IsAlive = false;

        // Play death sound if available.
        var audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.Play();
        }


        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        GetComponent<FPSPlayerController>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;

       GetComponent<AudioSource>().Play();

        GameObject.Find("Level").GetComponent<LevelManager>().LevelLost();
        
    }

    void UpdateSalarySlider()
    {
        if (salarySlider)
        {
            salarySlider.value = currentSalary;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HR")) 
        {
            TakeDamage(damageValue);
        }
    }



}
