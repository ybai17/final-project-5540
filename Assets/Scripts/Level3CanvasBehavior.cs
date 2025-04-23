using UnityEngine;
using UnityEngine.UI;

public class Level3CanvasBehavior : MonoBehaviour
{

    public Image fadeOut;
    public float fadeOutSpeed = 10f;

    bool fading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fadeOut.color = new Color(0, 0, 0, 0);
        fading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            fadeOut.color = Color.Lerp(fadeOut.color, Color.black, fadeOutSpeed * Time.deltaTime);
        }
    }

    public void StartFadeOut()
    {
        fading = true;
    }
}
