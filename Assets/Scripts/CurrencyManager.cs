using UnityEngine;


public class CurrencyManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI mainMenuCurrency;
    [SerializeField] TMPro.TextMeshProUGUI shopCurrency;
    public static CurrencyManager Instance;

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        SetCoins();
    }

    public void SetCoins()
    {
        mainMenuCurrency.text = PlayerPrefs.GetInt("Diamonds", 0).ToString();
        shopCurrency.text = mainMenuCurrency.text;
    }
}
