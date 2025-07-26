using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private int maxReflections = 1;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask collisionMask;

    private LineRenderer lineRenderer;

    private void Awake() => lineRenderer = GetComponent<LineRenderer>();

    public void ShowTrajectory(Vector2 origin, Vector2 direction)
    {
        List<Vector3> points = new() { origin };
        Vector2 currentDir = direction.normalized;
        float distanceLeft = maxDistance;

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, currentDir, distanceLeft, collisionMask);

            if (hit)
            {
                points.Add(hit.point);
                distanceLeft -= hit.distance;

                if (distanceLeft <= 0) break;

                origin = hit.point + currentDir * 0.001f;
                currentDir = Vector2.Reflect(currentDir, hit.normal);
            }
            else
            {
                points.Add(origin + currentDir * distanceLeft);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void HideTrajectory() => lineRenderer.positionCount = 0;
}