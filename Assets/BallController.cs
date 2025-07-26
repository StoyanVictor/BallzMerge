using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private TrajectoryPredictor trajectory;
    [SerializeField] private float launchForce = 10f;

    private Rigidbody2D rb;
    private bool isLaunched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
// Пример вызова из кода мяча при столкновении
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GridObject"))
        {
            // Получаем направление удара (нормализованный вектор от центра мяча)
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
        
            GridManager.Instance.OnBallHit(collision.gameObject, hitDirection);
        }
    }
    private void Update()
    {
        if (isLaunched) return; // Если мяч уже запущен, не показываем траекторию

        if (Input.GetMouseButton(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;

            trajectory.ShowTrajectory(transform.position, direction);
        }
        else if (Input.GetMouseButtonUp(0)) // При отпускании ЛКМ — стреляем
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;

            LaunchBall(direction);
            trajectory.HideTrajectory();
        }
    }

    private void LaunchBall(Vector2 direction)
    {
        rb.isKinematic = false; // Если мяч был статичен
        rb.velocity = Vector2.zero; // Сброс скорости
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
        isLaunched = true;
    }
}