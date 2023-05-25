
/****************************************************
 * FileName:		CheckPassword.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-17-10:07:30
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;

public class CheckPassword : MonoBehaviour
{

    #region --- Private Variable ---

    public static ReactiveProperty<bool> IsClearBG;
    private Color _bgColor;

    private MyTimer checkT;

    #endregion

    public GameObject debugObj;

    private void Awake()
    {
        checkT = new MyTimer(.2f);
        IsClearBG = new ReactiveProperty<bool> { Value = false };
        LoadConfig();

    }
    private void Start()
    {
        _bgColor = EnterScene._cameraColor.backgroundColor;
        IsClearBG.Subscribe(a => { EnterScene._cameraColor.backgroundColor = a ? Color.green : _bgColor; });
        debugObj.gameObject.SetActive(false);
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        checkT.OnUpdate(Time.deltaTime);
        if (checkT.IsFinish)
        {
            checkT.ReStart();
            var buffer = GUIUtility.systemCopyBuffer;
            buffer = Regex.Replace(buffer, @"\s", "");
            if (buffer.Length >= 40 || buffer.Length < 10)
                return;
            if (!int.TryParse(buffer.Substring(buffer.Length - 1), out int value) )
                return;
            buffer = buffer.Remove(buffer.Length - 1);
            switch (buffer)
            {
                //debug
                case "qrR8xPlFRfyF6yd4v9J694b9S":
                    GUIUtility.systemCopyBuffer = "";
                    debugObj.gameObject.SetActive(value != 0);
                    Debug.unityLogger.logEnabled = value != 0;
                    break;
                //广告
                case "aq89Hl853iXC9qOyViPe60ClF":
                    GUIUtility.systemCopyBuffer = "";
                    SDKInit.IsDebug = value == 0;
                    if (value != 0)
                        Debug.Log("已开启广告功能");
                    else
                        Debug.Log("已关闭广告功能");
                    break;
                //绿屏
                case "iW9808c25xlhcB2R6V73Er6B3":
                    GUIUtility.systemCopyBuffer = "";
                    IsClearBG.Value = !IsClearBG.Value;
                    break;
                //金币
                case "GrF60qK7M1XYrvz6567p0Fyo5":
                    GUIUtility.systemCopyBuffer = "";
                    var AddMul = value != 0 ? 1 : -1;
                    GameSetting.Money.Value = Mathf.Clamp(GameSetting.Money.Value + AddMul * 50000, 0, 99999);
                    GameSetting.Coin.Value = Mathf.Clamp(GameSetting.Coin.Value + AddMul * 50000, 0, 99999);
                    break;
                //增加Level
                case "0U2B7U6n6nAb3F0s51l5232iY":
                    GUIUtility.systemCopyBuffer = "";
                    LevelSetting.Value += value;
                    break;
                //debug广告
                case "nI76gK6i8u9I8Ib9P4aBQTt3u":
                    GUIUtility.systemCopyBuffer = "";
                    Messenger.Broadcast("FuckingABTest");
                    MaxSdk.ShowMediationDebugger();
                    break;
                //清理数据
                case "c9bwZLu37z6TeGJtYM238Oh7k":
                    GUIUtility.systemCopyBuffer = "";
                    PlayerPrefs.DeleteAll();
                    Application.Quit();
                    break;
            }
        }

    }

    public GameObject PerformanceTool;

    private void LoadConfig()
    {
        SceneSettingUI.OpenDebug.Subscribe(a => PerformanceTool.SetActive(a));

        var path = "";
#if UNITY_EDITOR || UNITY_IPHONE
        path = "file://" + Application.streamingAssetsPath + "/tmp.txt";
#else
        path =  Application.streamingAssetsPath + "/tmp.txt";
#endif

        StartCoroutine(DoLoad(path));
    }

    private IEnumerator DoLoad(string path)
    {
        var www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        SceneSettingUI.OpenDebug.Value = int.Parse(www.downloadHandler.text.Trim()) == 996;
    }
}
