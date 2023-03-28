using UnityEngine;
using System;
using GoogleMobileAds.Api;


public class AdScript : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI debugger;

    public static AdScript Instance;

    #region Admobs Initialization

    RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
        .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True)
        .SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent.True)
        .SetMaxAdContentRating(MaxAdContentRating.G)
        .build();

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    BannerView bannerView;

    [Header("Above 13 Ad ID's")]
    [SerializeField] string bannerAdID;
    [SerializeField] string interstitialAdID;
    [SerializeField] string rewardedAdID;

    [Header("Below 13 Ad ID's")]
    [SerializeField] string childBannerAdID;
    [SerializeField] string childInterstitialAdID;
    [SerializeField] string childRewardedAdID;

    #endregion

    private void Awake()
    {
        if (Instance)
            DestroyImmediate(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //Admobs
        // TEST ADS (APP ID = ca-app-pub-3940256099942544~3347511713)
        // APP ID ca-app-pub-7413226327717292~5497484040

        if (PlayerPrefs.GetInt("isChild", 0) == 1)
        {
            SetConfiguration();
            return;
        }

        MobileAds.Initialize(initStatus => { });

        // Request Admob Ads
        RequestAds();
    }

    public void SetConfiguration()
    {
        bannerAdID = childBannerAdID;
        interstitialAdID = childInterstitialAdID;
        rewardedAdID = childRewardedAdID;

        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize(initStatus => { });

        // Request Admob Ads
        RequestAds();
    }

    void RequestAds()
    {
        debugger.text = $"Banner ID: {bannerAdID}\nInterstitial ID: {interstitialAdID}\nRewarded ID: {rewardedAdID}";

        RequestAdmobBannerAd();
        RequestAdmobInterstitialAd();
        RequestAdmobRewardedAd();
    }

    #region Admobs
    // Banner Ads
    private void RequestAdmobBannerAd()
    {
        bannerView = new BannerView(bannerAdID, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnAdFailedToLoad += HandleOnBannerFailedToLoad;
        bannerView.OnAdClosed += HandleOnBannerClosed;
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void OpenInspector()
    {
        MobileAds.OpenAdInspector(error => {
            if (error != null)
                debugger.text = $"Code: {error.GetCode()}\n Message: {error.GetMessage()}";
        });
    }

    public void HandleOnBannerClosed(object sender, EventArgs args)
    {
        bannerView.Destroy();
        RequestAdmobBannerAd();
    }

    public void HandleOnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        bannerView.Destroy();
        RequestAdmobBannerAd();
    }

    // Interstitial Ads
    private void RequestAdmobInterstitialAd()
    {
        interstitialAd = new InterstitialAd(interstitialAdID);
        interstitialAd.OnAdClosed += HandleOnInterstitialClosed;
        interstitialAd.OnAdFailedToLoad += HandleOnIntersititalFailedToLoad;
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
    }

    public void ShowAdmobInterstitialAd()
    {
        if (interstitialAd.IsLoaded())
            interstitialAd.Show();
    }

    public void HandleOnInterstitialClosed(object sender, EventArgs args)
    {
        interstitialAd.Destroy();
        RequestAdmobInterstitialAd();
    }

    public void HandleOnIntersititalFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        interstitialAd.Destroy();
        RequestAdmobInterstitialAd();
    }

    // Rewarded Video Ads
    private void RequestAdmobRewardedAd()
    {
        rewardedAd = new RewardedAd(rewardedAdID);
        rewardedAd.OnAdClosed += HandleOnRewardedClosed;
        rewardedAd.OnAdFailedToLoad += OnFailedToLoadRewarded;
        rewardedAd.OnAdFailedToShow += OnFailedToShowRewarded;
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }

    private void ShowAdmobRewardedAd()
    {
        rewardedAd.Show();
    }

    private void HandleOnRewardedClosed(object sender, EventArgs args)
    {
        RequestAdmobRewardedAd();
    }

    private void OnFailedToShowRewarded(object sender, AdErrorEventArgs e)
    {
        RequestAdmobRewardedAd();
    }

    private void OnFailedToLoadRewarded(object sender, AdFailedToLoadEventArgs e)
    {
        RequestAdmobRewardedAd();
    }
    #endregion

    #region AdManager
    //public void ShowRewarded()
    //{
    //    if (rewardedAd.IsLoaded())
    //        ShowAdmobRewardedAd();
    //}

    //public bool IsLoadedRewarded()
    //{
    //    return rewardedAd.IsLoaded();
    //}

    //public void ShowInterstitial()
    //{
    //    if (interstitialAd.IsLoaded())
    //        ShowAdmobInterstitialAd();
    //}

    //public bool IsLoadedInterstitial()
    //{
    //    return interstitialAd.IsLoaded();
    //}
    #endregion
}