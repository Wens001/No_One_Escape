
/****************************************************
 * FileName:		EnterScene.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-25-18:28:10
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using MoreMountains.NiceVibrations;
using GameToolkit.Localization;
using UniRx;

public class EnterScene : MonoBehaviour
{

    public static Camera _cameraColor;

    private static UniversalAdditionalCameraData cameraData;
    private static bool isinit = false;

    private static ReactiveProperty<bool> isChinese;
    public static ReactiveProperty<bool> IsChinese
    {
        get
        {
            if (isChinese == null)
            {
                isChinese = new ReactiveProperty<bool>();

                bool isc = Application.systemLanguage == SystemLanguage.Chinese;
                isc = isc || Application.systemLanguage == SystemLanguage.ChineseSimplified;
                isc = isc || Application.systemLanguage == SystemLanguage.ChineseTraditional;

                if (false)
                {
                    Localization.Instance.CurrentLanguage = Language.Chinese;
                    isChinese.Value = true;
                }
            }
            return isChinese;
        }
    }

    private void Awake()
    {
        if (isinit)
            return;
        isinit = true;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        transform.Find("CameraStack").TryGetComponent(out cameraData);
        cameraData.TryGetComponent(out _cameraColor);

        MMVibrationManager.iOSInitializeHaptics();

    }

    

    public static void TryToAddStack(Camera camera)
    {
        if (!cameraData.cameraStack.Contains(camera))
            cameraData.cameraStack.Insert(0,camera);
    }

    public static void TryToRemoveStack(Camera camera)
    {
        if (cameraData.cameraStack.Contains(camera))
            cameraData.cameraStack.Remove(camera);
    }

}
