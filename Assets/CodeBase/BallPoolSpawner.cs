using System.Collections.Generic;
using UnityEngine;

public class BallPoolSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int poolSize = 5;

    private Queue<GameObject> ballPool = new Queue<GameObject>();

    private void Start()
    {
        CreatePool();
        SpawnNewBall();
    }
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false);
            ballPool.Enqueue(ball);
        }
    }
    private GameObject GetBallFromPool()
    {
        if (ballPool.Count > 0)
        {
            GameObject ball = ballPool.Dequeue();
            ball.SetActive(true);
            return ball;
        }
        else
        {
            return Instantiate(ballPrefab);
        }
    }
    private void ReturnBallToPool(GameObject ball)
    {
        ball.GetComponent<BallController>().isLaunched = false;
        ball.SetActive(false);
        ballPool.Enqueue(ball);
    }
    private void SpawnNewBall()
    {
        GameObject ball = GetBallFromPool();
        ball.transform.position = new Vector3(spawnPos.position.x, -7.1f, 0);
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Сброс скорости
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.contacts[0];
        Vector3 contactPosition = contact.point;
        spawnPos.position = contactPosition;

        ReturnBallToPool(other.gameObject); // Вместо Destroy
        GridManager.Instance.MoveAllObjectsUp();
        GridManager.Instance.currentMove++;
        SpawnNewBall();
    }
}