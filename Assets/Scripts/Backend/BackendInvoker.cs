using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BackendInvoker : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;

    private IStorage storage;

    private void Awake()
    {
        storage = GetComponent<IStorage>();
    }

    private void OnEnable()
    {
        storage.OnStore.AddListener(OnItemAdded);
        storage.OnRemove.AddListener(OnItemRemoved);
    }

    private void OnDisable()
    {
        storage.OnStore.RemoveListener(OnItemAdded);
        storage.OnRemove.RemoveListener(OnItemRemoved);
    }

    private void OnItemRemoved(IStorable item)
    {
        StartCoroutine(SendRequestCoroutine(item.GetAttributes.id, "Removed"));
    }

    private void OnItemAdded(IStorable item)
    {
        StartCoroutine(SendRequestCoroutine(item.GetAttributes.id, "Added"));
    }

    private IEnumerator SendRequestCoroutine(int itemId, string action)
    {
        string url = settings.url;
        string bearerToken = settings.bearerToken;

        RequestBody requestBody = new RequestBody
        {
            itemId = itemId,
            action = action
        };

        string json = JsonUtility.ToJson(requestBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Server Response: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
    }
}

public struct RequestBody
{
    public int itemId;
    public string action;
}
