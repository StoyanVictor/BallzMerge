using UnityEngine;
using CodeBase;

public class RecordChecker : MonoBehaviour
{
    [SerializeField] private GameCurrentMoveCounter moveCounter;
    private SaveLoadDataService<BestResult> _saveLoadDataService;
    public int currentRecord;
    private BestResult _bestResult;

    private void Awake()
    {
        _saveLoadDataService = new SaveLoadDataService<BestResult>();
        _bestResult = _saveLoadDataService.LoadData() ?? new BestResult(0);
        currentRecord = _bestResult.Result;
        moveCounter.ResetCounter(); // Сброс при старте
    }
    private void OnEnable()
    {
        GameEvents.OnBallUsed += IncreaseCurrentMove;
        GameEvents.OnBallUsed += CheckForRecord;
    }

    private void OnDisable()
    {
        GameEvents.OnBallUsed -= IncreaseCurrentMove;
        GameEvents.OnBallUsed -= CheckForRecord;
    }

    private void IncreaseCurrentMove()
    {
        moveCounter.currentMove++;
        GameEvents.MoveChanged(moveCounter.currentMove);
    }
    private void CheckForRecord()
    {
        if (_bestResult.Result < moveCounter.currentMove)
        {
            _bestResult.Result = moveCounter.currentMove;
            _saveLoadDataService.SaveData(_bestResult);
        }
    }
}