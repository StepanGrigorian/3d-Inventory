using UnityEngine;

[CreateAssetMenu(menuName="Storage Attributes")]
public class StorableAttributes : ScriptableObject
{
    public float weight = 1f;
    public new string name;
    public int id;
    public StorableType type;
}
