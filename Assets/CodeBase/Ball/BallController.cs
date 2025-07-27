using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private TrajectoryPredictor trajectory;
    [SerializeField] private float launchForce = 10f;
    public GridController gridController;
    public bool isLaunched;
    private Rigidbody2D rb;
    
    
    private IInputHandler inputHandler;
    public Action onCurentMoveIncrease;
    
    [Header("Platform")]
    [SerializeField] private bool usePcInput = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = usePcInput ? new PcInputHandler() : new MobileInputHandler();
    }
    void Update()
    {
        inputHandler.Update();
        if (isLaunched) return;
        CheckForAiming();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GridObject"))
        {
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
        
            gridController.OnBallHit(collision.gameObject, hitDirection);
            GameEvents.BallHits();
        }
    }
    private void CheckForAiming()
    {
        if (inputHandler.IsAiming)
        {
            trajectory.ShowTrajectory(transform.position, inputHandler.GetDirection(transform.position));
        }
        else if (inputHandler.IsReleased)
        {
            LaunchBall(inputHandler.GetDirection(transform.position));
            trajectory.HideTrajectory();
        }
    }

    private void LaunchBall(Vector2 direction)
    {
        onCurentMoveIncrease?.Invoke();
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
        isLaunched = true;
    }
    
}