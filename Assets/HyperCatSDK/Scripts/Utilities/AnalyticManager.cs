#region

using System.Collections.Generic;
using com.adjust.sdk;
#if !PROTOTYPE
using Firebase.Analytics;

#endif

#endregion

public class AnalyticManager : Singleton<AnalyticManager>
{
#if !PROTOTYPE

    #region Setup

    private static string GetCorrectEventName(string name)
    {
        return string.Format("{0}_{1}", GameManager.Instance.gameSetting.firebaseEventPrefix, name);
    }

    public static void SetFirebaseUserProperties(string eventName, string property)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase Set User Property: " + eventName + ", value: " + property, HcColor.Yellow);
            FirebaseAnalytics.SetUserProperty(GetCorrectEventName(eventName), property);
        }
#endif
    }

    public static void LogEvent(string eventName, params Parameter[] parameters)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName), parameters);
        }
#endif
    }

    public static void LogEvent(string eventName, string paramName, string paramValue)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName), paramName, paramValue);
        }
#endif
    }

    public static void LogEvent(string eventName)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName));
        }
#endif
    }

    #endregion

    #region Adjust

    public static void SendISImpressionDataToAdjust(IronSourceImpressionData impressionData)
    {
        var adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
        adjustAdRevenue.setRevenue(impressionData.revenue.Value, "USD");
        // Optional fields
        adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
        adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
        adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
        // Track Adjust ad revenue
        Adjust.trackAdRevenue(adjustAdRevenue);
    }

    public static void SendAOARevToAdjust(double rev)
    {
        var adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adjustAdRevenue.setRevenue(rev, "USD");
        // Optional fields
        adjustAdRevenue.setAdRevenueNetwork("appopenads");
        //adjustAdRevenue.setAdRevenueUnit(data.adUnit);
        //adjustAdRevenue.setAdRevenuePlacement(data.placement);
        // Track Adjust ad revenue
        Adjust.trackAdRevenue(adjustAdRevenue);
    }

    public static void SendIAPRevToAdjust(double rev, string currency, string transactionId)
    {
        var adjustEvent = new AdjustEvent("reviap");
        adjustEvent.setRevenue(rev, currency);
        adjustEvent.setTransactionId(transactionId);
        Adjust.trackEvent(adjustEvent);
    }

    #endregion
    
#endif
}