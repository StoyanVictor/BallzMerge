using System;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject ball;

    private void Start()
    {
        SpawnNewBall();
    }
    private void SpawnNewBall()
    {
        Instantiate(ball, new Vector3(spawnPos.position.x,-7.1f), Quaternion.identity);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.contacts[0];
        Vector3 contactPosition = contact.point;
        spawnPos.position = contactPosition;
        Destroy(other.gameObject);
        GridManager.Instance.MoveAllObjectsUp();
        GridManager.Instance.currentMove++;
        SpawnNewBall();
    }
}