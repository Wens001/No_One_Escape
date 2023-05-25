
/****************************************************
 * FileName:		WinPanelUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-26-16:01:20
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System;

public class WinPanelUI : BasePanel
{

	#region UI Variable Statement
	private Text text_level; 
	private Button button_doubleBtn; 
	private Button button_PlayAgain; 
	private Button button_NextLevel;
    private Button Upgrade;
    private RectTransform victory;
    private GameOverShowText gameOverShowText;
    #endregion

    #region UI Variable Assignment 
    private void InitUI() 
	{
		text_level = transform.Find("Datas/level").GetComponent<Text>(); 
		button_doubleBtn = transform.Find("buttons/doubleBtn").GetComponent<Button>(); 
		button_PlayAgain = transform.Find("buttons/PlayAgain").GetComponent<Button>(); 
		button_NextLevel = transform.Find("buttons/NextLevel").GetComponent<Button>();
        Upgrade = transform.Find("buttons/Upgrade").GetComponent<Button>();
        victory = transform.Find("Datas/victory").GetComponent<RectTransform>();
        gameOverShowText = new GameOverShowText(transform.Find("Datas/victory/strs"));
    }
    #endregion

    private bool isAnim = false;

    private bool IsAnim(Unit a)
    {
        return !isAnim;
    }

	#region UI Event Register 
	private void AddEvent() 
	{
		button_doubleBtn.OnClickAsObservable().Where(IsAnim).Subscribe(_ =>OndoubleBtnClicked());
		button_PlayAgain.OnClickAsObservable().Where(IsAnim).Subscribe(_=> OnPlayAgainClicked());
		button_NextLevel.OnClickAsObservable().Where(IsAnim).Subscribe(_ => OnNextLevelClicked());
        Upgrade         .OnClickAsObservable().Where(IsAnim).Subscribe(_ => OnUpgradeClicked());
    }

    private void Awake()
    {
        InitUI();
        AddEvent();

    }

    private void OnUpgradeClicked()
    {
        AddMoneyAnimation(gameOverShowText.GetMoney(), () => {
            UIPanelManager.Instance.PopPanel();
            LevelSetting.Value ++;
            SceneMainUI.Instance.Show();
            SceneMainUI.Instance.index.Value = 3;
            if ( Mathf.Approximately(SDKInit.GuideAB,1) )
                MainGuide.Instance.OnEnter();
        });
    }

    private IEnumerator AddMoneyAnimationIE(int value,Action ac)
    {

        float timer = 0;
        float Dtimer = .75f ;

        if (isClickDoubleBtn)
        {
            value = 0;
            Dtimer = 0;
        }

        var beginMoney = GameSetting.Money.Value;
        isAnim = true;
        for (; timer < Dtimer; )
        {
            GameSetting.Money.Value
                = beginMoney + Mathf.CeilToInt(Mathf.Lerp(0f, value, timer / Dtimer ));
            timer += Time.deltaTime;
            yield return null;
        }
        GameSetting.Money.Value = beginMoney + value;
        isAnim = false;
        ac?.Invoke();
    }

    private void AddMoneyAnimation(int value, Action ac)
    {
        StartCoroutine(AddMoneyAnimationIE(value, ac));
    }


    private bool isClickDoubleBtn;
    private void OndoubleBtnClicked()
	{
        if (SDKInit.Instance.RewardedAdsIsReady())
        {
            SDKInit.rewardType = RewardType.Function ;
            SDKInit.rewardCallback = () =>
            {
                button_doubleBtn.gameObject.SetActive(false);
                AddMoneyAnimation(gameOverShowText.GetMoney() * 2, ()=> { isClickDoubleBtn = true; });
            };
            SDKInit.Instance.ShowRewardedAds("double_Settlement");
        }
	}

	private void OnPlayAgainClicked()
	{
        AddMoneyAnimation(gameOverShowText.GetMoney(),()=> {
            UIPanelManager.Instance.PopPanel();
            TransitionManager.Instance.ReLoadScene();
        });
    }

	private void OnNextLevelClicked()
	{
        AddMoneyAnimation(gameOverShowText.GetMoney(), () => {
            UIPanelManager.Instance.PopPanel();
            LevelSetting.Value ++;
            SceneMainUI.Instance.Show();
            SceneMainUI.Instance.index.Value = 2;
        });
    }

    public override void OnEnter()
    {
        if (SDKInit.NOADS.Value == 0)
        {
            AppLovinCrossPromo.Instance().ShowMRec(3, 55, 25, 25, 0);
        }

        isClickDoubleBtn = false;
        gameObject.SetActive(true);
        button_doubleBtn.gameObject.SetActive(true);
        text_level.text = EnterScene.IsChinese.Value ? "关卡" : "LEVEL" + LevelSetting.Value.ToString();
        this.AttachTimer(.3f,()=>SDKInit.Instance.ShowInterstitial("Inter_win"));
        
        GameManager.Instance.GameStop();
        victory.localScale = Vector3.zero;
        victory.DOScale(Vector3.one, .7f).SetEase(Ease.InOutElastic);
        gameOverShowText.Show();

        button_doubleBtn.transform.DOKill();
        button_doubleBtn.transform.localScale = Vector3.zero;
        button_doubleBtn.transform
            .DOScale(Vector3.one, .7f)
            .SetEase(Ease.InOutElastic)
            .onComplete += ()=> {
                button_doubleBtn.transform
                .DOScale(1.2f, .5f)
                .SetLoops(-1,LoopType.Yoyo); };

        button_PlayAgain.transform.localScale = Vector3.zero;
        button_NextLevel.transform.localScale = Vector3.zero;

        Upgrade.transform.DOKill();
        Upgrade.transform.localScale = Vector3.zero;
        Observable.Timer(TimeSpan.FromSeconds(3.5f)).Subscribe(
            _=> {
                button_PlayAgain.transform.DOScale(Vector3.one, 1f);
                button_NextLevel.transform.DOScale(Vector3.one, 1f);
                Upgrade.transform.DOScale(Vector3.one, 1f)
                .onComplete += ()=> { Upgrade.transform.DOScale(1.2f, .5f).SetLoops(-1, LoopType.Yoyo); };
            }
            );
        if (LevelSetting.Value == 3)
        {
            Upgrade.gameObject.SetActive(true);
            button_PlayAgain.gameObject.SetActive(false);
        }
        else
        {
            Upgrade.gameObject.SetActive(false);
            button_PlayAgain.gameObject.SetActive(true);
        }
        button_NextLevel.gameObject.SetActive(LevelSetting.Value != 3);
        AudioManager.Instance.ChangeMusicVolume(0);
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {
        
    }

    public override void OnExit()
    {
        AppLovinCrossPromo.Instance().HideMRec();
        gameObject.SetActive(false);
    }
#endregion
}

public class GameOverShowText
{
    private Transform root;
    private StrData[] strDatas;
    public bool isInit { get; private set; } = false;
    private class StrData
    {
        public Transform root;
        public Text head;
        public Text value;
        public Image image;
        public Image cha;
        public Text money;
    }

    public GameOverShowText(Transform root)
    {
        this.root = root;
        Init();
    }

    private void Init()
    {
        if (isInit)
            return;
        strDatas = new StrData[3];
        for (int i = 0; i < strDatas.Length; i++)
        {
            strDatas[i] = new StrData();
            var child = root.GetChild(i);
            strDatas[i].root = child;
            child.Find("head").TryGetComponent(out strDatas[i].head);
            child.Find("value").TryGetComponent(out strDatas[i].value);
            child.Find("Image").TryGetComponent(out strDatas[i].image);
            child.Find("cha").TryGetComponent(out strDatas[i].cha);
            child.Find("money").TryGetComponent(out strDatas[i].money);
        }
        isInit = true;
        
    }

    public int GetMoney()
    {
        Init();
        int res = 0;
        if (PlayerControl.Instance.IsZombie)
        {
            res += GameManager.Instance.gameDataCount.killHuman * 10;
            res += 30;
        }
        else
        {
            res += GameManager.Instance.gameDataCount.clickTrigger * 10;
            res += GameManager.Instance.gameDataCount.helpHuman * 10;
            res += 30 ;
        }
        return res;
    }

    public void Show()
    {
        Init();
        //如果玩家是杀手
        if (PlayerControl.Instance.IsZombie)
        {
            SetDataOther(0, EnterScene.IsChinese.Value ? " 抓 住" : "CAUGHT", GameManager.Instance.gameDataCount.killHuman);

            strDatas[1].head.text = EnterScene.IsChinese.Value ? " 逃 脱" : "ESCAPED";
            strDatas[1].value.text = "0";
            strDatas[1].image.gameObject.SetActive(GameManager.GoOutHumanSize == 0);
            strDatas[1].cha.gameObject.SetActive(GameManager.GoOutHumanSize != 0);
            strDatas[1].money.text = GameManager.GoOutHumanSize != 0 ? "" : "30";

            strDatas[2].root.gameObject.SetActive(false);
            DelayShow(.5f, 2);
        }
        //幸存者
        else
        {
            SetDataOther(0,  EnterScene.IsChinese.Value ? " 解 锁" : "UNLOCKED", GameManager.Instance.gameDataCount.clickTrigger );
            SetDataOther(1, EnterScene.IsChinese.Value ? " 救 助" : "SAVED", GameManager.Instance.gameDataCount.helpHuman );
            SetData2(EnterScene.IsChinese.Value ? " 逃 脱" : "ESCAPED", 30);
            DelayShow(.5f,3);
        }
        
    }

    public void DelayShow(float delay,int size)
    {
        var time = delay;
        for (int i = 0; i < size; i++)
        {
            var t = i;
            strDatas[t].root.gameObject.SetActive(false);
            Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(
                _=> {
                    strDatas[t].root.gameObject.SetActive(true);
                    strDatas[t].root.localScale = Vector3.zero;
                    strDatas[t].root.DOScale(Vector3.one, .3f);
                }
                );
            time += delay;
        }
    }

    private void SetDataOther(int i,string str,int size)
    {
        strDatas[i].head.text = str;
        strDatas[i].value.text = size == -1 ? "" : (size).ToString(); 
        strDatas[i].image.gameObject.SetActive(true);
        strDatas[i].cha.gameObject.SetActive(false);
        strDatas[i].money.text = (size * 10).ToString();
    }

    private void SetData2(string str,int value)
    {
        strDatas[2].root.gameObject.SetActive(true);
        strDatas[2].head.text = str;
        strDatas[2].value.text = "";
        if (GameManager.isWin)
        {
            strDatas[2].money.text = value.ToString();
            strDatas[2].cha.gameObject.SetActive(false);
            strDatas[2].image.gameObject.SetActive(true);
        }
        if (GameManager.isDead)
        {
            strDatas[2].money.text = "";
            strDatas[2].cha.gameObject.SetActive(true);
            strDatas[2].image.gameObject.SetActive(false);
        }
    }

}
