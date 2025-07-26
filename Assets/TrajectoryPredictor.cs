using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    public int maxReflections = 1;
    public float maxDistance = 20f;
    public LayerMask collisionMask;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowTrajectory(Vector2 origin, Vector2 direction)
    {
        // Создаём список точек (начальная точка + возможные отражения)
        var points = new System.Collections.Generic.List<Vector3>();
        points.Add(origin); // Первая точка - старт

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;
        float remainingDistance = maxDistance;

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, remainingDistance, collisionMask);

            if (hit.collider != null)
            {
                // Добавляем точку удара
                points.Add(hit.point);

                // Уменьшаем оставшуюся дистанцию
                remainingDistance -= Vector2.Distance(currentPos, hit.point);

                // Если дистанция закончилась - выходим
                if (remainingDistance <= 0) 
                    break;

                // Меняем направление на отражённое
                currentDir = Vector2.Reflect(currentDir, hit.normal);
                currentPos = hit.point + currentDir * 0.001f; // Микро-сдвиг от коллайдера
            }
            else
            {
                // Если нет столкновения - добавляем конечную точку
                points.Add(currentPos + currentDir * remainingDistance);
                break;
            }
        }

        // Применяем точки к LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}