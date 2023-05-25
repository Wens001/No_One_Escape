
/****************************************************
 * FileName:		SceneMainUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-02-15:44:40
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------------------------
/**
* @author :
* date    :
* purpose :
*/
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
public class SceneMainUI :Singleton<SceneMainUI>
{
    public SceneSettingUI settingUI;


    #region UI Variable Statement 
    private Button Setting;

    private Text text_moneyText; 
	private Text text_coinText; 
	private Button button_shopBtn; 
	private Button button_shopChineseBtn;
    private Button button_lockBtn; 
	private Button button_playBtn; 
	private Button button_upgradeBtn; 
	private Button button_lockBtn7;

    private GameObject DownBG;
    private RectTransform DownBG1;

	private RectTransform[]textRt;
    private RectTransform[] buttonsRt;
    private Vector3[] buttonsBeginSize;
    public ReactiveProperty<int> index { get; private set; }

    public BasePanel[] uiPanels;

    #endregion

    #region UI Variable Assignment 
    private bool isInit = false;

    private void UIAnim(Transform trans)
    {
        trans.DOKill();
        trans.localScale = Vector3.one;
        trans.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
    }

    public void InitUI() 
	{
        if (isInit)
            return;
        isInit = true;
        DontDestroyOnLoad(gameObject);
        MoneyAndCoinInit();
        transform.Find("Setting").TryGetComponent(out Setting);
        Setting.OnClickAsObservable().Subscribe(_ => { UIAnim(Setting.transform); settingUI.OnEnter(); } );

        text_moneyText = transform.Find("rightUp/money/moneyText").GetComponent<Text>();
        text_coinText = transform.Find("rightUp/coin/coinText").GetComponent<Text>();

        DownBG = transform.Find("DownBG").gameObject;
        DownBG1 = transform.Find("DownBG/BG1") as RectTransform;

        button_shopChineseBtn = transform.Find("shopChineseBtn").GetComponent<Button>();
        button_shopBtn = transform.Find("DownImage/shopBtn").GetComponent<Button>(); 
		button_lockBtn = transform.Find("DownImage/lockBtn").GetComponent<Button>(); 
		button_playBtn = transform.Find("DownImage/playBtn").GetComponent<Button>(); 
		button_upgradeBtn = transform.Find("DownImage/upgradeBtn").GetComponent<Button>(); 
		button_lockBtn7 = transform.Find("DownImage/lockBtn2").GetComponent<Button>();

        buttonsRt = new RectTransform[5];
        buttonsBeginSize = new Vector3[5];
        buttonsRt[0] = button_shopBtn.transform as RectTransform;
        buttonsRt[1] = button_lockBtn.transform as RectTransform;
        buttonsRt[2] = button_playBtn.transform as RectTransform;
        buttonsRt[3] = button_upgradeBtn.transform as RectTransform;
        buttonsRt[4] = button_lockBtn7.transform as RectTransform;
        for (int i = 0; i < buttonsBeginSize.Length; i++)
            buttonsBeginSize[i] = buttonsRt[i].localScale;


        textRt = new RectTransform[5];
        textRt[0] = transform.Find("DownText/shopText").transform as RectTransform;
        textRt[1] = transform.Find("DownText/lockText").transform as RectTransform;
        textRt[2] = transform.Find("DownText/playText").transform as RectTransform;
        textRt[3] = transform.Find("DownText/upgradeText").transform as RectTransform;
        textRt[4] = transform.Find("DownText/lockText2").transform as RectTransform;
        for (int i = 0; i < textRt.Length; i++)
            textRt[i].localScale = Vector3.zero;

        index = new ReactiveProperty<int> { Value = 2 };
        index.Subscribe(_ => ChangeIndex(_));

        button_shopBtn.OnClickAsObservable().Subscribe(_ => { index.Value = 0; });
        //button_lockBtn.OnClickAsObservable();
        button_playBtn.OnClickAsObservable().Subscribe(_ => { index.Value = 2; });
        button_upgradeBtn.OnClickAsObservable().Subscribe(_ => { index.Value = 3; });
        //button_lockBtn7.OnClickAsObservable();

    }

