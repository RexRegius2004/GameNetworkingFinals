using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public int lives = 3;
    public int health = 100;
    public int score = 0;
    public string playerName = "Unnamed";

    public GameObject snailPrefab;
    private GameObject snailInstance;

    public ServerManager serverManager;

    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        if (!IsOwner) return;

        playerName = "Player_" + Random.Range(1, 999);
        rb = GetComponent<Rigidbody>();

        // Spawn snail with ownership
        snailInstance = Instantiate(snailPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        snailInstance.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        snailInstance.GetComponent<SnailFollow>().player = this.transform;

        StartCoroutine(serverManager.GetPlayerData());
    }

    void Update()
    {
        if (!IsOwner) return;

        // Movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, 0, moveZ);
        rb.linearVelocity = movement * moveSpeed;

        // Update server position data
        serverManager.playerData.PlayerPosition = new PlayerPosition
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };

        // Manual sync to server with space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(serverManager.PostPlayerData());
        }
    }

    public void OnSnailTouched()
    {
        lives--;

        if (lives <= 0)
        {
            Debug.Log("You lost!");
            // Optionally despawn or disable player
        }

        score -= 10;
        health -= 5;

        serverManager.playerData.Score = score;
        serverManager.playerData.PlayerHealth = health;

        StartCoroutine(serverManager.PostPlayerData());
    }
}
