using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    int currentLevel;
    public TMP_Text continueButtonText;
    public Button continueButton;

    [Header("Mouse Sensitivities (multipliers)")]
    public float mouseSensLow = 0.1f;
    public float mouseSensMed = 1f;
    public float mouseSensHigh = 4f;

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensMed);

        Debug.Log("fetched CurrentLevel: " + currentLevel);

        if (currentLevel == 0)
        {
            continueButton.gameObject.SetActive(false);
        }
        else 
        {
            continueButton.gameObject.SetActive(true);
            continueButtonText.text = "Continue (Level " + currentLevel + ")";
        }
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }

    public void SetSensLow()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensLow);
    }

    public void SetSensMed()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensMed);
    }

    public void SetSensHigh()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensHigh);
    }
}
