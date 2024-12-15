using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIItem : MonoBehaviour
{
    [SerializeField] private ChestUI parent;
    
    public StorableType storableType;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetEnabled()
    {
        switch (storableType)
        {
            case StorableType.None:
                break;
            case StorableType.Blue:
                image.color = Color.blue;
                break;
            case StorableType.Green:
                image.color = Color.green;
                break;
            case StorableType.Yellow:
                image.color = Color.yellow;
                break;
        }
    }

    public void SetDisabled()
    {
        image.color = Color.grey;
    }
}
