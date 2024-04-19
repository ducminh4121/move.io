using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HcGameSetting", menuName = "HyperCat/Game Setting")]
public class HCGameSetting : ScriptableObject
{
    [BoxGroup("Common")]
    [VerticalGroup("Common/V", PaddingBottom = 10)]
    [HorizontalGroup("Common/V/H")]
    [TitleGroup("Identifier", GroupID = "Common/V/H/Identifier")]
    public string gameName = "Prototype";

    [TitleGroup("Identifier", GroupID = "Common/V/H/Identifier")]
    public string packageName = "com.hypercat.prototype";

    [HorizontalGroup("Common/V/H", Width = 80, MarginLeft = 10, MarginRight = 10)]
    [TitleGroup("Icon", GroupID = "Common/V/H/Icon")]
    [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 80)]
    [HideLabel]
    public Texture2D gameIcon;
    
    [HorizontalGroup("A", LabelWidth = 100), PropertySpace(10, 10)]
    public string gameVersion = "1.0";

    [HorizontalGroup("A"), PropertySpace(10, 10)]
    public int bundleVersion = 1;

    [HorizontalGroup("A"), PropertySpace(10, 10)]
    public int buildVersion = 0;

    [PropertySpace(10, 20), Header("Appstore ID (iOS Only)")]
    public string appstoreId;

    [Space]
#if UNITY_ANDROID
    public string adjustAppToken;
    
    [BoxGroup("Ads IDs")]
    public string androidAppKey;
    
    [BoxGroup("Ads IDs")]
    public string admobAndroidId;
    
#elif UNITY_IOS
    
    [BoxGroup("Ads IDs")]
    public string iosAppKey;

    [BoxGroup("Ads IDs")]
    public string admobIosId;

#endif

#if UNITY_EDITOR
    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    [BoxGroup("Ads IDs"), PropertySpace(10, 0), LabelText("Verify mediated network adapters"), GUIColor(0.4f, 0.8f, 1)]
    public static void VerifyRequiredAdapter()
    {
        HCTools.CheckRequiredNetworks();
    }
#endif

    [BoxGroup("Ads IDs"), Space]
    public string aoaId;

#if UNITY_EDITOR
    [Button(ButtonSizes.Large, ButtonStyle.Box)]
    [BoxGroup("Ads IDs"), PropertySpace(10, 0), LabelText("Sync ids from HyperCat server"), GUIColor(0.4f, 0.8f, 1)]
    public static void SyncIdFromHyperCatServer()
    {
        HCTools.GetGameIdFromServer();
    }
#endif

    [Space, Header("Analytic")]
    public string firebaseEventPrefix = "hcPrototype";

#if UNITY_EDITOR
    public UIOrientation deviceOrientation = UIOrientation.Portrait;
    
    [Button]
    public static void SaveGameSetting()
    {
        HCTools.SaveGameSetting();
    }
#endif
}