using UnityEngine;
using UnityEngine.Events;

public interface IInteractor
{
    Transform Transform { get; }
    UnityEvent<HoverType> OnCanInteractStatusChange { get; }
}