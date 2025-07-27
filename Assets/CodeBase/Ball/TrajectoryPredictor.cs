using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxReflections = 3; 
    [SerializeField] private float maxDistance = 20f; 
    [SerializeField] private LayerMask collisionMask; 

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void ShowTrajectory(Vector2 origin, Vector2 direction)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        Vector2 currentPosition = origin;
        Vector2 currentDirection = direction.normalized;
        float remainingDistance = maxDistance;

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentDirection, remainingDistance, collisionMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);
                remainingDistance -= hit.distance;
                if (remainingDistance <= 0) break;
                currentPosition = hit.point + (hit.normal);
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
            }
            else
            {
                points.Add(currentPosition + currentDirection * remainingDistance);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}