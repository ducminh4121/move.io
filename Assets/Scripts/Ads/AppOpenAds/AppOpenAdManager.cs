#region

using System;
using com.adjust.sdk;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using AdRequest = GoogleMobileAds.Api.AdRequest;

#endregion

public class AppOpenAdManager : Singleton<AppOpenAdManager>
{
    private static AppOpenAdManager _instance;

    public static bool shouldShowOpenAds = true;
    public static bool shouldShowResumeAds = true;
    public static bool isWatchingOtherAds = false;
    public static bool showedFirstOpenAd = false;

    public static int tryGetAoaTime = -1;

    private AppOpenAd _ad;

    private bool _isShowingAoa;
    private DateTime _loadTime;

    private int _firstOpenAdAttemp = 0;

    private bool IsAdAvailable => _ad != null && (DateTime.UtcNow - _loadTime).TotalHours < 4;

    public void Init()
    {
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        UnityEngine.Debug.Log("App State is " + state);
        if (state == AppState.Foreground)
        {
            ShowResumeAppAd();
        }
    }

    public void OnInitSuccess()
    {
        StartRequestAoa();
    }

    public void StartRequestAoa()
    {
        if (!GameManager.EnableAds)
            return;

        var id = GameManager.Instance.gameSetting.aoaId;

        var request = new AdRequest();
        AppOpenAd.Load(id, request, OnLoadAoaResponse);

        Debug.Log($"Request AOA - Id: {id}");
    }

    private void OnLoadAoaResponse(AppOpenAd ad, LoadAdError loadAdError)
    {
        if (loadAdError != null)
        {
            Debug.LogFormat($"Failed to load Aoa. (reason: {loadAdError.GetMessage()})");

            if (!showedFirstOpenAd)
            {
                if (_firstOpenAdAttemp < 3)
                {
                    _firstOpenAdAttemp++;
                    Invoke(nameof(StartRequestAoa), 1f);
                }
                else
                {
                    showedFirstOpenAd = true;
                    if (tryGetAoaTime > 0)
                        Invoke(nameof(StartRequestAoa), tryGetAoaTime);
                }
            }
            else
            {
                if (tryGetAoaTime > 0)
                    Invoke(nameof(StartRequestAoa), tryGetAoaTime);
            }

            return;
        }

        // App open ad is loaded.
        _ad = ad;
        _loadTime = DateTime.UtcNow;

        if (!showedFirstOpenAd)
            ShowOpenAppAd();
    }

    public void ShowOpenAppAd()
    {
        if (!shouldShowOpenAds)
            return;

        ShowAoa();

        showedFirstOpenAd = true;
    }

    public void ShowResumeAppAd()
    {
        if (!shouldShowResumeAds)
            return;

        ShowAoa();
    }

    private bool ShowAoa()
    {
        if (!GameManager.EnableAds)
            return false;

        if (_isShowingAoa)
            return false;

        if (!IsAdAvailable)
        {
            StartRequestAoa();
            return false;
        }

        _ad.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        _ad.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        _ad.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        _ad.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        _ad.OnAdPaid += HandlePaidEvent;
        
        _ad.Show();

        return true;
    }

    private void HandleAdDidDismissFullScreenContent()
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        _isShowingAoa = false;
        StartRequestAoa();
    }

    private void HandleAdFailedToPresentFullScreenContent(AdError adError)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", adError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        StartRequestAoa();
    }

    private void HandleAdDidPresentFullScreenContent()
    {
        Debug.Log("Displayed app open ad");
        _isShowingAoa = true;
    }

    private void HandleAdDidRecordImpression()
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(AdValue adValue)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            adValue.CurrencyCode, adValue.Value);
        
        AnalyticManager.SendAOARevToAdjust(adValue.Value / 1000000f);
    }
}