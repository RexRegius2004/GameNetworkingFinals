using Unity.Netcode;
using UnityEngine;

public class SnailFollow : NetworkBehaviour
{
    public Transform player;
    public float speed = 2f;

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}


