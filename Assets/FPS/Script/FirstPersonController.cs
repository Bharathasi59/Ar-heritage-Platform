using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Joystick joystick; // Assign in Inspector (only for Classic Mobile)

    [Header("Player Settings")]
    [SerializeField] private float cameraSensitivity = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveInputDeadZone = 10f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.8f;
    private Vector3 velocity; // Tracks vertical velocity

    [Header("Footstep Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    // [SerializeField] private float stepInterval = 0.5f; // time between steps

    [Header("Interval Sound Settings")]
    [SerializeField] private AudioSource intervalAudioSource; // assign in Inspector
    [SerializeField] private AudioClip intervalClip; // assign your sound
    [SerializeField] private float intervalTime = 60f; // 60 seconds

    private float intervalTimer;


    private float stepTimer = 0f;

    private Vector3 lastPosition;
    [SerializeField] private float stepDistance = 2f; // Distance per footstep
    private float distanceMoved = 0f;




    // Touch detection (Classic Mobile mode)
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    // Camera control
    private Vector2 lookInput;
    private float cameraPitch;

    // Movement
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    // Control selection
    private ControlManager controlManager;

    private Quaternion baseRotation; // Add this field to store the base rotation


    void Start()
    {
        controlManager = ControlManager.Instance;
        intervalTimer = intervalTime; // start counting down

        // id = -1 means finger not tracked
        leftFingerId = -1;
        rightFingerId = -1;
        halfScreenWidth = Screen.width / 2;

        // Dead zone calculation
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);

        // Enable gyro if needed
        if (controlManager.useGyroscope && SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            baseRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -0.1f; // Small negative to stick to ground
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Footstep sound
        Vector3 horizontalMovement = transform.position - lastPosition;
        horizontalMovement.y = 0; // ignore vertical movement
        distanceMoved += horizontalMovement.magnitude;

        if (characterController.isGrounded && distanceMoved >= stepDistance)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                distanceMoved = 0f;
            }
            lastPosition = transform.position;
        }
        else
        {
            stepTimer = 0f; // reset timer if not moving
        }


        if (controlManager.useGyroscope)
        {
            GyroLook();
            GetTouchInput(); // still get touches for movement
            if(leftFingerId != -1)
            {
                Move();
            }
        }
        else
        {
            GetTouchInput();

            if (rightFingerId != -1)
            {
                LookAround();
            }

            if (leftFingerId != -1)
            {
                Move();
            }
        }

        // Countdown timer
        intervalTimer -= Time.deltaTime;

        if (intervalTimer <= 0f)
        {
            PlayIntervalSound();
            intervalTimer = intervalTime; // reset timer
        }

    }

    // ---------------- MOBILE TOUCH MODE ---------------- //
    void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        leftFingerId = t.fingerId;
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        rightFingerId = t.fingerId;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == leftFingerId) leftFingerId = -1;
                    else if (t.fingerId == rightFingerId) rightFingerId = -1;
                    break;

                case TouchPhase.Moved:
                    if (t.fingerId == rightFingerId)
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    else if (t.fingerId == leftFingerId)
                        moveInput = t.position - moveTouchStartPosition;
                    break;

                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId)
                        lookInput = Vector2.zero;
                    break;
            }
        }
    }

    void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;
        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

    // ---------------- GYROSCOPE MODE ---------------- //
    void GyroLook()
    {
        Quaternion deviceRotation = Input.gyro.attitude;

        // Convert from right-handed to Unity left-handed
        deviceRotation = new Quaternion(deviceRotation.x, deviceRotation.y, -deviceRotation.z, -deviceRotation.w);

        // Get Euler angles
        Vector3 euler = deviceRotation.eulerAngles;

        // Apply yaw (horizontal) to player body
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

        // Apply pitch (vertical) to camera
        cameraTransform.localRotation = Quaternion.Euler(euler.x, 0f, 0f); // minus fixes inverted pitch
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        footstepAudioSource.clip = footstepClips[index];
        footstepAudioSource.Play();
    }

    void PlayIntervalSound()
    {
        if (intervalClip == null || intervalAudioSource == null) return;

        intervalAudioSource.clip = intervalClip;
        intervalAudioSource.Play();
    }


}