    private int NowIndex = -1;

    private void ShowPanel(int index)
    {
        if (index == NowIndex)
            return;
        NowIndex = index;

        for (int i = 0; i < uiPanels.Length; i++)
        {
            if (!uiPanels[i])
                continue;
            if (index == i)
                uiPanels[i].OnEnter();
            else
                uiPanels[i].OnExit();
        }
    }

	#endregion 

	#region UI Event Register 

    public void ChangeIndex(int _index)
    {
        if (_index < 0)
        {
            NowIndex = _index;
            return;
        }

        var timer = .25f;
        for (int i = 0; i < buttonsRt.Length; i++)
            if (i != _index)
            {
                buttonsRt[i].DOKill();
                textRt[i].DOKill();
                buttonsRt[i].DOScale(buttonsBeginSize[i], timer);
                textRt[i].DOScale(0, .1f);
            }
            else
            {
                buttonsRt[i].DOKill();
                textRt[i].DOKill();
                buttonsRt[i].DOScale(.9f, timer);
                textRt[i].DOScale(1, timer);
            }
        DownBG1.anchoredPosition = new Vector2( (2 - _index) * -215f , 0);
        ShowPanel(_index);
    }

    #endregion

    #region ½ð±ÒºÍ×êÊ¯


    private Text money_text;
    private Text coin_text;

    

    private void MoneyAndCoinInit()
    {
        transform.Find("rightUp/money/moneyText").TryGetComponent(out money_text);
        transform.Find("rightUp/coin/coinText").TryGetComponent(out coin_text);

        
        GameSetting.Money.Property.Subscribe(a => {
            money_text.text = a.ToString();
            PlayerPrefs.SetInt(ConstValue.SaveDataStr.Money, a); 
        });

        GameSetting.Coin.Property.Subscribe(a => {
            coin_text.text = a.ToString();
            PlayerPrefs.SetInt(ConstValue.SaveDataStr.Coin, a);
        });

        Messenger.AddListener<int>(ConstValue.CallBackFun.AddMoney, AddMoney);
        Messenger.AddListener<int>(ConstValue.CallBackFun.AddCoin, AddCoin);

    }

    private void AddMoney(int value)
    {
        GameSetting.Money.Value += value;
    }

    private void AddCoin(int value)
    {
        GameSetting.Coin.Value += value;
    }

    #endregion


    private void Start()
    {
        InitUI();
        Show();
    }

    public void Show()
    {
        IsShow = true;
        SetDownActive(true);
        SDKInit.Instance.HideBanner();
        LevelSetting.Level.Property.Subscribe(a =>
        {
            button_shopBtn.interactable = a > 3;
            button_upgradeBtn.interactable = a > 3;
        });

        button_shopChineseBtn.gameObject.SetActive(EnterScene.IsChinese.Value);
        button_shopBtn.gameObject.SetActive(!EnterScene.IsChinese.Value);
    }

    private void SetDownActive(bool flag)
    {
        foreach (var item in textRt)
            item.gameObject.SetActive(flag);
        foreach (var item in buttonsRt)
            item.gameObject.SetActive(flag);
        DownBG.SetActive(true);

        button_shopChineseBtn.gameObject.SetActive(EnterScene.IsChinese.Value);
        button_shopBtn.gameObject.SetActive(!EnterScene.IsChinese.Value);
    }

    public static bool IsShow { get; private set; } 

    public void Hide()
    {
        IsShow = false;
        SetDownActive(false);
        Model_Upgrade.Instance.gameObject.SetActive(false);
        foreach (var item in uiPanels)
            item?.OnExit();
        index.Value = -1;
        DownBG.SetActive(false);

        button_shopChineseBtn.gameObject.SetActive(false);
        button_shopBtn.gameObject.SetActive(false);
    }
}
