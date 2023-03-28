using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject shop;
    [SerializeField] Sprite[] ballSprites;
    [SerializeField] Image shopBall;
    [SerializeField] GameObject select;
    [SerializeField] GameObject selected;
    [SerializeField] GameObject buy;
    [SerializeField] GameObject price;
    [SerializeField] SpriteRenderer player;
    [SerializeField] int ballPrice;

    Animator shopAnimator;

    private void Awake()
    {
        shopAnimator = GetComponent<Animator>();

        if (PlayerPrefs.GetInt("shopFirstStart", 0) == 0)
        {
            PlayerPrefs.SetInt("shopFirstStart", 1);
            PlayerPrefs.SetInt("selectedBall", 0);

            PlayerPrefs.SetInt("Ball 0", 1);
            for (int i = 1; i < ballSprites.Length; i++)
                PlayerPrefs.SetInt($"Ball {i}", 0);
        }
    }

    public void Open() 
    {
        shop.SetActive(true);
        Initialize();
    }

    public void Close()
    {
        shopAnimator.SetBool("isClosing", true);
        Invoke("Deactivate", .5f);
    }

    void Deactivate() { shop.SetActive(false); }

    public void Next()
    {
        int index = PlayerPrefs.GetInt("selectedBall", 0) == ballSprites.Length - 1 ? 0 : PlayerPrefs.GetInt("selectedBall", 0) + 1;
        SetSprite(index);
    }

    public void Previous()
    {
        int index = PlayerPrefs.GetInt("selectedBall", 0) == 0 ? ballSprites.Length - 1 : PlayerPrefs.GetInt("selectedBall", 0) - 1;
        SetSprite(index);
    }

    void SetSprite(int index)
    {
        shopBall.sprite = ballSprites[index];
        PlayerPrefs.SetInt("selectedBall", index);
        SetButtons(index);
    }

    public void Buy()
    {
        int diamonds = PlayerPrefs.GetInt("Diamonds");
        int index = PlayerPrefs.GetInt("selectedBall", 0);
        
        if (diamonds >= ballPrice)
        {
            PlayerPrefs.SetInt("Diamonds", diamonds - ballPrice);
            CurrencyManager.Instance.SetCoins();
            
            PlayerPrefs.SetInt($"Ball {index}", 1);
            PlayerPrefs.SetInt("selectedBall", index);
            Select();
        }
    }

    public void Select()
    {
        int selectedBallIndex = PlayerPrefs.GetInt("selectedBall", 0);
        PlayerPrefs.SetInt("playerBall", selectedBallIndex);
        player.sprite = ballSprites[selectedBallIndex];
        
        SetSelected();
    }

    void SetButtons(int index)
    {
        if (PlayerPrefs.GetInt($"Ball {index}", 0) == 0)
            SetBuy();

        else if (PlayerPrefs.GetInt("selectedBall") == PlayerPrefs.GetInt("playerBall"))
            SetSelected();

        else
            SetSelect();
    }

    void SetBuy()
    {
        buy.SetActive(true);
        select.SetActive(false);
        selected.SetActive(false);
        price.SetActive(true);
    }

    void SetSelect()
    {
        buy.SetActive(false);
        select.SetActive(true);
        selected.SetActive(false);
        price.SetActive(false);
    }

    void SetSelected()
    {
        buy.SetActive(false);
        select.SetActive(false);
        selected.SetActive(true);
        price.SetActive(false);
    }

    void Initialize()
    {
        int index = PlayerPrefs.GetInt("playerBall", 0);
        PlayerPrefs.SetInt("selectedBall", index);

        shopBall.sprite = ballSprites[index];
        SetSelected();
    }
}
