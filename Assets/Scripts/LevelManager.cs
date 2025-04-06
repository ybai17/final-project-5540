using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel;

    public TMP_Text healthText;

    public AudioClip victorySound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelLost()
    {
        Invoke("ReloadSameScene", 5);
    }

    public void LevelWon()
    {
        healthText.text = "You Won!";
        healthText.color = Color.green;
        healthText.enabled = true;

        AudioSource.PlayClipAtPoint(victorySound, GameObject.FindGameObjectWithTag("Player").transform.position);

        //Invoke("ReloadSameScene", 5);

        SceneManager.LoadScene(1);
    }

    void ReloadSameScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
