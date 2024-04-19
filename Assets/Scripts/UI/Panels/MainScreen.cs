#region

using TMPro;
using UnityEngine;

#endregion

public class MainScreen : UIPanel
{
    public static MainScreen Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.MainScreen;
    }

    public static void Show()
    {
        var newInstance = (MainScreen) GUIManager.Instance.NewPanel(UiPanelType.MainScreen);
        Instance = newInstance;
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();

        Init();
    }

    private void Init()
    {

    }

    public void StartGame()
    {
        AudioAssistant.Shot(TypeSound.Button);

        if (!GameManager.NetworkAvailable)
        {
            PopupNoInternet.Show();
            return;
        }

        PlayScreen.Show();
    }

    public void OnBuyNoAds()
    {
        AudioAssistant.Shot(TypeSound.Button);
        IAPManager.Instance.BuyProduct(IapProductName.NoAds);
    }

    protected override void RegisterEvent()
    {

    }

    protected override void UnregisterEvent()
    {

    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}