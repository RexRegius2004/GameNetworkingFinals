[System.Serializable]
public class PlayerPosition
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int PlayerHealth;
    public int Score;
    public PlayerPosition PlayerPosition;
}


