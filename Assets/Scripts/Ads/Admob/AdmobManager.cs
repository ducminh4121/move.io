
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class AdmobManager : Singleton<AdmobManager>
{
    [SerializeField] private AppOpenAdManager aoaManager;

    public static bool enableGdpr = true;
    private Action _onInitSdk;
    
    public void Init(Action onInitSdk)
    {
        _onInitSdk = onInitSdk;
        
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        
        var request = new ConsentRequestParameters();
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void InitSdk()
    {
        MobileAds.Initialize(_ => aoaManager.OnInitSuccess());
        aoaManager.Init();
        
        _onInitSdk?.Invoke();
    }
    
    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            Debug.LogError(consentError.Message);
            InitSdk();
            return;
        }
        
        if (enableGdpr)
        {
            ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
            {
                if (formError != null)
                {
                    Debug.LogError(formError.Message);
                    InitSdk();
                    return;
                }

                if (ConsentInformation.CanRequestAds())
                    InitSdk();
                else
                    _onInitSdk?.Invoke();
            });
        }
        else
            InitSdk();
    }
}