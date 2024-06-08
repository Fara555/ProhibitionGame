using System.Collections;
using UnityEngine;
using EZCameraShake;

public class PlayerCamera : MonoBehaviour
{
    [Header("Head Bob")]
    [SerializeField] private float walkAmplitude = 0.02f;
    [SerializeField] private float sprintAmplitude = 0.04f;
    [SerializeField, Range(0, 30f)] private float walkFrequency = 10f;
    [SerializeField, Range(0, 30f)] private float sprintFrequency = 18f;
    [SerializeField] private bool enableBobing = true;

    [Header("Camera/Flashlight")]
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float cameraAcceleration;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform orientation;

    private float _toggleSpeed = 3f;
    private Vector3 _startPos;
    private Rigidbody playerRB;
     private PlayerFootStepSystem playerFootSteps;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        playerRB = GetComponentInParent<Rigidbody>();
        playerFootSteps = playerRB.gameObject.GetComponentInChildren<PlayerFootStepSystem>();
        _startPos = orientation.localPosition;
    }

    private void Start()
    {
        // Lock cursos
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    private void Update()
    {
        //Get horizontal and verticla input from mouse
        xRotation += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * cameraSensitivity;
        yRotation += Input.GetAxisRaw("Mouse X") * Time.deltaTime * cameraSensitivity;

        // Clamp rotation so player cant rotate camera infinitly
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        hand.localRotation = Quaternion.Euler(-xRotation, yRotation, 0); //Set hand rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, yRotation, 0), cameraAcceleration * Time.deltaTime); // Set head rotation - represent of horizontal rotation
        orientation.localRotation = Quaternion.Lerp(orientation.localRotation, Quaternion.Euler(-xRotation, 0, 0), cameraAcceleration * Time.deltaTime); //Set orientaion rotation - represent of vertical rotation

        if (!enableBobing) return; // return if bobbing is disabled;
        if (playerFootSteps.isSprinting) CheckMotion(SprintFootStepMotion()); // Animate camera when player is sprinting
        else CheckMotion(WalkFootStepMotion()); // Animate camera when player is walking
        ResetPosition(); // Set position of orientation back
    }

    public void LookAtEnemy(Transform enemy) // Start this method when enemy attacks player
    {
        StartCoroutine(LookAtEnemyRoutine(enemy));
    }

    private IEnumerator LookAtEnemyRoutine(Transform enemy) 
    {
        // Save original rotation angles
        Quaternion startHeadRotation = transform.localRotation;
        Quaternion startOrientationRotation = orientation.localRotation;
        Quaternion startHandRotation = hand.localRotation;

        // Get final rotation angles depending on current `Head` and `Orientation` position
        Vector3 directionToEnemy = enemy.position - transform.position;
        Quaternion endHeadRotation = Quaternion.Euler(0f, Quaternion.LookRotation(directionToEnemy).eulerAngles.y, 0f);
        Quaternion endOrientationRotation = Quaternion.Euler(Quaternion.LookRotation(directionToEnemy).eulerAngles.x, 0f, 0f);
        Quaternion endRotation = Quaternion.LookRotation(directionToEnemy);


        float time = 0.0f;
        while (time < 1f)
        {
            // Slowly rotate Head and Orientation to the right angles
            transform.localRotation = Quaternion.Slerp(startHeadRotation, endHeadRotation, time);
            orientation.localRotation = Quaternion.Slerp(startOrientationRotation, endOrientationRotation, time);
            hand.localRotation = Quaternion.Slerp(startHandRotation, endRotation, time);


            time += Time.deltaTime * 2.5f; // Turning speed
            yield return null;
        }

        //  Fix final position
        transform.localRotation = endHeadRotation;
        orientation.localRotation = endOrientationRotation;
        hand.localRotation = endRotation;

        orientation.LookAt(enemy.transform);

        // Update global coordinates
        yRotation = transform.localEulerAngles.y;
        xRotation = transform.localEulerAngles.x;
    }

    #region HeadBobbing

    private void CheckMotion(Vector3 motion) //Set motion 
    {
        float speed = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z).magnitude;

        if (speed < _toggleSpeed) return;
        if (!playerFootSteps.isGrounded) return;

        orientation.localPosition += motion;
    }

    private Vector3 WalkFootStepMotion() // Walk motion represent of walking camera animation
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * walkFrequency) * walkAmplitude / 2;
        pos.x += Mathf.Cos(Time.time * walkFrequency / 2) * walkAmplitude * 0.5f;
        return pos;
    }

    private Vector3 SprintFootStepMotion()// Sprint motion represent of sprinting camera animation
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * sprintFrequency) * sprintAmplitude / 2;
        pos.x += Mathf.Cos(Time.time * sprintFrequency / 2) * sprintAmplitude * 0.5f;
        return pos;
    }

    private void ResetPosition() // Reset orientation position after bobbing animation
    {
        if (orientation.localPosition == _startPos) return;
        orientation.localPosition = Vector3.Lerp(orientation.localPosition, _startPos, 1 * Time.deltaTime);
    }

    #endregion
}
