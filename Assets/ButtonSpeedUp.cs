using UnityEngine;
public class ButtonSpeedUp : MonoBehaviour
{
    private bool isActive;
    public void SpeedUp()
    {
        if (!isActive)
        {
            Time.timeScale *= 5;
            isActive = true;
        }
        else
        {
            Time.timeScale = 1;
            isActive = false;
        }

    }
}
