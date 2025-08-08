using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public string serverUrl = "https://localhost:7047/api/data";
    public PlayerData playerData;

    public IEnumerator GetPlayerData()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("GET JSON: " + json);

            playerData = JsonConvert.DeserializeObject<PlayerData>(json);
        }
        else Debug.LogError("GET Error: " + request.error);
    }

    public IEnumerator PostPlayerData()
    {
        string json = JsonConvert.SerializeObject(playerData);
        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("POST success: " + json);
        else
            Debug.LogError("POST Error: " + request.error);
    }

    public void UpdatePlayerHealth(float health)
    {
        playerData.PlayerHealth = (int)health;
        StartCoroutine(PostPlayerData());
    }

    public void UpdatePlayerPosition(Vector3 pos)
    {
        playerData.PlayerPosition = new PlayerPosition() { x = pos.x, y = pos.y, z = pos.z };
        StartCoroutine(PostPlayerData());
    }

    public void UpdatePlayerScore(int newScore)
    {
        playerData.Score = newScore;
        StartCoroutine(PostPlayerData());
    }
}
