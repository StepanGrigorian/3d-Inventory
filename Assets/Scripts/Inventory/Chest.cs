using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Chest : MonoBehaviour, IStorage, IHover, IInteractable
{
    [SerializeField] private SerializedDictionary<StorableType, Transform> itemsPositions = new();

    [SerializeField] private Animator uiAnimator;

    [SerializeField] private ChestUI chestUI;

    private Animator animator;

    private readonly UnityEvent<IStorable> onStore = new();
    private readonly UnityEvent<IStorable> onRemove = new();

    [SerializeField] private Dictionary<StorableType, List<IStorable>> storables = new();

    private bool isInteracting = false;

    public Dictionary<StorableType, List<IStorable>> Storables => storables;
    public HoverType GetHoverType => HoverType.Type2;
    public UnityEvent<IStorable> OnStore => onStore;
    public UnityEvent<IStorable> OnRemove => onRemove;
    public bool IsInteracting => isInteracting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (StorableType type in Enum.GetValues(typeof(StorableType)))
        {
            storables.Add(type, new List<IStorable>());
        }
        onRemove.AddListener(Remove);
    }

    public void Interact(bool status)
    {
        animator.ResetTrigger("Open");
        animator.ResetTrigger("Close");
        animator.SetTrigger(status ? "Open" : "Close");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IStorable item) && !item.IsStored)
        {
            Interact(true);
            if (!item.ListenersAdded)
            {
                item.Storage = this;
                onStore.AddListener(Store);
                item.ListenersAdded = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IStorable item))
        {
            Interact(false);
            if (item.ListenersAdded)
            {
                item.Storage = null;
                onStore.RemoveListener(Store);
                item.ListenersAdded = false;
            }
        }
    }

    public void Store(IStorable item)
    {
        if (storables[item.GetAttributes.type] != null)
        {
            storables[item.GetAttributes.type].Add(item);
            item.IsStored = true;
            item.DisablePhysics();
            StartCoroutine(StoreIE(item));
        }
    }

    private IEnumerator StoreIE(IStorable item, float time = 0.5f)
    {
        if (itemsPositions.ContainsKey(item.GetAttributes.type))
        {
            Vector3 start = item.Transform.position;
            Vector3 target = itemsPositions[item.GetAttributes.type].position;

            Quaternion startRotation = item.Transform.rotation;
            Quaternion targetRotation = itemsPositions[item.GetAttributes.type].rotation;
            float progress = 0;

            while (progress < 1f)
            {
                item.Transform.position = Vector3.Lerp(start, target, progress);
                item.Transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
                progress += Time.deltaTime / time;
                yield return null;
            }
        }
    }

    public void RemoveItemByType(StorableType type)
    {
        if (storables[type]?.Count > 0)
        {
            IStorable item = storables[type][0];
            storables[type].RemoveAt(0);
            onRemove.Invoke(item);
        }
    }

    public void Remove(IStorable item)
    {
        item.IsStored = false;
        item.EnablePhysics();
        item.Rigidbody.AddForce((transform.up + transform.forward) * 3f, ForceMode.Impulse);
    }

    public bool StartInteraction(IInteractor interactor)
    {
        isInteracting = true;
        Interact(true);
        uiAnimator.SetTrigger("Show");
        return true;
    }

    public void EndInteraction(IInteractor interactor)
    {
        isInteracting = false;
        Interact(false);
        uiAnimator.SetTrigger("Hide");
        chestUI.HandleInput();
    }
}
