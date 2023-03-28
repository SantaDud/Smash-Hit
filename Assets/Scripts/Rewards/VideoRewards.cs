using UnityEngine;
using UnityEngine.UI;


public class VideoRewards : MonoBehaviour
{
    [Space]
    [Header("Reward Details")]

    [Space]
    [Header("Reward Popup Details")]
    [SerializeField] GameObject rewardPopup;
    [SerializeField] Sprite vid_1;
    [SerializeField] Sprite vid_2;
    [SerializeField] Sprite vid_3;
    public TMPro.TMP_Text rewAmount;


    private string reward_type;
    private Button watchVideo;
    private Button backToMenuButton;
    private TMPro.TMP_Text rewardAmountText;
    private Image vid_image;
    public GameObject claimedPanel;

    [SerializeField] private int currentCount;

    private bool popUpOpen;

    private void Awake()
    {
        currentCount = PlayerPrefs.GetInt(reward_type, 1);
        switch (currentCount)
        {
            case 1:
                gameObject.GetComponent<Image>().sprite = vid_1;
                break;
            case 2:
                gameObject.GetComponent<Image>().sprite = vid_2;
                break;
            case 3:
                gameObject.GetComponent<Image>().sprite = vid_3;
                break;
        }
        rewAmount.text = (currentCount * 60).ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        reward_type = "Video";
        currentCount = PlayerPrefs.GetInt(reward_type, 1);
        
        if (currentCount >= 4)
            currentCount = 1;
        
        vid_image = rewardPopup.transform.GetChild(1).GetComponent<Image>();
        watchVideo = rewardPopup.transform.GetChild(7).GetComponent<Button>();
        watchVideo.onClick.RemoveAllListeners();
        watchVideo.onClick.AddListener(WatchVideo);
        backToMenuButton = rewardPopup.transform.GetChild(3).GetComponent<Button>();
        backToMenuButton.onClick.AddListener(backToMenu);
        rewardAmountText = rewardPopup.transform.GetChild(4).GetComponent<TMPro.TMP_Text>();
        SetUI();
        popUpOpen = false;
    }
    private void backToMenu()
    {
        rewardPopup.SetActive(false);
        popUpOpen = false;
        SetUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (popUpOpen)
        {
            currentCount = PlayerPrefs.GetInt(reward_type, 1);
            SetUI();
        }
    }

    // function for the main/outer button
    public void OpenVideoReward()
    {
        rewardPopup.SetActive(true);
        popUpOpen = true;
        SetUI();
    }

    private void SetUI()
    {
        switch (currentCount)
        {
            case 1:
                vid_image.sprite = vid_1;
                gameObject.GetComponent<Image>().sprite = vid_1;
                break;
            case 2:
                vid_image.sprite = vid_2;
                gameObject.GetComponent<Image>().sprite = vid_2;
                break;
            case 3:
                vid_image.sprite = vid_3;
                gameObject.GetComponent<Image>().sprite = vid_3;
                break;
        }
        rewardAmountText.text = $"+{currentCount * 60} Diamonds";
        rewAmount.text = (currentCount * 60).ToString();
    }

    // opened by the popup's button
    private void WatchVideo()
    {
        if (FindObjectOfType<Connection>().connected /*&& AdScript.Instance.IsLoadedRewarded()*/)
        {
            rewardPopup.SetActive(false);
            claimedPanel.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = $"+{currentCount * 60} Diamonds";
            claimedPanel.SetActive(true);
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + (currentCount * 60));

            currentCount++;

            if (currentCount >= 4)
                currentCount = 1;

            PlayerPrefs.SetInt(reward_type, currentCount);

            //AdScript.Instance.ShowRewarded();
        }

        else if (!FindObjectOfType<Connection>().connected)
        {
            StopAnimation();
            rewardPopup.transform.GetChild(5).GetComponent<Animator>().SetBool("noInternet", true);
            Invoke("StopAnimation", 1.3f);
        }
        else
        {
            StopAnimation();
            rewardPopup.transform.GetChild(6).GetComponent<Animator>().SetBool("noVideoAd", true);
            Invoke("StopAnimation", 1.3f);
        }
    }

    void StopAnimation()
    {
        rewardPopup.transform.GetChild(5).GetComponent<Animator>().SetBool("noInternet", false);
        rewardPopup.transform.GetChild(6).GetComponent<Animator>().SetBool("noVideoAd", false);
    }
}
