using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;

public class FortuneWheelReward : MonoBehaviour
{
    [Space]
    [Header("Picker Wheel")]
    [SerializeField] PickerWheel pickerWheel;

    // A 24h delay in milliseconds
    private const float delayInMinutes = 3600000f * 24f;

    [Space]
    [Header("Reward Details")]

    [Space]
    [Header("Reward Popup Details")]
    [SerializeField] GameObject rewardPopup;
    [SerializeField] Sprite spr_1;
    [SerializeField] Sprite spr_2;

    // Reward & timing specific code
    private ulong lastRewardOpenedTick;
    private string reward_type;
    private TMPro.TMP_Text waitTimeText;
    private Button backToMenuButton;
    private Button spinButton;
    private Button adButton;
    public GameObject claimedPanel;

    // other
    private bool popUpOpen;
    private int rewardAmount;
    private int switcher;

    void Start()
    {
        switcher = 1;
        popUpOpen = false;
        reward_type = "FortuneWheel";
        waitTimeText = rewardPopup.transform.GetChild(3).GetComponent<TMPro.TMP_Text>();
        backToMenuButton = rewardPopup.transform.GetChild(5).GetComponent<Button>();
        spinButton = rewardPopup.transform.GetChild(4).GetComponent<Button>();
        adButton = rewardPopup.transform.GetChild(8).GetComponent<Button>();
        adButton.gameObject.SetActive(false);

        IsRewardAvailable();

        spinButton.onClick.RemoveAllListeners();
        spinButton.onClick.AddListener(SpinWheel);
        backToMenuButton.onClick.AddListener(backToMenu);
    }

    private void backToMenu()
    {
        rewardPopup.SetActive(false);
        popUpOpen = false;

    }

    void Update()
    {
        if (popUpOpen)
            SetUI();
    }

    IEnumerator ChangeSprites()
    {
        yield return new WaitForSeconds(0.3f);
        if(switcher == 1)
        {
            rewardPopup.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = spr_1;
            rewardPopup.transform.GetChild(1).GetChild(0).GetComponent<Image>().SetNativeSize();
            switcher = 2;
        }
        else if (switcher == 2)
        {
            rewardPopup.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = spr_2;
            rewardPopup.transform.GetChild(1).GetChild(0).GetComponent<Image>().SetNativeSize();
            switcher = 1;
        }

        StartCoroutine("ChangeSprites");
    }

    private void SetUI()
    {
        if (!IsRewardAvailable())
        {
            waitTimeText.text = GetTimerText();
            spinButton.gameObject.SetActive(false);
            adButton.gameObject.SetActive(true);
            adButton.onClick.RemoveAllListeners();
            adButton.onClick.AddListener(WatchAd);
            rewardAmount = 0;
        }
        else
        {
            spinButton.gameObject.SetActive(true);
            adButton.gameObject.SetActive(false);
            waitTimeText.text = "";
            spinButton.onClick.RemoveAllListeners();
            spinButton.onClick.AddListener(SpinWheel);
        }
    }

    private bool IsRewardAvailable()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(reward_type)))
            return true;
        lastRewardOpenedTick = ulong.Parse(PlayerPrefs.GetString(reward_type));

        ulong diff = (ulong)System.DateTime.Now.Ticks - lastRewardOpenedTick;
        ulong m = diff / System.TimeSpan.TicksPerMillisecond;

        float secondsLeft = (delayInMinutes - m) / 1000.0f;

        if (PlayerPrefs.GetInt("fortuneAdsWatched", 0) == 1)
            secondsLeft = 0;

        if (secondsLeft <= 0)
            return true;
        else
            return false;
    }

    private string GetTimerText()
    {
        ulong diff = (ulong)System.DateTime.Now.Ticks - lastRewardOpenedTick;
        ulong m = diff / System.TimeSpan.TicksPerMillisecond;
        float secondsLeft = (delayInMinutes - m) / 1000.0f;

        if (PlayerPrefs.GetInt("fortuneAdsWatched", 0) == 1)
            secondsLeft = 0;

        string timerText = "Wait ";
        // Hours
        timerText += $"{((int)secondsLeft / 3600).ToString("00")}h:";
        secondsLeft -= ((int)secondsLeft / 3600) * 3600;
        // Minutes
        timerText += $"{((int)secondsLeft / 60).ToString("00")}m:";
        // Seconds
        timerText += $"{(secondsLeft % 60).ToString("00")}s";
        return timerText;
    }

    private void OpenReward()
    {
        PlayerPrefs.SetString(reward_type, System.DateTime.Now.Ticks.ToString());
        if (rewardAmount != 0)
        {
            rewardPopup.SetActive(false);
            claimedPanel.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = $"+{rewardAmount} Diamonds";
            claimedPanel.SetActive(true);
            PlayerPrefs.SetInt("fortuneAdsWatched", 0);
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + rewardAmount);
            popUpOpen = false;
        }

        else
        {
            popUpOpen = false;
            rewardPopup.SetActive(false);
        }
    }

    private void WatchAd()
    {
        if (FindObjectOfType<Connection>().connected/* && AdScript.Instance.IsLoadedRewarded()*/)
        {
            PlayerPrefs.SetInt("fortuneAdsWatched", 1);
            //AdScript.Instance.ShowRewarded();
        }

        else if (!FindObjectOfType<Connection>().connected)
        {
            StopAnimation();
            rewardPopup.transform.GetChild(6).GetComponent<Animator>().SetBool("noInternet", true);
            Invoke("StopAnimation", 1.3f);
        }
        else
        {
            StopAnimation();
            rewardPopup.transform.GetChild(7).GetComponent<Animator>().SetBool("noVideoAd", true);
            Invoke("StopAnimation", 1.3f);
        }
    }

    void StopAnimation()
    {
        rewardPopup.transform.GetChild(6).GetComponent<Animator>().SetBool("noInternet", false);
        rewardPopup.transform.GetChild(7).GetComponent<Animator>().SetBool("noVideoAd", false);
    }

    private void SpinWheel()
    {
        switcher = 1;
        pickerWheel.Spin();
        spinButton.interactable = false;
        StartCoroutine("ChangeSprites");
        pickerWheel.OnSpinEnd(WheelPiece =>
        {
            spinButton.interactable = true;
            int amount = WheelPiece.Amount;
            rewardAmount = amount;
            OpenReward();
            switcher = 3;
            StopCoroutine("ChangeSprites");
        });
    }

    public void ShowWheelPopup()
    {
        SetUI();
        rewardPopup.SetActive(true);
        popUpOpen = true;
    }
}
