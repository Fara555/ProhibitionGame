using Hearing;
using UnityEngine;
using static Hearing.Sound;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private Transform head;
    [SerializeField] private float groundDrag;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Footsteps")]
    private PlayerFootStepSystem footStepSystem;

    private float timeSinceLastJump;
 
    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    private void Start()
    {
        timeSinceLastJump = jumpCooldown;
        footStepSystem = GetComponentInChildren<PlayerFootStepSystem>();    
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        
        bool previouslyGrounded = footStepSystem.isGrounded; 

        footStepSystem.CheckIsGrounded();

        //if (!previouslyGrounded && footStepSystem.isGrounded)
        //{
        //    footStepSystem.CheckLandSound();
        //}


        if (footStepSystem.isGrounded) rb.drag = groundDrag;
        else rb.drag = 0;

        MovePlayer();
        GetInput();
        SpeedControl();

        timeSinceLastJump += Time.deltaTime;
    }

    private void MovePlayer()
    {
        moveDirection = head.forward * verticalInput + head.right * horizontalInput;

        if (footStepSystem.isGrounded && footStepSystem.isSprinting) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * sprintMultiplier, ForceMode.Force);
        else if (footStepSystem.isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!footStepSystem.isGrounded && footStepSystem.isSprinting) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier * sprintMultiplier, ForceMode.Force);
        else if (!footStepSystem.isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        //footStepSystem.CheckJumpSound();
        timeSinceLastJump = 0f; 
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && timeSinceLastJump >= jumpCooldown && footStepSystem.isGrounded) Jump();
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

}
