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

        if (Vector3.Distance(transform.position, player.position) < 0.5f)
        {
            // Touch detected
            player.GetComponent<PlayerController>().OnSnailTouched();
        }
    }
}


