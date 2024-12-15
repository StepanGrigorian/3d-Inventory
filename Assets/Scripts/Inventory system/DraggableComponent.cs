using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DraggableComponent : MonoBehaviour, IInteractable, IStorable, IHover
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private StorableAttributes attributes;

    private new Rigidbody rigidbody;
    private new Collider collider;

    private bool isInteracting;
    public bool IsInteracting => isInteracting;

    public Transform Transform => transform;
    public Rigidbody Rigidbody => rigidbody;

    public HoverType GetHoverType => HoverType.Type1;
    public StorableAttributes GetAttributes => attributes;

    private Chest storage;
    public IStorage Storage
    {
        get => storage;
        set => storage = (Chest)value;
    }

    private bool isStored;
    public bool IsStored
    {
        get => isStored;
        set => isStored = value;
    }

    private bool listenersAdded = false;
    public bool ListenersAdded
    {
        get => listenersAdded;
        set => listenersAdded = value;
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public bool StartInteraction(IInteractor interactor)
    {
        if (!isStored)
        {
            isInteracting = true;
            DisablePhysics();
            transform.SetParent(interactor.Transform);
            return true;
        }
        return false;
    }

    public void EnablePhysics()
    {
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        collider.isTrigger = false;
    }

    public void DisablePhysics()
    {
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        collider.isTrigger = true;
    }

    public void EndInteraction(IInteractor interactor)
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + collider.bounds.extents.y;
            transform.position = newPosition;
        }

        isInteracting = false;
        EnablePhysics();
        transform.SetParent(null);

        Storage?.OnStore?.Invoke(this);
    }
}
