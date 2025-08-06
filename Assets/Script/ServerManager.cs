using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class ServerManager : MonoBehaviour
{
    private string serverUrl = "https://localhost:7047/api/data"; // <- Your local server URL
    public PlayerData playerData;

    void Start()
    {
        StartCoroutine(GetPlayerData());
    }

    public IEnumerator GetPlayerData()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Raw JSON Response:\n" + json);

            playerData = JsonConvert.DeserializeObject<PlayerData>(json);

            Debug.Log("Parsed Player Name: " + playerData.PlayerName);
            Debug.Log("Parsed Score: " + playerData.Score);
        }
        else
        {
            Debug.LogError("GET Error: " + request.error);
        }
    }



    public void UpdateDataToServer()
    {
        StartCoroutine(PostPlayerData());
    }

    public IEnumerator PostPlayerData()
    {
        string json = JsonUtility.ToJson(playerData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificate(); // bypass HTTPS cert

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data Updated Successfully");
        }
        else
        {
            Debug.LogError("POST Error: " + request.error);
        }
    }

    //Testing Scripts

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ModifyPlayerData(); // Press "U" to modify and update
        }
    }

    public void ModifyPlayerData()
    {
        playerData.PlayerName = "UpdatedPlayer";
        playerData.PlayerHealth = 100;
        playerData.Score += 300;
        playerData.PlayerPosition = new PlayerPosition { x = 3.2f, y = 3.0f, z = 3.5f };

        // LOG BEFORE POSTING
        Debug.Log("Modified: " + playerData.PlayerName);
        Debug.Log("Modified: " + playerData.Score);
        Debug.Log("Modified Pos: " + playerData.PlayerPosition.x);

        StartCoroutine(PostPlayerData());
        StartCoroutine(ReFetchAfterDelay());
    }

    private IEnumerator ReFetchAfterDelay()
    {
        yield return new WaitForSeconds(2f); // wait for POST to complete
        StartCoroutine(GetPlayerData());
    }

}