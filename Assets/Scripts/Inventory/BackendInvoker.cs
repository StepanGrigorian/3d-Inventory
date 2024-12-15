using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class BackendInvoker : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;
    private IStorage Storage;
    private void Awake()
    {
        Storage = GetComponent<IStorage>();
    }

    private void OnEnable()
    {
        Storage.OnStore.AddListener(Added);
        Storage.OnRemove.AddListener(Removed);
    }

    private void OnDisable()
    {
        Storage.OnStore.RemoveListener(Added);
        Storage.OnRemove.RemoveListener(Removed);
    }

    private void Removed(IStorable item)
    {
        StartCoroutine(SendRequestCoroutine(item.GetAttributes.id, "Removed"));
    }

    private void Added(IStorable item)
    {
        StartCoroutine(SendRequestCoroutine(item.GetAttributes.id, "Added"));
    }
    private IEnumerator SendRequestCoroutine(int itemId, string action)
    {
        string url = settings.url;
        string bearerToken = settings.bearerToken;

        RequestBody requestBody = new RequestBody() { itemId = itemId, action = action};

        string json = JsonUtility.ToJson(requestBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}

public struct RequestBody
{
    public int itemId;
    public string action;
}