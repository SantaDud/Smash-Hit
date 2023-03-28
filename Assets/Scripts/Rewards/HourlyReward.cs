using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HourlyReward : MonoBehaviour
{
    // An hour delay in milliseconds
    private const float delayInMinutes = 3600000;

    [Space]
    [Header("Reward Details")]
    [SerializeField] int reward_amount;


    [Space]
    [Header("Reward Popup Details")]
    [SerializeField] GameObject rewardPopup;

    // Reward & timing specific code
    private ulong lastRewardOpenedTick;
    private string reward_type;
    private TMPro.TMP_Text waitTimeText;
    private Button backToMenuButton;
    private Button openButton;
    private Button adButton;
    public GameObject claimedPanel;

    // other
    private bool popUpOpen;
    private int rewardAmount;

    void Start()
    {
        popUpOpen = false;
        reward_type = "Hourly";
        waitTimeText = rewardPopup.transform.GetChild(3).GetComponent<TMPro.TMP_Text>();
        backToMenuButton = rewardPopup.transform.GetChild(5).GetComponent<Button>();
        openButton = rewardPopup.transform.GetChild(4).GetComponent<Button>();
        adButton = rewardPopup.transform.GetChild(8).GetComponent<Button>();
        adButton.gameObject.SetActive(false);

        IsRewardAvailable();
    
        openButton.onClick.AddListener(OpenReward);
        backToMenuButton.onClick.AddListener(backToMenu);
    }

    void Update()
    {
        if(popUpOpen)
            SetUI();
    }

    public void OpenHourly()
    {
        SetUI();
        rewardPopup.SetActive(true);
        popUpOpen = true;
    }

    private void SetUI()
    {
        if (!IsRewardAvailable())
        {
            waitTimeText.text = GetTimerText();
            openButton.gameObject.SetActive(false);
            adButton.gameObject.SetActive(true);
            adButton.onClick.RemoveAllListeners();
            adButton.onClick.AddListener(WatchAd);
            rewardAmount = 0;
        }
        else
        {
            openButton.onClick.RemoveAllListeners();
            openButton.onClick.AddListener(OpenReward);
            adButton.gameObject.SetActive(false);
            openButton.gameObject.SetActive(true);
            rewardAmount = reward_amount;
            waitTimeText.text = "";
        }
    }

    private void backToMenu()
    {
        rewardPopup.SetActive(false);
        popUpOpen = false;

    }

    private bool IsRewardAvailable()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(reward_type)))
            return true;
        
        lastRewardOpenedTick = ulong.Parse(PlayerPrefs.GetString(reward_type));

        ulong diff = (ulong)System.DateTime.Now.Ticks - lastRewardOpenedTick;
        ulong m = diff / System.TimeSpan.TicksPerMillisecond;

        float secondsLeft = (delayInMinutes - m) / 1000.0f;

        if (PlayerPrefs.GetInt("adsWatched", 0) == 1)
            secondsLeft = 0;
        
        if (secondsLeft <= 0)
            return true;
        else
            return false;
    }

    private string GetTimerText()
    {
        //lastRewardOpenedTick = ulong.Parse(PlayerPrefs.GetString(reward_type));
        ulong diff = (ulong)System.DateTime.Now.Ticks - lastRewardOpenedTick;
        ulong m = diff / System.TimeSpan.TicksPerMillisecond;
        float secondsLeft = (delayInMinutes - m) / 1000.0f;

        if (PlayerPrefs.GetInt("adsWatched", 0) == 1)
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
        if(rewardAmount != 0)
        {
            rewardPopup.SetActive(false);
            claimedPanel.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = $"+{rewardAmount} Diamonds";
            claimedPanel.SetActive(true);
            PlayerPrefs.SetString(reward_type, System.DateTime.Now.Ticks.ToString());
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + rewardAmount);
            PlayerPrefs.SetInt("adsWatched", 0);
        }
        else
        {
            popUpOpen = false;
            rewardPopup.SetActive(false);
        }
    }

    private void WatchAd()
    {
        if (FindObjectOfType<Connection>().connected /*&& AdScript.Instance.IsLoadedRewarded()*/)
        {
            //AdScript.Instance.ShowRewarded();
            PlayerPrefs.SetInt("adsWatched", 1);
        }
        else if(!FindObjectOfType<Connection>().connected)
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
}
