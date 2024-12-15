using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(InteractorComponent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float interactionRange = 100f;

    [SerializeField] private LayerMask interactionMask;
    [SerializeField] private new Camera camera;

    private CharacterController characterController;
    private InteractorComponent interactor;
    private Vector2 cameraInput;
    private Vector2 movementInput;
    private IInteractable currentInteractable;
    private HoverType currentHoverType = HoverType.None;
    private bool isInteracting = false;

    private void OnValidate()
    {
        if (camera == null)
        {
            camera = GetComponentInChildren<Camera>();
            if (camera == null)
            {
                Debug.LogError("Character must have child camera!");
            }
        }
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        interactor = GetComponentInChildren<InteractorComponent>();
        interactor.camera = camera;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CaptureInput();
        MovePlayer();
        RotateCamera();
        PerformInteraction();
    }

    private void FixedUpdate()
    {
        CheckForInteraction();
    }

    private void CaptureInput()
    {
        cameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        movementInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    private void MovePlayer()
    {
        Vector3 forwardMovement = transform.forward * movementInput.x * speed * Time.deltaTime;
        Vector3 rightMovement = transform.right * movementInput.y * speed * Time.deltaTime;
        characterController.Move(forwardMovement + rightMovement);
    }

    private void RotateCamera()
    {
        float inputX = cameraInput.x;
        float inputY = cameraInput.y * mouseSensitivity;
        float cameraVerticalRotation = camera.transform.localEulerAngles.x;

        if (cameraVerticalRotation > 180f)
        {
            cameraVerticalRotation -= 360f;
        }

        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation - inputY, -90f, 90f);
        camera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        transform.Rotate(Vector3.up * inputX * mouseSensitivity);
    }

    private void CheckForInteraction()
    {
        RaycastHit hit;

        IInteractable interactable = null;
        IHover hover = null;
        if (!isInteracting)
        {
            if (
                Physics.Raycast(
                    camera.transform.position,
                    camera.transform.forward,
                    out hit, interactionRange,
                    interactionMask,
                    QueryTriggerInteraction.Ignore))
            {
                hit.collider.TryGetComponent(out interactable);
                hit.collider.TryGetComponent(out hover);
                currentHoverType = hover.GetHoverType;
            }
            else
            {
                currentHoverType = HoverType.None;
            }
            currentInteractable = interactable;
            interactor.OnCanInteractStatusChange?.Invoke(currentHoverType);
        }
    }

    private void PerformInteraction()
    {
        if (Input.GetMouseButtonDown(0) && currentInteractable != null && !currentInteractable.IsInteracting)
        {
            if (currentInteractable.StartInteraction(interactor))
            {
                isInteracting = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) && currentInteractable != null && currentInteractable.IsInteracting)
        {
            currentInteractable.EndInteraction(interactor);
            isInteracting = false;
        }
    }
}
