using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    public TextMeshProUGUI scoreText;
    private PlayerController player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AssignPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    private void Update()
    {
        if (player != null)
        {
            scoreText.text = $"Name: {player.playerName}\nScore: {player.score}\nHealth: {player.health:0}";
        }
    }
}

