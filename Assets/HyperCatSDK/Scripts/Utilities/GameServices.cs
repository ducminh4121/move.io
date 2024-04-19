#region

using Firebase;
using Firebase.Analytics;
using System;
using UnityEngine;
using com.adjust.sdk;
using Firebase.Messaging;
using HyperCatSdk;

#endregion

public class GameServices : Singleton<GameServices>
{
#if !PROTOTYPE
    public FirebaseApp firebaseApp;

    private bool _firebaseInited;
    public bool FirebaseInited => _firebaseInited && firebaseApp != null;

    [HideInInspector]
    public string firebaseMesssageToken = "";

    public void Init()
    {
#if FIREBASE
        FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        InitFirebase();
#endif

        InitAdjust();
    }

    private void InitAdjust()
    {
        var adjustConfig = new AdjustConfig(GameManager.Instance.gameSetting.adjustAppToken,
            AdjustEnvironment.Production, false);
        adjustConfig.setLogLevel(AdjustLogLevel.Info);
        adjustConfig.setSendInBackground(true);
        adjustConfig.setEventBufferingEnabled(false);
        adjustConfig.setLaunchDeferredDeeplink(true);
        adjustConfig.setDefaultTracker(string.Empty);
        adjustConfig.setUrlStrategy(AdjustUrlStrategy.Default.ToLowerCaseString());
        adjustConfig.setAppSecret(0, 0, 0, 0, 0);
        adjustConfig.setDelayStart(0);
        adjustConfig.setNeedsCost(false);
        adjustConfig.setPreinstallTrackingEnabled(false);
        adjustConfig.setPreinstallFilePath(string.Empty);
        adjustConfig.setAllowAdServicesInfoReading(true);
        adjustConfig.setAllowIdfaReading(true);
        adjustConfig.setCoppaCompliantEnabled(false);
        adjustConfig.setPlayStoreKidsAppEnabled(false);
        adjustConfig.setLinkMeEnabled(false);
        
        Adjust.start(adjustConfig);
    }

    private void InitFirebase()
    {
        FirebaseApp.CheckDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                SetupFirebase();
            }
            else
            {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        firebaseMesssageToken = e.Token;
    }

    private void SetupFirebase()
    {
        _firebaseInited = true;

        RemoteConfigManager.Instance.StartAsync();

        HCDebug.Log("Firebase Inited Successfully!", HcColor.Aqua);

        AnalyticManager.SetFirebaseUserProperties(UserProperties.lastTimeLogin, DateTime.Now.DayOfYear.ToString());
        AnalyticManager.SetFirebaseUserProperties(UserProperties.gameVersion, GameManager.Instance.gameSetting.gameVersion);
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        
        AnalyticsRevenueAds.Init();
    }
#endif
}