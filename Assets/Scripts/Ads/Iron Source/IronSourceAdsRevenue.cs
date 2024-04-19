using System.Collections.Generic;
using com.adjust.sdk;
using HyperCatSdk;

public partial class IronSourceManager
{
    private void RegisterRevenuePaidCallback()
    {
        IronSourceEvents.onImpressionDataReadyEvent += OnImpressionDataReady;
    }

    private void OnImpressionDataReady(IronSourceImpressionData impressionData)
    {
        if (impressionData != null && !string.IsNullOrEmpty(impressionData.adNetwork) && impressionData.revenue != null)
        {
            AnalyticsRevenueAds.SendEvent(impressionData);

            AnalyticManager.SendISImpressionDataToAdjust(impressionData);
        }
    }
}