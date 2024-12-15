using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

[RequireComponent(typeof(Animator))]
public class Chest : MonoBehaviour, IStorage, IHover, IInteractable
{
    [SerializeField] private SerializedDictionary<StorableType, Transform> ItemsPositions = new();
    [SerializeField] private Animator UIAnimator;
    [SerializeField] private ChestUI ChestUI;

    private Animator animator;

    private UnityEvent<IStorable> onStore = new();
    private UnityEvent<IStorable> onRemove = new();

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
        foreach(StorableType type in Enum.GetValues(typeof(StorableType)))
        {
            storables.Add(type, new());
        }
        OnRemove.AddListener(Remove);
    }

    public void Interact(bool status)
    {
        animator.SetTrigger(status ? "Open" : "Close");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IStorable item) && !item.IsStored)
        {
            Interact(true);
            item.Storage = this;
            OnStore.AddListener(Store);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IStorable item))
        {
            Interact(false);
            item.Storage = null;
            OnStore.RemoveListener(Store);
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
        if (ItemsPositions.ContainsKey(item.GetAttributes.type))
        {
            Vector3 start = item.Transform.position;
            Vector3 target = ItemsPositions[item.GetAttributes.type].transform.position;

            Quaternion startRotation = item.Transform.rotation;
            Quaternion targetRotation = ItemsPositions[item.GetAttributes.type].transform.rotation;
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
            var item = storables[type][0];
            storables[type].RemoveAt(0);
            OnRemove?.Invoke(item);
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
        UIAnimator.SetTrigger("Show");
        return true;
    }

    public void EndInteraction(IInteractor interactor)
    {
        isInteracting = false;
        Interact(false);
        UIAnimator.SetTrigger("Hide");
        ChestUI.HandleInput();
    }
}
