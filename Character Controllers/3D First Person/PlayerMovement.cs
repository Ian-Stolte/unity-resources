using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5;
    public float yawSpeed = 2;
    public float pitchSpeed = 1;
    public float minPitch = -80;
    public float maxPitch = 80;
    private float pitch;

    [Header("Jump")]
    public float jumpForce = 6;
    public LayerMask groundLayer;
    private float jumpDelay;

    [Header("References")]
    public Transform cam;
    private Rigidbody rb;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Read look input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Update pitch & yaw
        pitch -= mouseY * pitchSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        float yawDelta = mouseX * yawSpeed;
        transform.Rotate(Vector3.up, yawDelta);

        //Read movement input
        int forward = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            forward++;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            forward--;
        int right = 0;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            right++;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            right--;

        //Apply movement
        Vector3 moveDir = (forward * transform.forward + right * transform.right).normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);

        //Jump
        jumpDelay -= Time.deltaTime;
        bool grounded = Physics.OverlapSphere(transform.position - new Vector3(0, 1, 0), 0.2f, groundLayer).Length > 0;
        if (grounded && Input.GetKeyDown(KeyCode.Space) && jumpDelay <= 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpDelay = 0.5f;
        }
    }
}
