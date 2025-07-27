using System;
using TMPro;
using UnityEngine;

public class GameInfoDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recordTxt;
    [SerializeField] private TextMeshProUGUI _curentMoveTxt;
    [SerializeField] private GameCurrentMoveCounter currentMoveCounter;
    [SerializeField] private RecordChecker recordChecker;
    [SerializeField] private GameObject loseScreen;

    private void Start()
    {
        _recordTxt.text ="Record: " + recordChecker.currentRecord.ToString();
        _curentMoveTxt.text ="CurrentMove: " + currentMoveCounter.currentMove.ToString();
        loseScreen.SetActive(false);
    }
    private void OnEnable()
    {
        GameEvents.OnMoveChanged += UpdateMoveUI;
        GameEvents.OnGameLose += Lose;
    }

    private void Lose()
    {
        loseScreen.SetActive(true);
    }

    private void OnDisable()
    {
        GameEvents.OnMoveChanged -= UpdateMoveUI;
        GameEvents.OnGameLose -= Lose;
    }

    private void UpdateMoveUI(int value)
    {
        _curentMoveTxt.text = $"CurrentMove: {value}";
    }
}
