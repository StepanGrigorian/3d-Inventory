using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CursorUI : MonoBehaviour
{
    [SerializeField] private InteractorComponent Player;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        Player.OnCanInteractStatusChange.AddListener(UpdateColor);
    }

    private void OnDisable()
    {
        Player.OnCanInteractStatusChange.RemoveListener(UpdateColor);
    }

    private void UpdateColor(HoverType status)
    {
        switch (status)
        {
            case HoverType.None:
                image.color = Color.red;
                break;
            case HoverType.Type1:
                image.color = Color.green;
                break;
            case HoverType.Type2:
                image.color = Color.blue;
                break;
        }
    }
}