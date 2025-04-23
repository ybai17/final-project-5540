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
        if (SceneManager.GetActiveScene().name == "Level1Office")
        {
            currentLevel = 1;
        }
        if (SceneManager.GetActiveScene().name == "Level2Yilei")
        {
            currentLevel = 2;
        }
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            currentLevel = 3;
        }
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

        if (currentLevel == 3)
        {
            currentLevel = 0;
            PlayerPrefs.SetInt("CurrentLevel", 0);
            SceneManager.LoadScene(currentLevel);
        }
        else {
            currentLevel += 1;

            SceneManager.LoadScene(currentLevel);
        }
    }

    void ReloadSameScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
