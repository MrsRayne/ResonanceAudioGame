using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;


[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IGrounded, IMovementSpeed, ICollisionForce
{
    public bool Grounded => isGrounded;
    public float WalkSpeed => 4f;
    public float CollisionForce { get; private set; }
    public RigidbodyFirstPersonController.AdvancedSettings advancedSettings = new RigidbodyFirstPersonController.AdvancedSettings();
    public UnityEvent onLand = default;

    Coroutine movementCoroutine;

    /// Camera.
    public Camera mainCamera;

    // Character controller.
    private CharacterController characterController = null;

    // Player movement speed.
    private float movementSpeed = 5.0f;
    public float movementDuration = 0.4f;
    [Min(0.01f)] public float runMultiplier = 2f;

    // Target camera rotation in degrees.
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    // Maximum allowed vertical rotation angle in degrees.
    private const float clampAngleDegrees = 80.0f;

    // Camera rotation sensitivity.
    private const float sensitivity = 2.0f;

    // Spatial Sound Control
    public UnityEvent onPreLeftFootstep;
    public UnityEvent onLeftFootstep;
    public UnityEvent onPreRightFootstep;
    public UnityEvent onRightFootstep;
    public LayerMask obstacleMask;

    bool jump, previouslyGrounded, jumping, isGrounded;
    bool leftFootstep;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Vector3 rotation = mainCamera.transform.localRotation.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
    }


    private void FixedUpdate()
    {
        isGrounded = true;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorLock(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(false);
        }

#endif  // UNITY_EDITOR
        // Update the rotation.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Note that multi-touch control is not supported on mobile devices.
            mouseX = 0.0f;
            mouseY = 0.0f;
        }

        rotationX += sensitivity * mouseY;
        rotationY += sensitivity * mouseX;
        rotationX = Mathf.Clamp(rotationX, -clampAngleDegrees, clampAngleDegrees);
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0.0f);

        // Update the position.

        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");
        Vector3 movementDirection = new Vector3(movementX, 0.0f, movementY);
        movementDirection = mainCamera.transform.localRotation * movementDirection;
        movementDirection.y = 0.0f;

        float running = Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f;
        CollisionForce = WalkSpeed * running;

        characterController.SimpleMove(movementSpeed * movementDirection);

        // Initialize footstep audio effect

        if (Input.GetKey(KeyCode.W) && movementCoroutine == null && Grounded ||
            Input.GetKey(KeyCode.A) && movementCoroutine == null && Grounded ||
            Input.GetKey(KeyCode.S) && movementCoroutine == null && Grounded ||
            Input.GetKey(KeyCode.D) && movementCoroutine == null && Grounded) 
        { 
            movementCoroutine = StartCoroutine(GridBasedMovement());
        }

    }

    IEnumerator GridBasedMovement()
    {
        var startTime = Time.time;
        float running = Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f;

        PreFootstepEvents();

        while (Time.time < startTime + (movementDuration / running))
        {
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
        }

        FootstepEvents();        
        movementCoroutine = null;
    }


    // Sets the cursor lock for first-person control.
    private void SetCursorLock(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void PreFootstepEvents()
    {
        if (leftFootstep)
        {
            onPreLeftFootstep?.Invoke();
        }
        else
        {
            onPreRightFootstep?.Invoke();
        }
    }

    void FootstepEvents()
    {
        if (leftFootstep)
        {
            onLeftFootstep?.Invoke();
        }
        else
        {
            onRightFootstep?.Invoke();
        }
        leftFootstep = !leftFootstep;
    }
}

