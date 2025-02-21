using UnityEngine;

public class FPSPlayerController : MonoBehaviour
{
    public bool debugLogs;
    public float moveSpeed = 10;
    public float playerGravity = 5f;
    public float jumpHeight = 0.2f;
    public float airControl = 10f;

    Vector3 moveInput;
    Vector3 moveDirectionActual;
    CharacterController controller;

    bool isCrouched;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (debugLogs) {
            //Debug.Log("X-axis input: " + horizontal);
            //Debug.Log("Z-axis input: " + vertical);
        }
        
        moveInput = transform.right * horizontal + transform.forward * vertical;
        moveInput.Normalize();

        if (debugLogs)
            Debug.Log("Player.isGrounded: " + controller.isGrounded);

        if (controller.isGrounded) {
            moveDirectionActual = moveInput;
            
            if (Input.GetButton("Jump")) {
                moveDirectionActual.y = Mathf.Sqrt(2 * jumpHeight * playerGravity);
            } else {
                moveDirectionActual.y = 0;
            }
        } else {
            //mid-air
            moveInput.y = moveDirectionActual.y;
            moveDirectionActual = Vector3.Lerp(moveDirectionActual, moveInput, airControl * Time.deltaTime);
        }

        if (debugLogs)
            Debug.Log("move = " + moveDirectionActual);

        moveDirectionActual.y -= playerGravity * Time.deltaTime;
        controller.Move(moveDirectionActual * moveSpeed * Time.deltaTime);
    }
}
