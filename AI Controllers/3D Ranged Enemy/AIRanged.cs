using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [Header("Movement")]
    public Transform gun;
    public float moveSpeed = 3;
    private int forward;
    private int right;
    
    [Header("Direction Logic")]
    public float maxChange = 2;
    public float minChange = 0.5f;
    private float changeDirTimer;

    [Header("Shooting")]
    public float maxShoot = 1.5f;
    public float minShoot = 0.3f;
    public float maxShootWait = 1f;
    public float minShootWait = 0.1f;
    private float shootTimer;
    private bool shooting;

    [Header("References")]
    public Transform player;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetDirection();
    }

    void Update()
    {
        if (!GameManager.Instance.gameOver)
        {
            //Rotate character on Y axis (yaw)
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -4f, 0);
            //TODO: add a bit of randomness so the AI can miss

            //Rotate gun on X axis (pitch)
            Vector3 tiltDir = player.position - gun.position;
            Quaternion rot = Quaternion.LookRotation(tiltDir);
            gun.localRotation = Quaternion.Euler(rot.eulerAngles.x, 0f, 0f);

            //Move
            Vector3 moveDir = (forward * transform.forward + right * transform.right).normalized;
            if (Vector3.Distance(player.position, transform.position) < 4) //move faster if player too close
                moveDir *= 1.3f;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);

            //Check if need to change direction
            changeDirTimer -= Time.deltaTime;
            if (changeDirTimer <= 0f)
                SetDirection();

            //Track shooting
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shooting = !shooting;
                if (shooting)
                    shootTimer = Random.Range(minShootWait, maxShootWait);
                else
                    shootTimer = Random.Range(minShoot, maxShoot);
            }
        }
    }

    //Randomly choose a new movement direction
    void SetDirection()
    {
        changeDirTimer = Random.Range(minChange, maxChange);

        //Set forward/back based on distance to player
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < 4)
            forward = -1;                                     //100% chance to move away
        else if (dist < 9)
            forward = (Random.Range(0f, 1f) < 0.5f) ? -1 : 0; //50% chance to move away
        else if (dist < 14)
            forward = (Random.Range(0f, 1f) < 0.4f) ? 1 : 0;  //40% chance to move forward
        else if (dist < 20)
            forward = (Random.Range(0f, 1f) < 0.6f) ? 1 : 0;  //60% chance to move forward
        else if (dist > 20)
            forward = 1;                                      //100% chance to move forward

        //Set left/right
        float rand = Random.Range(0f, 1f);
        if (rand < 0.4f)        //40% chance to not move
        {
            if (dist < 6)
                right = (Random.Range(0f, 1f) < 0.5f) ? -1 : 1; //force strafe if too close
            else
                right = 0;
        }
        else if (rand < 0.7f)   //30% chance to move left
            right = -1;
        else                    //30% chance to move right
            right = 1;

        //Jump
        float jumpChance = (dist < 5f) ? 0.5f : 0.2f; //higher jump chance if player too close
        if (Random.Range(0f, 1f) < jumpChance)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, GetComponent<CharacterMovement>().jumpForce, rb.linearVelocity.z);
    }
}
