using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("Wheel and Ball")]
    [SerializeField] GameObject wheel;
    [SerializeField] public GameObject ball;
    
    public static bool isMenuActive;
    public static bool isPlaying;

    [Header("Screens")]
    [SerializeField] GameObject ageScreen;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gameplay;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject shop;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject gameWin;

    [Header("Gameplay UI")]
    [SerializeField] TMPro.TextMeshProUGUI gamePlayLevel;
    [SerializeField] TMPro.TextMeshProUGUI gamePlayScore;
    [SerializeField] Transform subStageDots;
    [SerializeField] PowerUps powerUp;
    [SerializeField] Transform diamonds;

    [Header("Game Over Texts")]
    [SerializeField] TMPro.TextMeshProUGUI gameOverLevel;
    [SerializeField] TMPro.TextMeshProUGUI gameOverScore;
    [SerializeField] GameObject infoText;

    [Header("Level Complete Texts")]
    [SerializeField] TMPro.TextMeshProUGUI levelCompleteLevel;
    [SerializeField] TMPro.TextMeshProUGUI levelCompleteScore;

    int _number;
    bool _enable;
    
    public static UIManager Instance;
    #endregion

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        else
            Destroy(this);
    }

    public void EnableAgeScreen() { ageScreen.SetActive(true); }

    public void EnableMainMenu()
    {
        isMenuActive = true;
        isPlaying = false;

        mainMenu.SetActive(true);
        ball.SetActive(false);
        wheel.SetActive(false);
        gameplay.SetActive(false);
        pause.SetActive(false);
        gameWin.SetActive(false);
        gameOver.SetActive(false);
    }

    public void EnableGameplay()
    {
        if (GameManager.isLevelComplete)
            GameManager.isLevelComplete = false;

        isMenuActive = false;
        isPlaying = true;

        mainMenu.SetActive(false);
        ball.SetActive(true);
        wheel.SetActive(true);
        gameplay.SetActive(true);
        gameWin.SetActive(false);
        gameOver.SetActive(false);
        InitializePowerUps();
    }

    public void InitializePowerUps() { powerUp.Initialize(); }

    public void EnableGameOver()
    {
        gameplay.SetActive(false);

        gameOverLevel.text = GameManager.level.ToString();
        gameOverScore.text = gamePlayScore.text;
        gameOver.SetActive(true);
    }

    public void EnableInfoText()
    {
        infoText.SetActive(true);
        Invoke("DisableInfoText", 1f);
    }

    void DisableInfoText() { infoText.SetActive(false); }

    public void EnableLevelWin()
    {
        gameplay.SetActive(false);

        levelCompleteLevel.text = GameManager.level.ToString();
        levelCompleteScore.text = gamePlayScore.text;
        gameWin.SetActive(true);
    }

    public void AddLevel(bool initialize = false)
    {
        if (initialize)
        {
            gamePlayLevel.text = "Level 1";
            SetDots(true);
            return;
        }

        gamePlayLevel.text = "Level " + (++GameManager.level).ToString();

        if (GameManager.level % 5 == 1)
            SetDots(true);

        else
            SetDots();

        if (GameManager.level % 5 == 0 && GameManager.difficulty == GameManager.Difficulty.Easy)
            GameManager.isBonusLevel = true;
    }

    public void AddScore(bool initialize = false, int value = 1)
    {
        if (initialize)
        {
            gamePlayScore.text = "0";
            return;
        }

        PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds", 0) + value);
        CurrencyManager.Instance.SetCoins();

        gamePlayScore.text = (int.Parse(gamePlayScore.text) + value).ToString();
    }

    public void EnableDiamonds(int number, bool enable)
    {
        _number = number;
        _enable = enable;
        
        StopCoroutine("Enable");
        StartCoroutine("Enable");
    }

    IEnumerator Enable()
    {
        int i = 0;
        while (true)
        {
            int childDiamonds = diamonds.childCount;

            // Sync with Fixed Update
            yield return new WaitForFixedUpdate();

            Debug.Log($"E: {_enable}\nI: {i}");

            if (_number <= childDiamonds && i < _number)
                diamonds.GetChild(i).gameObject.SetActive(_enable);
            
            else
                break;

            if (!_enable)
                diamonds.GetChild(i).GetComponent<Image>().enabled = true;

            i++;
            // Wait for the rotation time to end
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SetDots(bool initialize = false)
    {
        // F4B54B
        Color newColor;

        ColorUtility.TryParseHtmlString("#F4B54B", out newColor);

        if (initialize)
        {
            for (int i = 1; i < subStageDots.childCount; i++)
                subStageDots.GetChild(i).GetComponent<Image>().color = Color.white;
            
            return;
        }
        
        for (int i = 0; i < subStageDots.childCount; i++)
        {
            if (subStageDots.GetChild(i).GetComponent<Image>().color == newColor)
                continue;
            else
            {
                subStageDots.GetChild(i).GetComponent<Image>().color = newColor;
                return;
            }
        }
    }
}
