using UnityEngine;

public interface IInteractable
{
    bool StartInteraction(IInteractor interactor);
    void EndInteraction(IInteractor interactor);
    bool IsInteracting { get; }
}