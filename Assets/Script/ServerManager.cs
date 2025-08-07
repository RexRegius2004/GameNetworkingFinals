using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ServerManager : MonoBehaviour
{
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
            Debug.Log("Raw JSON: " + json);
            playerData = JsonConvert.DeserializeObject<PlayerData>(json);
        }
        else
        {
            Debug.LogError("GET Error: " + request.error);
        }
    }

    public IEnumerator PostPlayerData()
    {
        string json = JsonConvert.SerializeObject(playerData);
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data sent successfully");
        }
        else
        {
            Debug.LogError("POST Error: " + request.error);
        }
    }
}
