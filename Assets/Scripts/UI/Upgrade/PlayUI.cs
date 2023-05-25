
/****************************************************
 * FileName:		PlayUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-06-16:26:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UnityEngine.SceneManagement;

public class PlayUI : BasePanel
{
    public override void OnEnter()
    {
        Awake();
        gameObject.SetActive(true);
        Model_Upgrade.Instance.gameObject.SetActive(true);
        Model_Upgrade.Instance.ShowPlayModel();
        title.gameObject.SetActive(false);
        SDKInit.Instance.HideBanner();
        ChangeAdsBtn(IsAdsSelect.Value);
    }

    public override void OnExit()
    {
        Awake();
        gameObject.SetActive(false);
    }

    public override void OnPause(){}

    public override void OnResume(){}

    #region --- Private Variable ---

    private RectTransform title;

    private Button playBtn;
    private Text level;
    #endregion


    public static bool IsTriggerPlayBtn = true ;    //判断是否按下过Play按钮

    #region 广告阵营

    private Button LeftSelectBtn;
    private GameObject LeftSelected;
    private Button RightSelectBtn;
    private GameObject RightSelected;

    public static ReactiveProperty<int> IsAdsSelect ;             //判断是否选择过阵营

    private void InitA()
    {
        transform.Find("bg/left/LeftSelectBtn").TryGetComponent(out LeftSelectBtn);
        LeftSelected = transform.Find("bg/left/LeftSelected").gameObject;

        transform.Find("bg/right/RightSelectBtn").TryGetComponent(out RightSelectBtn);
        RightSelected = transform.Find("bg/right/RightSelected").gameObject;

        IsAdsSelect = new ReactiveProperty<int> { Value = -1 };
        IsAdsSelect.Subscribe(ChangeAdsBtn);

        LeftSelectBtn.OnClickAsObservable().Subscribe(_=> {
            SDKInit.rewardCallback = () => { IsAdsSelect.Value = 0; };
            SDKInit.rewardType = RewardType.Function;
            SDKInit.Instance.ShowRewardedAds("choosekiller");
        } );
        RightSelectBtn.OnClickAsObservable().Subscribe(_ => {
            SDKInit.rewardCallback = () => { IsAdsSelect.Value = 1; };
            SDKInit.rewardType = RewardType.Function;
            SDKInit.Instance.ShowRewardedAds("choosesurvivor");
        });
    }

    public void ChangeAdsBtn(int value)
    {
        if (value == -1)
        {
            LeftSelectBtn.gameObject.SetActive(false);
            LeftSelected.gameObject.SetActive(false);
            RightSelectBtn.gameObject.SetActive(false);
            RightSelected.gameObject.SetActive(false);
            return;
        }
        LeftSelectBtn.gameObject.SetActive(value != 0);
        LeftSelected.gameObject.SetActive(value == 0);
        RightSelectBtn.gameObject.SetActive(value == 0);
        RightSelected.gameObject.SetActive(value != 0);
    }

    #endregion

    private bool isinit = false;
    void Awake()
    {
        if (isinit)
            return;
        isinit = true;
        title = transform.Find("title") as RectTransform;
        transform.Find("playBtn").TryGetComponent(out playBtn);
        transform.Find("level").TryGetComponent(out level);
        playBtn.OnClickAsObservable().Where(_=> SceneMainUI.IsShow).Subscribe(_ => {
            SceneMainUI.Instance.Hide();
            SDKInit.Instance.ShowBanner();
            IsTriggerPlayBtn = true;
            LevelSetting.LoadNowLevel();
        });

        playBtn.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(-1,LoopType.Yoyo);


        InitA();
        LevelSetting.Level.Property.Subscribe(_=> {  level.text = (EnterScene.IsChinese.Value ? "关卡 " : "LEVEL ") + _.ToString(); });
    }


    private void UpdateUpdate()
    {
        LeftSelectBtn.interactable = SDKInit.Instance.RewardedAdsIsReady();
        RightSelectBtn.interactable = SDKInit.Instance.RewardedAdsIsReady();
    }

}
