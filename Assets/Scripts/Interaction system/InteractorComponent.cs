using UnityEngine;
using UnityEngine.Events;

public class InteractorComponent : MonoBehaviour, IInteractor
{
    [HideInInspector] public new Camera camera;

    private UnityEvent<HoverType> onInteractionStatusChange = new();
    public UnityEvent<HoverType> OnCanInteractStatusChange => onInteractionStatusChange;
    public Transform Transform => camera.transform;
}
