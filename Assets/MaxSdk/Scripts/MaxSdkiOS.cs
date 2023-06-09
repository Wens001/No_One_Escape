using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// iOS AppLovin MAX Unity Plugin implementation
/// </summary>
public class MaxSdkiOS : MaxSdkBase
{
    static MaxSdkiOS()
    {
        InitCallbacks();
    }

#if UNITY_IOS
    public static MaxVariableServiceiOS VariableService
    {
        get { return MaxVariableServiceiOS.Instance; }
    }

    #region Initialization

    [DllImport("__Internal")]
    private static extern void _MaxSetSdkKey(string sdkKey);

    /// <summary>
    /// Set AppLovin SDK Key.
    ///
    /// This method must be called before any other SDK operation
    /// </summary>
    public static void SetSdkKey(string sdkKey)
    {
        _MaxSetSdkKey(sdkKey);
    }

    [DllImport("__Internal")]
    private static extern void _MaxInitializeSdk(string serializedAdUnitIds, string serializedMetaData);
    
    /// <summary>
    /// Initialize the default instance of AppLovin SDK.
    ///
    /// Please make sure that application's Android manifest or Info.plist includes the AppLovin SDK key.
    /// <param name="adUnitIds">
    /// OPTIONAL: Set the MAX ad unit ids to be used for this instance of the SDK. 3rd-party SDKs will be initialized with the credentials configured for these ad unit ids.
    /// This should only be used if you have different sets of ad unit ids / credentials for the same package name.</param>
    /// </summary>
    public static void InitializeSdk(String[] adUnitIds=null)
    {
        String serializedAdUnitIds = (adUnitIds != null) ? String.Join(",", adUnitIds) : "";
        _MaxInitializeSdk(serializedAdUnitIds, GenerateMetaData());
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsInitialized();

    /// <summary>
    /// Check if the SDK has been initialized.
    /// </summary>
    /// <returns>True if SDK has been initialized</returns>
    public static bool IsInitialized()
    {
        return _MaxIsInitialized();
    }

    #endregion

    #region User Identifier

    [DllImport("__Internal")]
    private static extern void _MaxSetUserId(string userId);

    /// <summary>
    /// Set an identifier for the current user. This identifier will be tied to SDK events and our optional S2S postbacks.
    /// 
    /// If you're using reward validation, you can optionally set an identifier to be included with currency validation postbacks.
    /// For example, a username or email. We'll include this in the postback when we ping your currency endpoint from our server.
    /// </summary>
    /// 
    /// <param name="userId">The user identifier to be set.</param>
    public static void SetUserId(string userId)
    {
        _MaxSetUserId(userId);
    }

    #endregion

    #region Mediation Debugger

    [DllImport("__Internal")]
    private static extern void _MaxShowMediationDebugger();

    /// <summary>
    /// Present the mediation debugger UI.
    /// This debugger tool provides the status of your integration for each third-party ad network.
    ///
    /// Please call this method after the SDK has initialized.
    /// </summary>
    public static void ShowMediationDebugger()
    {
        _MaxShowMediationDebugger();
    }

    [DllImport("__Internal")]
    private static extern string _MaxGetAdInfo(string adUnitIdentifier);

    /// <summary>
    /// Returns information about the last loaded ad for the given ad unit identifier. Returns null if no ad is loaded.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of an ad</param>
    /// <returns>Information about the ad, or null if no ad is loaded.</returns>
    public static MaxSdkBase.AdInfo GetAdInfo(string adUnitIdentifier)
    {
        string adInfoString = _MaxGetAdInfo(adUnitIdentifier);

        if (string.IsNullOrEmpty(adInfoString)) return null;

        return new MaxSdkBase.AdInfo(adInfoString);
    }

    #endregion

    #region Privacy

    [DllImport("__Internal")]
    private static extern int _MaxConsentDialogState();

    /// <summary>
    /// Get the consent dialog state for this user. If no such determination could be made, {@link ConsentDialogState#Unknown} will be returned.
    ///
    /// Note: this method should be called only after SDK has been initialized
    /// </summary>
    public static ConsentDialogState GetConsentDialogState()
    {
        if (!IsInitialized())
        {
            Debug.LogWarning(
                "[AppLovin MAX] MAX Ads SDK has not been initialized yet. GetConsentDialogState() may return ConsentDialogState.Unknown");
        }

        return (ConsentDialogState) _MaxConsentDialogState();
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetHasUserConsent(bool hasUserConsent);

    /// <summary>
    /// Set whether or not user has provided consent for information sharing with AppLovin and other providers.
    /// </summary>
    /// <param name="hasUserConsent">'true' if the user has provided consent for information sharing with AppLovin. 'false' by default.</param>
    public static void SetHasUserConsent(bool hasUserConsent)
    {
        _MaxSetHasUserConsent(hasUserConsent);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxHasUserConsent();

    /// <summary>
    /// Check if user has provided consent for information sharing with AppLovin and other providers.
    /// </summary>
    public static bool HasUserConsent()
    {
        return _MaxHasUserConsent();
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetIsAgeRestrictedUser(bool isAgeRestrictedUser);

    /// <summary>
    /// Mark user as age restricted (i.e. under 16).
    /// </summary>
    /// <param name="isAgeRestrictedUser">'true' if the user is age restricted (i.e. under 16).</param>
    public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
    {
        _MaxSetIsAgeRestrictedUser(isAgeRestrictedUser);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsAgeRestrictedUser();

    /// <summary>
    /// Check if user is age restricted.
    /// </summary>
    public static bool IsAgeRestrictedUser()
    {
        return _MaxIsAgeRestrictedUser();
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetDoNotSell(bool doNotSell);

    /// <summary>
    /// Set whether or not user has opted out of the sale of their personal information.
    /// </summary>
    /// <param name="doNotSell">'true' if the user has opted out of the sale of their personal information.</param>
    public static void SetDoNotSell(bool doNotSell)
    {
        _MaxSetDoNotSell(doNotSell);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsDoNotSell();

    /// <summary>
    /// Check if the user has opted out of the sale of their personal information.
    /// </summary>
    public static bool IsDoNotSell()
    {
        return _MaxIsDoNotSell();
    }

    #endregion

    #region Banners

    [DllImport("__Internal")]
    private static extern void _MaxCreateBanner(string adUnitIdentifier, string bannerPosition);

    /// <summary>
    /// Create a new banner.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to create</param>
    /// <param name="bannerPosition">Banner position</param>
    public static void CreateBanner(string adUnitIdentifier, BannerPosition bannerPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "create banner");
        _MaxCreateBanner(adUnitIdentifier, bannerPosition.ToString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerPlacement(string adUnitIdentifier, string placement);

    /// <summary>
    /// Set the banner placement for an ad unit identifier to tie the future ad events to.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set the placement for</param>
    /// <param name="placement">Placement to set</param>
    public static void SetBannerPlacement(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner placement");
        _MaxSetBannerPlacement(adUnitIdentifier, placement);
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateBannerPosition(string adUnitIdentifier, string bannerPosition);

    /// <summary>
    /// Updates the position of the banner to the new position provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the banner for which to update the position</param>
    /// <param name="bannerPosition">A new position for the banner</param>
    public static void UpdateBannerPosition(string adUnitIdentifier, BannerPosition bannerPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update banner position");
        _MaxUpdateBannerPosition(adUnitIdentifier, bannerPosition.ToString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowBanner(string adUnitIdentifier);

    /// <summary>
    /// Show banner at a position determined by the 'CreateBanner' call.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to show</param>
    public static void ShowBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show banner");
        _MaxShowBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxDestroyBanner(string adUnitIdentifier);

    /// <summary>
    /// Remove banner from the ad view and destroy it.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to destroy</param>
    public static void DestroyBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "destroy banner");
        _MaxDestroyBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxHideBanner(string adUnitIdentifier);

    /// <summary>
    /// Hide banner.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to hide</param>
    public static void HideBanner(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "hide banner");
        _MaxHideBanner(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerBackgroundColor(string adUnitIdentifier, string hexColorCodeString);

    /// <summary>
    /// Set non-transparent background color for banners to be fully functional.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set background color for</param>
    /// <param name="color">A background color to set for the ad</param>
    public static void SetBannerBackgroundColor(string adUnitIdentifier, Color color)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set background color");
        _MaxSetBannerBackgroundColor(adUnitIdentifier, MaxSdkUtils.ParseColor(color));
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetBannerExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the banner to set the extra parameter for.</param>
    /// <param name="key">The key for the extra parameter.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetBannerExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set banner extra parameter");
        _MaxSetBannerExtraParameter(adUnitIdentifier, key, value);
    }

    #endregion

    #region MRECs

    [DllImport("__Internal")]
    private static extern void _MaxCreateMRec(string adUnitIdentifier, string mrecPosition);

    /// <summary>
    /// Create a new MREC.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to create</param>
    /// <param name="mrecPosition">MREC position</param>
    public static void CreateMRec(string adUnitIdentifier, AdViewPosition mrecPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "create MREC");
        _MaxCreateMRec(adUnitIdentifier, mrecPosition.ToString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetMRecPlacement(string adUnitIdentifier, string placement);

    /// <summary>
    /// Set the MREC placement for an ad unit identifier to tie the future ad events to.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to set the placement for</param>
    /// <param name="placement">Placement to set</param>
    public static void SetMRecPlacement(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set MREC placement");
        _MaxSetMRecPlacement(adUnitIdentifier, placement);
    }

    [DllImport("__Internal")]
    private static extern void _MaxUpdateMRecPosition(string adUnitIdentifier, string mrecPosition);

    /// <summary>
    /// Updates the position of the MREC to the new position provided.
    /// </summary>
    /// <param name="adUnitIdentifier">The ad unit identifier of the MREC for which to update the position</param>
    /// <param name="mrecPosition">A new position for the MREC</param>
    public static void UpdateMRecPosition(string adUnitIdentifier, AdViewPosition mrecPosition)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "update MREC position");
        _MaxUpdateMRecPosition(adUnitIdentifier, mrecPosition.ToString());
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowMRec(string adUnitIdentifier);

    /// <summary>
    /// Show MREC at a position determined by the 'CreateMRec' call.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to show</param>
    public static void ShowMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show MREC");
        _MaxShowMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxDestroyMRec(string adUnitIdentifier);

    /// <summary>
    /// Remove MREC from the ad view and destroy it.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to destroy</param>
    public static void DestroyMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "destroy MREC");
        _MaxDestroyMRec(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxHideMRec(string adUnitIdentifier);

    /// <summary>
    /// Hide MREC.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the MREC to hide</param>
    public static void HideMRec(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "hide MREC");
        _MaxHideMRec(adUnitIdentifier);
    }

    #endregion

    #region Interstitials

    [DllImport("__Internal")]
    private static extern void _MaxLoadInterstitial(string adUnitIdentifier);

    /// <summary>
    /// Start loading an interstitial.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load</param>
    public static void LoadInterstitial(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load interstitial");
        _MaxLoadInterstitial(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsInterstitialReady(string adUnitIdentifier);

    /// <summary>
    /// Check if interstitial ad is loaded and ready to be displayed.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load</param>
    /// <returns>True if the ad is ready to be displayed</returns>
    public static bool IsInterstitialReady(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "check interstitial loaded");
        return _MaxIsInterstitialReady(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowInterstitial(string adUnitIdentifier, string placement);

    /// <summary>
    /// Present loaded interstitial. Note: if the interstitial is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load</param>
    public static void ShowInterstitial(string adUnitIdentifier)
    {
        ShowInterstitial(adUnitIdentifier, null);
    }

    /// <summary>
    /// Present loaded interstitial for a given placement to tie ad events to. Note: if the interstitial is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load</param>
    /// <param name="placement">The placement to tie the showing ad's events to</param>
    public static void ShowInterstitial(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show interstitial");

        if (IsInterstitialReady(adUnitIdentifier))
        {
            _MaxShowInterstitial(adUnitIdentifier, placement);
        }
        else
        {
            Debug.LogWarning("[AppLovin MAX] Not showing MAX Ads interstitial: ad not ready");
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetInterstitialExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to set the extra parameter for.</param>
    /// <param name="key">The key for the extra parameter.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetInterstitialExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set interstitial extra parameter");
        _MaxSetInterstitialExtraParameter(adUnitIdentifier, key, value);
    }

    #endregion

    #region Rewarded

    [DllImport("__Internal")]
    private static extern void _MaxLoadRewardedAd(string adUnitIdentifier);

    /// <summary>
    /// Start loading an rewarded ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to load</param>
    public static void LoadRewardedAd(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "load rewarded ad");
        _MaxLoadRewardedAd(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsRewardedAdReady(string adUnitIdentifier);

    /// <summary>
    /// Check if rewarded ad ad is loaded and ready to be displayed.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to load</param>
    /// <returns>True if the ad is ready to be displayed</returns>
    public static bool IsRewardedAdReady(string adUnitIdentifier)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "check rewarded ad loaded");
        return _MaxIsRewardedAdReady(adUnitIdentifier);
    }

    [DllImport("__Internal")]
    private static extern void _MaxShowRewardedAd(string adUnitIdentifier, string placement);


    /// <summary>
    /// Present loaded rewarded ad. Note: if the rewarded ad is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to show</param>
    public static void ShowRewardedAd(string adUnitIdentifier)
    {
        ShowRewardedAd(adUnitIdentifier, null);
    }

    /// <summary>
    /// Present loaded rewarded ad for a given placement to tie ad events to. Note: if the rewarded ad is not ready to be displayed nothing will happen.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the interstitial to load</param>
    /// <param name="placement">The placement to tie the showing ad's events to</param>
    public static void ShowRewardedAd(string adUnitIdentifier, string placement)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "show rewarded ad");

        if (IsRewardedAdReady(adUnitIdentifier))
        {
            _MaxShowRewardedAd(adUnitIdentifier, placement);
        }
        else
        {
            Debug.LogWarning("[AppLovin MAX] Not showing MAX Ads rewarded ad: ad not ready");
        }
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetRewardedAdExtraParameter(string adUnitIdentifier, string key, string value);

    /// <summary>
    /// Set an extra parameter for the ad.
    /// </summary>
    /// <param name="adUnitIdentifier">Ad unit identifier of the rewarded ad to set the extra parameter for.</param>
    /// <param name="key">The key for the extra parameter.</param>
    /// <param name="value">The value for the extra parameter.</param>
    public static void SetRewardedAdExtraParameter(string adUnitIdentifier, string key, string value)
    {
        ValidateAdUnitIdentifier(adUnitIdentifier, "set rewarded extra parameter");
        _MaxSetRewardedAdExtraParameter(adUnitIdentifier, key, value);
    }

    #endregion

    #region Event Tracking

    [DllImport("__Internal")]
    private static extern void _MaxTrackEvent(string name, string parameters);

    /// <summary>
    /// Track an event using AppLovin.
    /// </summary>
    /// <param name="name">An event from the list of pre-defined events may be found in MaxEvents.cs as part of the AppLovin SDK framework.</param>
    /// <param name="parameters">A dictionary containing key-value pairs further describing this event.</param>
    public static void TrackEvent(string name, IDictionary<string, string> parameters = null)
    {
        _MaxTrackEvent(name, MaxSdkUtils.DictToPropsString(parameters));
    }

    #endregion

    #region Settings

    [DllImport("__Internal")]
    private static extern void _MaxSetMuted(bool muted);

    /// <summary>
    /// Set whether to begin video ads in a muted state or not.
    ///
    /// Please call this method after the SDK has initialized.
    /// </summary>
    /// <param name="muted"><c>true</c> if video ads should being in muted state.</param>
    public static void SetMuted(bool muted)
    {
        _MaxSetMuted(muted);
    }

    [DllImport("__Internal")]
    private static extern bool _MaxIsMuted();

    /// <summary>
    /// Whether video ads begin in a muted state or not. Defaults to <c>false</c>.
    ///
    /// Note: Returns <c>false</c> if the SDK is not initialized.
    /// </summary>
    /// <returns><c>true</c> if video ads begin in muted state.</returns>
    public static bool IsMuted()
    {
        return _MaxIsMuted();
    }

    [DllImport("__Internal")]
    private static extern bool _MaxSetVerboseLogging(bool enabled);

    /// <summary>
    /// Toggle verbose logging of AppLovin SDK. If enabled AppLovin messages will appear in standard application log accessible via console. All log messages will have "AppLovinSdk" tag.
    /// </summary>
    /// <param name="enabled"><c>true</c> if verbose logging should be enabled.</param>
    public static void SetVerboseLogging(bool enabled)
    {
        _MaxSetVerboseLogging(enabled);
    }

    [DllImport("__Internal")]
    private static extern void _MaxSetTestDeviceAdvertisingIdentifiers(string[] advertisingIdentifiers, int size);

    /// <summary>
    /// Enable devices to receive test ads, by passing in the advertising identifier (IDFA/GAID) of each test device.
    /// Refer to AppLovin logs for the IDFA/GAID of your current device.
    /// </summary>
    /// <param name="advertisingIdentifiers">String list of advertising identifiers from devices to receive test ads.</param>
    public static void SetTestDeviceAdvertisingIdentifiers(string[] advertisingIdentifiers)
    {
        _MaxSetTestDeviceAdvertisingIdentifiers(advertisingIdentifiers, advertisingIdentifiers.Length);
    }

    #endregion

#endif
}
