using UnityEngine;

public class Level3PlayerController : MonoBehaviour
{
    public AudioClip fallingSound;
    bool isFalling = false;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 30 && !isFalling)
        {
            isFalling = true;

            Vector3 fallingLocation = new Vector3(transform.position.x, 0, transform.position.z);

            AudioSource.PlayClipAtPoint(fallingSound, fallingLocation);

            GameObject.FindGameObjectWithTag("UI").GetComponent<Level3CanvasBehavior>().StartFadeOut();

            Invoke("CallLevelLost", 3);
        }
    }

    void CallLevelLost()
    {
        GameObject.Find("Level").GetComponent<LevelManager>().LevelLost();
    }
}
