using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float cameraPanSpeed = 5f;
    public Vector3 positionA = new Vector3(10, 1, 30);
    public Vector3 positionB = new Vector3(-10, 1, 30);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float pingPong = Mathf.PingPong(Time.time * cameraPanSpeed, 1);
        transform.position = Vector3.Lerp(positionA, positionB, pingPong);
    }
}
