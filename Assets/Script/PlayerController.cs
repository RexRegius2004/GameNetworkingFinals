using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float health = 100;
    public int score = 0;
    public string playerName = "Unnamed";

    public GameObject snailPrefab;
    private GameObject snailInstance;

    public ServerManager serverManager;

    public float moveSpeed = 5f;
    private Rigidbody rb;
    private float snailContactTime = 0f;
    private float scoreTimer = 0f;

    void Start()
    {
        if (!IsOwner) return;

        playerName = "Player_" + Random.Range(1, 999);
        rb = GetComponent<Rigidbody>();

        snailInstance = Instantiate(snailPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        snailInstance.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        snailInstance.GetComponent<SnailFollow>().player = this.transform;
        PlayerHUD.Instance.AssignPlayer(this);

        StartCoroutine(serverManager.GetPlayerData());
    }

    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);

        if (health > 0)
        {
            scoreTimer += Time.deltaTime;
            if (scoreTimer >= 1f)
            {
                score++;
                scoreTimer = 0f;

                ServerManager.Instance.UpdatePlayerScore(score);
            }
        }

        if (Time.frameCount % 60 == 0)
        {
            ServerManager.Instance.UpdatePlayerPosition(transform.position);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;

        Debug.Log("Snail touched: " + other.name);
        if (other.CompareTag("Snail"))
        {
            snailContactTime += Time.deltaTime;
            health -= Time.deltaTime * 10f;

            if (health <= 0)
            {
                Debug.Log("YOU LOST!");
            }

            ServerManager.Instance.UpdatePlayerHealth(health);
        }
    }
}
