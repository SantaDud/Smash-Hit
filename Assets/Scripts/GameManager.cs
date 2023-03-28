using UnityEngine;
using System.Collections;
using Unity.Burst.CompilerServices;

public class GameManager : MonoBehaviour
{
    public static int level = 1;
    public static int minDiamonds;
    public static int maxDiamonds;
    public static int numberOfDiamonds;

    public static GameManager Instance;
    
    public static bool isGameOver = false;
    public static bool isLevelComplete = false;
    public static bool isBonusLevel = false;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Expert,
    }

    public static Difficulty difficulty = Difficulty.Easy;

    private void Awake()
    {
        Application.targetFrameRate= 200;
    }

    void Start()
    {
        if (!Instance)
            Instance = this;

        else
        {
            Destroy(this);
            return;
        }

        if (PlayerPrefs.GetInt("ageSet", 0) == 0)
            UIManager.Instance.EnableAgeScreen();
        
        UIManager.Instance.EnableMainMenu();
        //StartCoroutine("PrintVariables");
    }

    IEnumerator PrintVariables()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Debug.Log($"Number of Diamonds: {numberOfDiamonds}");

            yield return new WaitForSecondsRealtime(3);
        }
    }

    public void SetDifficulty(int d) { difficulty = (Difficulty) d; }

    public void StartGame()
    {
        UIManager.Instance.EnableGameplay();

        level = 1;
        minDiamonds = level;
        maxDiamonds = level + 2;
        numberOfDiamonds = 0;
        FindObjectOfType<Ball>().Initialize();

        UIManager.Instance.AddScore(true);
        FindObjectOfType<GameplayButton>().hitValueChecked = true;
    }

    public void GameOver()
    {
        isGameOver = false;
        isLevelComplete = false;

        UIManager.Instance.InitializePowerUps();
        UIManager.Instance.EnableMainMenu();
        level = 0;
        Ball.hit = 0;
        Ball.diamondsHit = 0;
    }

    public void Retry()
    {
        //if (AdScript.Instance.IsLoadedRewarded())
        //{
            isGameOver = false;

            EasyRetry();

            if (difficulty == Difficulty.Medium)
                MediumRetry();
        //}

        //else
        //    UIManager.Instance.EnableInfoText();
    }

    void EasyRetry()
    {
        //AdScript.Instance.ShowRewarded();

        int extraBalls = numberOfDiamonds - Ball.diamondsHit;
        Ball.hit = Ball.numberOfBalls;
        Ball.hit -= extraBalls;

        FindObjectOfType<Ball>().ContinueLevelSetup(extraBalls);
        UIManager.Instance.EnableGameplay();
    }

    void MediumRetry()
    {
        FindObjectOfType<Motor>().MediumRetrySetup();
    }
}
