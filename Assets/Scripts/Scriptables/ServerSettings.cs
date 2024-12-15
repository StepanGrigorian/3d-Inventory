using UnityEngine;

[CreateAssetMenu(menuName ="Backend/Server settings")]
public class ServerSettings : ScriptableObject
{
    public string url;
    public string bearerToken;
}
