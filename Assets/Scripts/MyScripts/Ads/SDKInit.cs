/****************************************************
 * FileName:		SDKInit.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-16-10:17:44
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UniRx;
using UnityEngine.UI;


public enum RewardType
{
    Null,
    addMoney,
    AddCoin,
    Revive,
    Function
}
public class SDKInit : Singleton<SDKInit>
{

#if UNITY_ANDROID
    private const string rewardedAdUnitId = "de8592d7c7657031";
    private const string interstitialAdUnitId = "4909b60fb06bd278";
    private const string BannerID = "e34ac5ce3917821c";

    private const string rewardedAdUnitIdC = "de8592d7c7657031";
    private const string interstitialAdUnitIdC = "4909b60fb06bd278";
    private const string BannerIDC = "e34ac5ce3917821c";
#endif
#if UNITY_IOS
    private const string rewardedAdUnitId = "ba2049dcf4c8f990";
    private const string interstitialAdUnitId = "a801dd24153e97e0";
    private const string BannerID = "2ff736c2abd8c8d9";

    private const string rewardedAdUnitIdC = "ca4cc556a1630471";
    private const string interstitialAdUnitIdC = "d9e7b32b16f1afca";
    private const string BannerIDC = "74d176495b05425c";
#endif

    #region --- Private Variable ---
    private const string alstr = "pIZT0gTB19HmoBhtMBRD2fXDkHJC9HryZOUa4yn552suDTlakAomrJzZbGmlTbg6_Ah46SACU05iTqHG_40rN-";

    private const string AppToken = "stvyc0gw1vy8";

#endregion

#region FaceBook

    public static void FaceBookInit()
    {
        FB.Init(() =>
        {
	   FB.ActivateApp();
            Debug.Log("FB OnInitComplete!");
            Debug.Log("FB.AppId: " + FB.AppId);
            Debug.Log("FB.GraphApiVersion: " + FB.GraphApiVersion);
        }, (isUnityShutDown) =>
        {
            Debug.Log("FB OnHideUnity： " + isUnityShutDown);
        });
    }

#endregion

    private static ReactiveProperty<bool> isgdpr;
    public static ReactiveProperty<bool> isGDPR
    {
        get
        {
            if (isgdpr == null)
                isgdpr = new ReactiveProperty<bool> { Value = false };
            return isgdpr;
        }
    }

    private void Awake()
    {
        FaceBookInit();
        if (Screen.height > Screen.width * 2)
        {
            var sd = bannerBG.rectTransform.sizeDelta;
            sd.y = 250;
            bannerBG.rectTransform.sizeDelta = sd;
        }
        MaxSdkCallbacks.OnBannerAdLoadedEvent += (a) => { Debug.Log("Banner加载完毕" + a); };
        MaxSdkCallbacks.OnBannerAdLoadFailedEvent += (a, b) => { Debug.LogError("Banner加载失败" + a + b); };
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            Debug.Log("AppLovin SDK 初始化完成");
            OnMaxSdkInitizalized();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            isGDPR.Value = sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies;
            SceneSettingUI.OpenDebug.Subscribe(a=> { if (a) MaxSdk.ShowMediationDebugger(); } );
        };

        MaxSdk.SetSdkKey(alstr);
        MaxSdk.InitializeSdk();
        MaxSdk.CreateBanner( EnterScene.IsChinese.Value ? BannerIDC : BannerID, MaxSdkBase.BannerPosition.BottomCenter);
        IAPInit();
        FireBaseInit();
    }

#region IAP



    //获取价格  IAPManager.Instance.GetLocalizedPriceString(consumableProducts[indexNumberConsumable].name)
    //初始化 IAPManager.Instance.InitializeIAPManager(InitializeResult);
    //购买+回调  IAPManager.Instance.BuyProduct(consumableProducts[indexNumberConsumable].name, ProductBought);
    public class MyStoreProducts
    {
        public ShopProductNames name;
        public bool bought;

        public MyStoreProducts(ShopProductNames name, bool bought)
        {
            this.name = name;
            this.bought = bought;
        }
    }

    private static IntProperty noADS;
    public static IntProperty NOADS
    {
        get
        {
            if (noADS == null)
                noADS = new IntProperty("NotADS",0);
            return noADS;
        }
    }

    private bool IAPIsInit = false;
    private List<MyStoreProducts> Products;
    private void IAPInit()
    {
        IAPManager.Instance.InitializeIAPManager(InitializeResult);
    }

    /// <summary>
    /// 内购购买产品
    /// </summary>
    /// <param name="shop"></param>
    public void BuyProduct(ShopProductNames shop)
    {
        if (!IAPManager.Instance.IsInitialized())
        {
            Debug.LogError("IPA没有初始化");
            return;
        }
        IAPManager.Instance.BuyProduct(shop, ProductBought);
    }

    private void InitializeResult(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
    {
        
        IAPIsInit = true;
        Products = new List<MyStoreProducts>();

        if (status == IAPOperationStatus.Success)
        {
            Debug.Log("IAP初始化完成"+ message);
            //IAP was successfully initialized
            //loop through all products and check which one are bought to update our variables
            for (int i = 0; i < shopProducts.Count; i++)
            {
                if (shopProducts[i].productName == "UnlockLevel1")
                {
                    //if a product is active, means that user had already bought that product so enable access
                    if (shopProducts[i].active)
                    {
                        //unlockLevel1 = true;
                    }
                }

                //construct a different list of each category of products, for an easy display purpose, not required
                switch (shopProducts[i].productType)
                {
                    case ProductType.Consumable:
                        Products.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                }

                

            }
        }
        else
        {
            Debug.Log("IAP初始化失败"+ message);
        }
    }


    /// <summary>
    ///购买一个产品后自动调用
    /// </summary>
    /// <param name="status">The purchase status: Success/Failed</param>
    /// <param name="message">Error message if status is failed</param>
    /// <param name="product">the product that was bought, use the values from shop product to update your game data</param>
    private void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            if (product.productType == ProductType.Consumable)
            {
                var value = product.value;
                if (PlayerPrefs.GetInt("IsBuyKey" + value.ToString()) == 0)
                    value += value;
                GameSetting.Coin.Value += value;
                Messenger.Broadcast<int>(ConstValue.CallBackFun.BuyCoin, product.value);
                NOADS.Value = 1;
            }

        }
        else
        {
            Debug.Log("购买产品失败: " + message);
        }
    }
    #endregion

    #region FireBaseInit

    private void FireBaseInit()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                // Firebase Unity SDK is not safe to use here.
            }
        });
#if !UNITY_EDITOR
        Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
#endif
    }

    #endregion

    #region ABTest

    public static float DifficultyAB = 0f;              //难度
    public static float GuideAB = 0f;                  //引导
    public static float KillerEffectAB = 0f;        //杀手黑夜效果
    public static string GroupStr = "not init";

    void OnMaxSdkInitizalized()
    {
        MaxSdk.VariableService.LoadVariables();
        Mapvalue("Difficulty_adjusted", ref DifficultyAB);
        Mapvalue("upgrade_guide", ref GuideAB);
        Mapvalue("night_mode", ref KillerEffectAB);
        GroupStr = MaxSdk.VariableService.GetString("experiment_group");

        if (GroupStr == "")
            Debug.LogError("获取experiment_group失败");

#if UNITY_ANDROID
        int index = Mathf.RoundToInt(DifficultyAB);
        if (Mathf.Approximately(GuideAB, 1))
            index = 3;
        for (int i = 0; i < SceneSettingUI.toggles.Length; i++)
        {
            SceneSettingUI.toggles[i].isOn = (i == index);
            var t = i;

            if (i < 3)
                SceneSettingUI.toggles[i].onValueChanged.AddListener((a) => { if (a) { DifficultyAB = t; GuideAB = 0; } });
            else
                SceneSettingUI.toggles[i].onValueChanged.AddListener((a) => { if (a) { DifficultyAB = 0; GuideAB = 1; } });
        }
#endif
#if UNITY_IOS
        int index = Mathf.RoundToInt(KillerEffectAB);
        SceneSettingUI.toggles[index].isOn = true;
        SceneSettingUI.toggles[0].onValueChanged.AddListener((a) => { if (a) { KillerEffectAB = 0; } });
        SceneSettingUI.toggles[1].onValueChanged.AddListener((a) => { if (a) { KillerEffectAB = 1; } });
        SceneSettingUI.toggles[2].gameObject.SetActive(false);
        SceneSettingUI.toggles[3].gameObject.SetActive(false);
#endif
    }

    void Mapvalue(string sourceString, ref float targetVariable)
    {
        string str = MaxSdk.VariableService.GetString(sourceString, "");
        if (!string.IsNullOrEmpty(str))
        {
            float.TryParse(str, out targetVariable);
            PlayerPrefs.SetFloat("max" + sourceString, targetVariable);
        }
        else
        {
            Debug.LogError("获取" + sourceString + "失败");
            targetVariable = PlayerPrefs.GetFloat("max" + sourceString, targetVariable);
        }
    }
    void Mapvalue(string sourceString, ref int targetVariable)
    {
        string str = MaxSdk.VariableService.GetString(sourceString, "");
        if (!string.IsNullOrEmpty(str))
        {
            int.TryParse(str, out targetVariable);
            PlayerPrefs.SetInt("max" + sourceString, targetVariable);
        }
        else
        {
            Debug.LogError("获取" + sourceString + "失败");
            targetVariable = PlayerPrefs.GetInt("max" + sourceString, targetVariable);
        }
    }

    #endregion

    public Image bannerBG;

    public void ShowBanner()
    {
        bannerBG.enabled = false;
        if (!MaxSdk.IsInitialized() || IsDebug || NOADS.Value == 1)
            return;
        MaxSdk.ShowBanner(EnterScene.IsChinese.Value ? BannerIDC : BannerID);
    }
    public void HideBanner()
    {
        bannerBG.enabled = false;
        if (!MaxSdk.IsInitialized())
            return;
        MaxSdk.HideBanner(EnterScene.IsChinese.Value ? BannerIDC : BannerID);
    }

#region 激励视频


    int retryAttempt2;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd( EnterScene.IsChinese.Value ? rewardedAdUnitIdC : rewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId)
    {
        Debug.Log("加载完成激励广告" );
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        // Reset retry attempt
    }

    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
        Debug.LogError("加载激励广告失败" + errorCode);
        retryAttempt2++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt2));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.LogError("播放激励广告失败"+errorCode);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId) {
        retryAttempt2 = 0;
        var click = new Dictionary<string, object>
        {
            ["level"] = LevelSetting.Value,
            [sendFBMessageStr] = sendFBMessageStr,
        };
        FB.LogAppEvent("rv_start", 2, click);
    }

    private void OnRewardedAdClickedEvent(string adUnitId) { }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        Debug.Log("关闭激励广告");
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
    {
        GetRewardFunction(rewardType);
        Debug.Log("获得奖励");
        var click = new Dictionary<string, object>
        {
            ["level"] = LevelSetting.Value,
            [sendFBMessageStr] = sendFBMessageStr,
        };
        FB.LogAppEvent("rv_complete", 2, click);
    }

    

    public static RewardType rewardType = RewardType.Null;
    public static int value = 0;
    public delegate void RewardCallback() ;
    public static RewardCallback rewardCallback;

    public void GetRewardFunction(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Null:
                break;
            case RewardType.addMoney:
                GameSetting.Money.Value += value;
                break;
            case RewardType.AddCoin:
                GameSetting.Coin.Value += value;
                break;
            case RewardType.Revive:
                GameManager.isWin = GameManager.isDead = false;
                UIPanelManager.Instance.PopPanel();
                PlayerControl.Instance.PlayerRebone();
                Messenger.Broadcast<HumanBase, HumanBase> (ConstValue.CallBackFun.PlayerRebone, PlayerControl.Instance, null);
                GameManager.Instance.GameContinue();
                GameManager.Instance.XuanYun(ZombieShowTimer.ZombiePlayer, 2f);
                ZombieShowTimer.Instance.GameTimer.timer = Mathf.Max(ZombieShowTimer.Instance.GameTimer.timer - 20, 0);
                GameUI.Data.SetActive(true);
                break;
            case RewardType.Function:
                rewardCallback?.Invoke();
                break;
        }
        InterstitialCD.ReStart();   
    }

    public static bool IsDebug = false;

    public bool RewardedAdsIsReady()
    {
        if (IsDebug)
            return true;
        return MaxSdk.IsRewardedAdReady(EnterScene.IsChinese.Value ? rewardedAdUnitIdC : rewardedAdUnitId);
    }

    private string sendFBMessageStr;
    /// <summary>
    /// 显示激励视频
    /// </summary>
    /// <param name="msg"></param>
    public void ShowRewardedAds(string msg)
    {
        if (IsDebug)
        {
            GetRewardFunction(rewardType);
            return;
        }

        sendFBMessageStr = msg;

        var parameters = new Dictionary<string, object>
        {
            ["RewardedAd"] = msg,
        };
        FB.LogAppEvent("RewardedAd", 1, parameters);

        var click = new Dictionary<string, object>
        {
            ["level"] = LevelSetting.Value,
            [msg] = msg,
        };
        FB.LogAppEvent("rv_click", 2, click);

        if (MaxSdk.IsRewardedAdReady(EnterScene.IsChinese.Value ? rewardedAdUnitIdC : rewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(EnterScene.IsChinese.Value ? rewardedAdUnitIdC : rewardedAdUnitId);
        }
    }

    #endregion

    #region 插页广告



    int retryAttempt;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;


        Observable.EveryUpdate().Subscribe(_=> InterstitialCD.OnUpdate(Time.deltaTime));
        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial( EnterScene.IsChinese.Value ? interstitialAdUnitIdC : interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        Debug.Log("加载成功插页广告");
        retryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
    {
        // Interstitial ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
        Debug.LogError("加载插页广告失败"+ errorCode);
        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        Debug.LogError("播放插页广告失败"+ errorCode);
        // Interstitial ad failed to display. We recommend loading the next ad
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        Debug.Log("关闭插页广告");
        // Interstitial ad is hidden. Pre-load the next ad
        LoadInterstitial();
    }


    private MyTimer InterstitialCD = new MyTimer(30);

    /// <summary>
    /// 显示插页广告
    /// </summary>
    public void ShowInterstitial(string msg)
    {
        if (NOADS.Value == 1)
        {
            Debug.Log("已去广告");
            return;
        }
        if (IsDebug)
        {
            Debug.Log("Debug模式");
            return;
        }
        if (!InterstitialCD.IsFinish)
        {
            Debug.Log("插页广告CD");
            return;
        }
        if (MaxSdk.IsInterstitialReady(EnterScene.IsChinese.Value ? interstitialAdUnitIdC : interstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(EnterScene.IsChinese.Value ? interstitialAdUnitIdC : interstitialAdUnitId);
            var parameters = new Dictionary<string, object>
            {
                ["InterstitialAd"] = msg,
            };
            FB.LogAppEvent("InterstitialAd", 1, parameters);
            InterstitialCD.ReStart();
        }
        else
        {
            Debug.Log("插页广告未加载好");
        }
    }

    #endregion

    /**
     * Include the Facebook namespace via the following code:
     * using Facebook.Unity;
     * For more details, please take a look at:
     * developers.facebook.com/docs/unity/reference/current/FB.LogAppEvent
     */
    public static void LogAchievedLevelEvent(string level)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                [AppEventParameterName.Level] = level,
                ["experiment_group"] = GroupStr,
            };
            FB.LogAppEvent(AppEventName.AchievedLevel, null, parameters);
        }
        catch (System.Exception)
        {

        }
    }

    private void OnApplicationFocus(bool focus)
    {
        Time.timeScale = focus ? 1 : 0;
    }



}
