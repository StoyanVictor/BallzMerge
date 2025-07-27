using System;
using UnityEngine;
public class SpeedSetup : MonoBehaviour
{
    private bool isActive;
    public void SpeedUp()
    {
        if (!isActive)
        {
            Time.timeScale = 5;
            isActive = true;
        }
        else
        {
            Time.timeScale = 2;
            isActive = false;
        }
    }
    private void Start()
    {
        Time.timeScale = 2;
    }
    private void OnEnable()
    {
        GameEvents.OnGameLose += LoseSpeed;
    }

    private void OnDisable()
    {
        GameEvents.OnGameLose -= LoseSpeed;
    }

    private void LoseSpeed()
    {
        Time.timeScale = 0;
    }
}
