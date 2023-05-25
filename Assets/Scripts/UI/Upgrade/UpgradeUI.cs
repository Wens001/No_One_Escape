
/****************************************************
 * FileName:		UpgradeUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-03-12:34:53
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
using System;
using Facebook.Unity;
public class UpgradeUI : BasePanel
{

    #region --- Public Variable ---

    [System.Serializable]
    public class PlayerData
    {
        public Sprite sprite;
        public string name;
        public string nameChinese;
        public string SaveName;
    }

    public PlayerData[] humanData;
    public PlayerData[] killerData;

    public UpgradePlayerDataUI upData;
    public UpgradePlayerDataUI downData;

    #endregion


    #region --- Private Variable ---
    private Button selectBtn;
    private GameObject IsSelect;

    private Button moreBtn;
    private Button backBtn;


    private RectTransform DownDatas;
    private RectTransform DownData2;

    private Button leftArror;
    private Button rightArror;

    private RectTransform rightData;
    private GameObject player_Text;

    #endregion
    private bool isinit = false;

    private float moveTime = .4f;

    #region 购买按钮

    public ReactiveProperty<int> killerIndex { get { return Model_Upgrade.Instance.killerIndex; } }
    public ReactiveProperty<int> humanIndex { get { return Model_Upgrade.Instance.humanIndex; } }

    private Button buyHumanBtn;
    private Text buyText;

    public const string BuyHuman = "BuyHuman";
    public const string BuyKiller = "BuyKiller";



    private void IsBuyButton()
    {

        var isleft = UpgradeTeamUI.Instance.leftTeam.Value;
        int isbuy;
        if (isleft)
        {
            isbuy = PlayerPrefs.GetInt(BuyKiller + killerIndex.Value, 0);
            buyText.text = Model_Upgrade.Instance.killerModels[killerIndex.Value].needMoney.ToString();
        }
        else
        {
            isbuy = PlayerPrefs.GetInt(BuyHuman + humanIndex.Value, 0);
            buyText.text = Model_Upgrade.Instance.humanModels[humanIndex.Value].needMoney.ToString();
        }

        buyHumanBtn.gameObject.SetActive(isbuy == 0);
        if (isbuy == 0)
        {
            IsSelect.gameObject.SetActive(false);
            selectBtn.gameObject.SetActive(false);
        }
        player_Text.gameObject.SetActive(isbuy == 0);
        upData.gameObject.SetActive(isbuy != 0);
        downData.gameObject.SetActive(isbuy != 0);
    }

    #endregion

    private void UIAnim(Transform trans)
    {
        trans.DOKill();
        trans.localScale = Vector3.one;
        trans.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
    }

    public void Init()
    {
        if (isinit)
            return;
        transform.Find("DownDatas/more").TryGetComponent(out moreBtn);
        transform.Find("DownData2/backBtn").TryGetComponent(out backBtn);
        transform.Find("DownDatas").TryGetComponent(out DownDatas);
        transform.Find("DownData2").TryGetComponent(out DownData2);
        transform.Find("LeftDatas/leftArror").TryGetComponent(out leftArror);
        transform.Find("LeftDatas/rightArror").TryGetComponent(out rightArror);
        rightData = transform.Find("RightDatas") as RectTransform;
        player_Text = transform.Find("DownDatas/player_Text").gameObject;

        transform.Find("BuyHuman").TryGetComponent(out buyHumanBtn);
        buyHumanBtn.transform.GetChild(0).TryGetComponent(out buyText);
        PlayerPrefs.SetInt(BuyHuman + 0, 1);
        PlayerPrefs.SetInt(BuyKiller + 0, 1);
        buyHumanBtn.OnClickAsObservable().Subscribe(_ => {
            UIAnim(buyHumanBtn.transform);
            var isleft = UpgradeTeamUI.Instance.leftTeam.Value;
            var needMoney = 0;
            string str;
            if (isleft)
            {
                needMoney = Model_Upgrade.Instance.killerModels[killerIndex.Value].needMoney;
                str = BuyKiller + killerIndex.Value;
            }
            else
            {
                needMoney = Model_Upgrade.Instance.humanModels[humanIndex.Value].needMoney;
                str = BuyHuman + humanIndex.Value;
            }
            if (GameSetting.Money.Value >= needMoney)
            {
                GameSetting.Money.Value -= needMoney;
                PlayerPrefs.SetInt(str, 1);
                if (isleft)
                    SelectBtnCallBack(killerIndex.Value);
                else
                    SelectBtnCallBack(humanIndex.Value);
            }
            //钱不够
            else
            {
                //SceneMainUI.Instance.index.Value = 0;
            }
        });

        Messenger.AddListener(ConstValue.CallBackFun.FBSendAllHumansData, FbSendHumanMessage);

        transform.Find("selectBtn").TryGetComponent(out selectBtn);
        IsSelect = transform.Find("IsSelect").gameObject;
        Model_Upgrade.Instance.Init();
        Model_Upgrade.Instance.humanIndex.Subscribe(SelectBtnCallBack);
        Model_Upgrade.Instance.killerIndex.Subscribe(SelectBtnCallBack);
        selectBtn.OnClickAsObservable().Subscribe(_ => SelectSaveData(true));
        UpgradeTeamUI.Instance.InitUI();
        UpgradeTeamUI.Instance.leftTeam.Subscribe(_ => SelectSaveData(false));
        SelectSaveData(false);

        moreBtn.OnClickAsObservable().Where(_ => !GameManager.UIIsMove).Subscribe(_ => {
            GameManager.UIIsMove = true;
            AnimSetActiveFalse(DownDatas, -1000);
            SetActive(DownData2, true);
            DownData2.DOLocalMoveX(0, .15f).SetEase(Ease.InOutQuad);
            Observable.Timer(TimeSpan.FromSeconds(moveTime))
            .Subscribe(a => { GameManager.UIIsMove = false; });
        }
        );
        backBtn.OnClickAsObservable().Where(_ => !GameManager.UIIsMove).Subscribe(_ => {
            GameManager.UIIsMove = true;
            AnimSetActiveFalse(DownData2, 1000);
            SetActive(DownDatas, true);
            DownDatas.DOLocalMoveX(0, .15f).SetEase(Ease.InOutQuad);
            Observable.Timer(TimeSpan.FromSeconds(moveTime))
            .Subscribe(a => { GameManager.UIIsMove = false; });
        }
        );
        DownData2.gameObject.SetActive(false);


        upData.InitUI();
        downData.InitUI();

        leftArror.OnClickAsObservable().Subscribe(_ => Model_Upgrade.Instance.LeftArrorButton());
        rightArror.OnClickAsObservable().Subscribe(_ => Model_Upgrade.Instance.RightArrorButton());

        isinit = true;
        OnExit();
    }

    private void SelectBtnCallBack(int value)
    {
        UpgradeTeamUI.Instance.InitUI();
        var isleft = UpgradeTeamUI.Instance.leftTeam.Value;
        bool istrue;
        if (isleft)
            istrue = value == PlayerPrefs.GetInt(ConstValue.SaveDataStr.KillerIndex, 0);
        else
            istrue = value == PlayerPrefs.GetInt(ConstValue.SaveDataStr.HumanIndex, 0);
        IsSelect.gameObject.SetActive(istrue);
        selectBtn.gameObject.SetActive(!istrue);

        IsSelect.transform.DOKill();
        selectBtn.transform.DOKill();
        IsSelect.transform.localScale = Vector3.one;
        selectBtn.transform.localScale = Vector3.one;
        if (istrue)
            IsSelect.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
        else
            selectBtn.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
        IsBuyButton();
    }

    private void SelectSaveData(bool save)
    {
        var isleft = UpgradeTeamUI.Instance.leftTeam.Value;
        if (isleft)
        {
            if (save)
                PlayerPrefs.SetInt(ConstValue.SaveDataStr.KillerIndex, Model_Upgrade.Instance.killerIndex.Value);
            SelectBtnCallBack(Model_Upgrade.Instance.killerIndex.Value);
        }
        else
        {
            if (save)
                PlayerPrefs.SetInt(ConstValue.SaveDataStr.HumanIndex, Model_Upgrade.Instance.humanIndex.Value);
            SelectBtnCallBack(Model_Upgrade.Instance.humanIndex.Value);
        }
    }


    /// <summary>
    /// FB发送所有人物打点信息
    /// </summary>
    public void FbSendHumanMessage()
    {
        if (!FB.IsInitialized)
            return;
        var click = new Dictionary<string, object>();

        for (int i = 0; i < humanData.Length; i++)
        {
            var str = humanData[i].SaveName + i.ToString();
            click[str] = PlayerPrefs.GetInt(str, 0);
        }
        for (int i = 0; i < killerData.Length; i++)
        {
            var str = killerData[i].SaveName + i.ToString();
            click[str] = PlayerPrefs.GetInt(str, 0);
        }

        FB.LogAppEvent("Humans_Level", null, click);
    }


    /// <summary>
    /// 改变属性
    /// </summary>
    public void ChangeTeamData(bool isLeft, int index)
    {
        Init();
        upData.sprite.Value = isLeft ? killerData[0].sprite : humanData[0].sprite;
        upData._name.Value = isLeft ? 
           EnterScene.IsChinese.Value ? killerData[0].nameChinese : killerData[0].name :
           EnterScene.IsChinese.Value ? humanData[0].nameChinese : humanData[0].name;

        upData._SaveName.Value = (isLeft ? killerData[0].SaveName : humanData[0].SaveName) + index.ToString();

        downData.sprite.Value = isLeft ? killerData[1].sprite : humanData[1].sprite;

        downData._name.Value = isLeft ?
           EnterScene.IsChinese.Value ? killerData[1].nameChinese : killerData[1].name :
           EnterScene.IsChinese.Value ? humanData[1].nameChinese : humanData[1].name;

        downData._SaveName.Value = (isLeft ? killerData[1].SaveName : humanData[1].SaveName) + index.ToString();
    }

    private void AnimSetActiveFalse(RectTransform rt, float X)
    {
        rt.DOMoveX(X, moveTime).SetEase(Ease.InOutQuad)
            .onComplete += () => { SetActive(rt, false); };
        //SetActive(rt, false);
    }

    private void SetActive(RectTransform rt, bool at)
    {
        rt.gameObject.SetActive(at);
    }

    void Awake()
    {
        Init();
    }

    public override void OnEnter()
    {
        Init();
        gameObject.SetActive(true);
        Model_Upgrade.Instance.gameObject.SetActive(true);
        Model_Upgrade.Instance.ShowModel();
        SDKInit.Instance.HideBanner();
        //rightData.SetX(rightDataBeginX + 600);
        //rightData.DOMoveX(rightDataBeginX, .2f);
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
    }
}
