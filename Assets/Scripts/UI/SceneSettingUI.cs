
/****************************************************
 * FileName:		SceneSettingUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-07-13:55:17
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
public class SceneSettingUI : BasePanel
{

    #region --- Private Variable ---
    #region Debug

    private static ReactiveProperty<bool> otherColor;
    public static ReactiveProperty<bool> OtherColor
    {
        get
        {
            if (otherColor == null)
                otherColor = new ReactiveProperty<bool>();
            return otherColor;
        }
    }
    private Toggle OtherColorTog;


    private static ReactiveProperty<bool> joystick;
    public static ReactiveProperty<bool> JoyStick
    {
        get
        {
            if (joystick == null)
                joystick = new ReactiveProperty<bool>();
            return joystick;
        }
    }
    private Toggle JoyStickTog;

    private static ReactiveProperty<bool> view;
    public static ReactiveProperty<bool> View
    {
        get
        {
            if (view == null)
                view = new ReactiveProperty<bool>();
            return view;
        }
    }
    private Toggle ViewTog;

    private static ReactiveProperty<bool> topDown;
    public static ReactiveProperty<bool> TopDown
    {
        get
        {
            if (topDown == null)
                topDown = new ReactiveProperty<bool>();
            return topDown;
        }
    }
    private Toggle TopDownTog;


    private static ReactiveProperty<bool> opendebug ;
    public static ReactiveProperty<bool> OpenDebug { get {
            if (opendebug == null)
                opendebug = new ReactiveProperty<bool>();
            return opendebug;
        } }
    private GameObject DebugPanel;
    private Transform ToggleGroup1;
    private Text speedText;
    public static Toggle[] toggles { get; private set; }
    private Button noAdsBtn;
    private Button greenBtn;

    #endregion


    private Transform others;
    private Button soundOn;
    private Button soundOff;
    private Button snakeOn;
    private Button snakeOff;
    private Button closeBtn;
    private Button privacyBtn;
    private Button AddMoney;
    private Button ClearAll;
    private Button Scenes;

    private Image bg;

    public override void OnEnter()
    {
        gameObject.SetActive(true);
        bg.enabled = true;
        others.gameObject.SetActive(true);
        others.DOKill();
        others.localScale = Vector3.zero;
        others.DOScale(Vector3.one,.3f).SetEase(Ease.InOutQuad);

        OpenDebug.Subscribe( a=> {
            if (!a)
                return;
            if ( SceneMainUI.Instance.index.Value < 0)
            {
                var hbs = GameManager.Instance.humanBases;
                speedText.text = "";
                for (int i = 0; i < hbs.Count; i++)
                {
                    if (hbs[i].IsMe)
                        speedText.text += "MySpeed" + (hbs[i].IsZombie ? hbs[i].p_Para.Kill_BaseSpeed : hbs[i].p_Para.Human_BaseSpeed) + "\n";
                    else
                        speedText.text += "AISpeed" + (hbs[i].IsZombie ? hbs[i].p_Para.Kill_BaseSpeed : hbs[i].p_Para.Human_BaseSpeed) + "\n";
                }
                speedText.text += "Nandu  " + SDKInit.DifficultyAB + "\n";
                speedText.text += "Yindao  " + SDKInit.GuideAB + "\n";
                speedText.text += "ABTest  " + SDKInit.GroupStr + "\n";

            }
        } );
    }

    public override void OnExit()
    {
        others.DOKill();
        others.localScale = Vector3.one;
        others.DOScale(Vector3.zero,.2f).SetEase(Ease.InOutQuad).onComplete += ()=> { gameObject.SetActive(false); };
    }

    public override void OnPause()
    {
        
    }

    public override void OnResume()
    {

    }
    #endregion

    private void Awake()
    {
        #region Debug

        TryGetComponent(out bg);
        DebugPanel = transform.Find("Others/DebugPanel").gameObject;
        ToggleGroup1 = DebugPanel.transform.Find("ToggleGroup1");
        DebugPanel.transform.Find("speedText").TryGetComponent(out speedText);
        toggles = new Toggle[4];
        for (int i = 0; i < toggles.Length; i++)
        {
            var t = i;
            ToggleGroup1.GetChild(i).TryGetComponent(out toggles[i]);
        }
        DebugPanel.transform.Find("Add").TryGetComponent(out Button addBtn);
        DebugPanel.transform.Find("Sub").TryGetComponent(out Button subBtn);
        DebugPanel.transform.Find("Level").TryGetComponent(out Text levelText);
        DebugPanel.transform.Find("GreenScreen").TryGetComponent(out greenBtn);
        DebugPanel.transform.Find("NoAds").TryGetComponent(out noAdsBtn);
        DebugPanel.transform.Find("AddMoney").TryGetComponent(out AddMoney);
        DebugPanel.transform.Find("ClearAll").TryGetComponent(out ClearAll);
        DebugPanel.transform.Find("Scenes").TryGetComponent(out Scenes);

        Scenes.OnClickAsObservable().Subscribe(_ =>
        {
            var value = LevelSetting.Level.Value;
            LevelSetting.Level.Value = 31;
            SceneMainUI.Instance.Hide();
            SDKInit.Instance.ShowBanner();
            LevelSetting.LoadNowLevel();
            LevelSetting.Level.Value = value;
        });


        DebugPanel.transform.Find("JoyStick").TryGetComponent(out JoyStickTog);
        JoyStickTog.OnValueChangedAsObservable().Subscribe(value => JoyStick.Value = value);

        DebugPanel.transform.Find("OtherColor").TryGetComponent(out OtherColorTog);
        OtherColorTog.isOn = false;
        OtherColorTog.OnValueChangedAsObservable().Subscribe(value => OtherColor.Value = value);

        DebugPanel.transform.Find("View").TryGetComponent(out ViewTog);
        ViewTog.isOn = false;
        ViewTog.OnValueChangedAsObservable().Subscribe(value =>
        {
            if (value)
                TopDownTog.isOn = false;
            View.Value = value;
        }
        );
        View.Subscribe(value => ViewTog.isOn = value );

        DebugPanel.transform.Find("TopDown").TryGetComponent(out TopDownTog);
        TopDownTog.isOn = false;
        TopDownTog.OnValueChangedAsObservable().Subscribe(value => {
            if (value)
                ViewTog.isOn = false;
            TopDown.Value = value;
        }
        
        );

        AddMoney.onClick.AddListener(() => GameSetting.Money.Value = 99999 );
        ClearAll.onClick.AddListener(() => { PlayerPrefs.DeleteAll(); Application.Quit(); });
        greenBtn.onClick.AddListener(() => {
            CheckPassword.IsClearBG.Value = !CheckPassword.IsClearBG.Value;
        });
        noAdsBtn.onClick.AddListener(() => { SDKInit.IsDebug = true; });
        addBtn.OnClickAsObservable().Subscribe(_ => LevelSetting.Level.Value++);
        subBtn.OnClickAsObservable().Subscribe(_ => { LevelSetting.Level.Value = Mathf.Clamp(LevelSetting.Level.Value - 1, 1, 9999); });
        LevelSetting.Level.Property.Subscribe(_ => levelText.text = LevelSetting.Level.Value.ToString());
        OpenDebug.Subscribe(a=> DebugPanel.gameObject.SetActive(a));



        #endregion



        others = transform.Find("Others");
        transform.Find("Others/soundOn").TryGetComponent(out soundOn);
        transform.Find("Others/soundOff").TryGetComponent(out soundOff);
        transform.Find("Others/snakeOn").TryGetComponent(out snakeOn);
        transform.Find("Others/snakeOff").TryGetComponent(out snakeOff);
        transform.Find("Others/closeBtn").TryGetComponent(out closeBtn);
        transform.Find("Others/privacy").TryGetComponent(out privacyBtn);

        SoundOnOff( GameSetting.Sound.Value >= .9f );
        SnakeOnOff(GameSetting.VibrationOn.Value == 1);

        soundOn.OnClickAsObservable().Subscribe(_ => SoundOnOff(false));
        soundOff.OnClickAsObservable().Subscribe(_ => SoundOnOff(true));

        snakeOn.OnClickAsObservable().Subscribe(_ => SnakeOnOff(false));
        snakeOff.OnClickAsObservable().Subscribe(_ => SnakeOnOff(true));

        closeBtn.OnClickAsObservable().Subscribe(_ => OnExit());

        privacyBtn.OnClickAsObservable().Subscribe( _=>GDPRView.Instance.Show(3) );

        SDKInit.isGDPR.Subscribe( a =>privacyBtn.gameObject.SetActive(a) );
        
        gameObject.SetActive(false);

    }

    private void SoundOnOff(bool ison)
    {
        GameSetting.Sound.Value = (ison ? 1 : 0);
        soundOn.gameObject.SetActive(ison);
        soundOff.gameObject.SetActive(!ison);
    }
    private void SnakeOnOff(bool ison)
    {
        GameSetting.VibrationOn.Value = ison ? 1 : 0; 
        snakeOn.gameObject.SetActive(ison);
        snakeOff.gameObject.SetActive(!ison);
    }

}
