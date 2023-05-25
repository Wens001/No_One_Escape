//
//  AppLovinCrossPromo.cs
//  
//  Unity plugin used for displaying cross promo ads in the AppLovin MAX network.
//
//  Copyright © 2019 AppLovin. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AppLovinCrossPromo : MonoBehaviour
{
    private static AppLovinCrossPromo _instance;
    private CrossPromoInterface _crossPromoImpl;

    public static void Init()
    {
        if (_instance) return;
        
        _instance = new GameObject("CrossPromoObj").AddComponent<AppLovinCrossPromo>();

        // Ensure instance be available through all scenes
        DontDestroyOnLoad(_instance.gameObject);

        #if !UNITY_EDITOR
            #if UNITY_IOS
                _instance._crossPromoImpl = new CrossPromoViewIos();
            #elif UNITY_ANDROID
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _instance._crossPromoImpl = new CrossPromoViewAndroid( jc.GetStatic<AndroidJavaObject>("currentActivity") );
            #endif
        #endif
    }

    public static AppLovinCrossPromo Instance()
    {
        return _instance;
    }

    public void ShowMRec(float xPercentage, float yPercentage, float widthPercentage, float heightPercentage, float rotation)
    {
        if (xPercentage < 0f || xPercentage > 100f) throw new ArgumentOutOfRangeException("xPercentage", "Must be between 0 and 100");
        if (yPercentage < 0f || yPercentage > 100f) throw new ArgumentOutOfRangeException("yPercentage", "Must be between 0 and 100");
        if (widthPercentage < 0f || widthPercentage > 100f) throw new ArgumentOutOfRangeException("widthPercentage", "Must be between 0 and 100");
        if (heightPercentage < 0f || heightPercentage > 100f) throw new ArgumentOutOfRangeException("heightPercentage", "Must be between 0 and 100");
        
        if (_crossPromoImpl == null)
        {
            Debug.LogError("显示交叉广告失败");
            return;
        }
        Debug.Log("显示交叉广告");
        _crossPromoImpl.ShowMRec(xPercentage, yPercentage, widthPercentage, heightPercentage, rotation);
    }

    public void HideMRec()
    {
        if (_crossPromoImpl == null) return;
        
        _crossPromoImpl.HideMRec();
    }

    abstract class CrossPromoInterface
    {
        /// <summary>
        /// Show Cross Promo MREC ad.
        /// </summary>
        /// <param name="xPercentage">Top-left X position expressed as a percentage of the screen width (i.e. 20.0).</param>
        /// <param name="yPercentage">Top-left Y position expressed as a percentage of the screen height (i.e. 30.0).</param>
        /// <param name="widthPercentage">Width expressed as a percentage of the minimum value between screen width and height (i.e. 25.0).</param>
        /// <param name="heightPercentage">Height expressed as a percentage of the minimum value between screen width and height (i.e. 25.0).</param>
        /// <param name="rotation">Clock-wise rotation expressed in degrees (i.e., 45).</param>
        public abstract void ShowMRec(float xPercentage, float yPercentage, float widthPercentage, float heightPercentage, float rotation);

        /// <summary>
        /// Hide currently visible Cross Promo MREC ad.
        /// </summary>
        public abstract void HideMRec();
    }

    class CrossPromoViewAndroid : CrossPromoInterface
    {
        AndroidJavaObject _ajo;

        public CrossPromoViewAndroid(AndroidJavaObject activity)
        {
            try
            {
                _ajo = new AndroidJavaObject("com.applovin.sdk.unity.crosspromo.AppLovinCrossPromo", activity);
            }
            catch (AndroidJavaException exception)
            {
                Debug.LogError("AppLovinCrossPromo class not found. Please make sure that the cross promo unity package has been imported correctly.");
                Console.WriteLine(exception);
            }
        }

        public override void ShowMRec(float xPercentage, float yPercentage, float widthPercentage, float heightPercentage, float rotation)
        {
            _ajo.Call("showCrossPromoMRec", xPercentage, yPercentage, widthPercentage, heightPercentage, rotation);
        }

        public override void HideMRec()
        {
            _ajo.Call("hideCrossPromoMRec");
        }
    }

#if UNITY_IOS
    class CrossPromoViewIos : CrossPromoInterface
    {
        [DllImport("__Internal")]
        private static extern void _crossPromoCreateInstance();
        [DllImport("__Internal")]
        private static extern void _crossPromoShowMRec(float xPercentage, float yPercentage, float widthPercentage, float heightPercentage, float rotation);
        [DllImport("__Internal")]
        private static extern void _crossPromoHideMRec();

        public CrossPromoViewIos()
        {
            _crossPromoCreateInstance();
        }

        public override void ShowMRec(float xPercentage, float yPercentage, float widthPercentage, float heightPercentage, float rotation)
        {
            _crossPromoShowMRec(xPercentage, yPercentage, widthPercentage, heightPercentage, rotation);
        }

        public override void HideMRec()
        {
            _crossPromoHideMRec();
        }
    }
#endif
}
