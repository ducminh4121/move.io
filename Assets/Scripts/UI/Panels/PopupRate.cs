#region

using HyperCatSdk;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class PopupRate : UIPanel
{
    public override UiPanelType GetId()
    {
        return UiPanelType.PopupRate;
    }

    public static void Show()
    {
        var newInstance = (PopupRate) GUIManager.Instance.NewPanel(UiPanelType.PopupRate);
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();
        
        InAppReviewManager.Instance.Request();
    }

    public void GoodRate()
    {
        Database.SaveData();
        AnalyticManager.LogEvent(AnalyticEvent.rateAction, AnalyticParam.value, "Good");
        
        InAppReviewManager.Instance.LaunchReviewFlow();
        
        Close();
    }

    public void BadRate()
    {
        AnalyticManager.LogEvent(AnalyticEvent.rateAction, AnalyticParam.value, "Bad");
        
        PopupNotification.Show(GameConst.FeedbackThanks);
        Close();
    }
    
    public override void Close()
    {
        base.Close();
        
        Gm.data.user.rated = true;
    }
}