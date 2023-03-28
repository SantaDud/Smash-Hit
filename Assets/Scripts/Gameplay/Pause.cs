using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool isPaused = false;

    public void PauseGame()
    {
        if (!isPaused)
        {
            gameObject.SetActive(true);
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            gameObject.SetActive(false);
            isPaused = false;
            Time.timeScale = 1;
        }
    }

    public void MainMenu()
    {
        Resume();
        
        GameManager.Instance.GameOver();
    }
}
