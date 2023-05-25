
/****************************************************
 * FileName:		DefeatPanelUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-26-15:20:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class DefeatPanelUI : BasePanel
{

	#region UI Variable Statement 
	private Button button_PlayAgain; 
	private Button button_SkipLevel;
	private Button button_Upgrade;

    private GameObject zombieText;
    private GameObject humanText;
    private RectTransform defeatImage;
    private Image bg;

    private RectTransform circle;
    private Image ctimer;
    private Text ctext;
    #endregion

    #region UI Variable Assignment 
    private void InitUI() 
	{
        button_Upgrade = transform.Find("buttons/Upgrade").GetComponent<Button>();
        defeatImage = transform.Find("BG/defeat") as RectTransform;
        button_PlayAgain = transform.Find("buttons/PlayAgain").GetComponent<Button>(); 
		button_SkipLevel = transform.Find("buttons/SkipLevel").GetComponent<Button>();
        zombieText = transform.Find("Datas/BG/Z").gameObject;
        humanText = transform.Find("Datas/BG/H").gameObject;
        bg = transform.Find("Datas/BG").GetComponent<Image>();

        circle = transform.Find("circle") as RectTransform;
        circle.Find("ctimer").TryGetComponent(out ctimer);
        circle.Find("ctext").TryGetComponent(out ctext);
        button_SkipLevel.transform.DOScale(1.1f, .5f).SetLoops(-1, LoopType.Yoyo);
    }
	#endregion 

	#region UI Event Register 
	private void AddEvent() 
	{
        button_Upgrade.onClick.AddListener(OnUpgradeClicked);
        button_PlayAgain.onClick.AddListener(OnPlayAgainClicked);
		button_SkipLevel.onClick.AddListener(OnSkipLevelClicked);
	}

    private void Awake()
    {
        InitUI();
        AddEvent();
        secondTimer = new MyTimer(5);
        secondTimer.SetFinish();
    }

    private void OnUpgradeClicked()
    {
        UIPanelManager.Instance.PopPanel();
        SceneMainUI.Instance.Show();
        SceneMainUI.Instance.ChangeIndex(3);
        SDKInit.Instance.ShowInterstitial("Inter_upgrade");
    }

    private void OnPlayAgainClicked()
	{
        UIPanelManager.Instance.PopPanel();
        TransitionManager.Instance.ReLoadScene();
        SDKInit.Instance.ShowInterstitial("Inter_retry");
    }

	private void OnSkipLevelClicked()
	{
        if (SDKInit.Instance.RewardedAdsIsReady())
        {
            if (PlayerControl.Instance.IsDead)
            {
                SDKInit.rewardType = RewardType.Revive;
                
            }
            else
            {
                SDKInit.rewardType = RewardType.Function;
                SDKInit.rewardCallback = () => {
                    var timer = ZombieShowTimer.Instance.GameTimer.timer;
                    ZombieShowTimer.Instance.GameTimer.ReStart();
                    ZombieShowTimer.Instance.GameTimer.timer = Mathf.Max(timer - 20, 0);
                    GameManager.Instance.XuanYun(ZombieShowTimer.ZombiePlayer, 2f);
                    UIPanelManager.Instance.PopPanel();
                    GameUI.Data.SetActive(true);
                    GameManager.isWin = GameManager.isDead = false;
                    GameManager.Instance.GameContinue();
                };
            }
            SDKInit.Instance.ShowRewardedAds("revive");
        }
    }
    public static bool KillerKill = false;
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        secondTimer.ReStart();
        GameManager.Instance.GameStop();
        button_Upgrade.interactable = LevelSetting.Value > 3;
        AudioManager.Instance.ChangeMusicVolume(0);
        zombieText.SetActive(PlayerControl.Instance.IsZombie);
        humanText.SetActive(PlayerControl.Instance.IsHuman && KillerKill == false);
        bg.enabled = KillerKill == false ;
    }

    private MyTimer secondTimer ;
    private SignedTimer signedTimer = new SignedTimer();
    private void Update()
    {
        signedTimer.OnUpdate(secondTimer.IsFinish);
        secondTimer.OnUpdate(Time.deltaTime);
        if (!secondTimer.IsFinish)
        {
            if (PlayerControl.Instance.IsHuman)
            {
                
                button_PlayAgain.transform.localScale = Vector3.zero;
                button_Upgrade.transform.localScale = Vector3.zero;
                button_SkipLevel.gameObject.SetActive(true);
                button_SkipLevel.interactable = SDKInit.Instance.RewardedAdsIsReady();
                circle.gameObject.SetActive(true);
                var timer = secondTimer.GetRatioRemaining;
                ctimer.fillAmount = timer;
                ctext.text = Mathf.FloorToInt(timer * secondTimer.DurationTime).ToString();
            }
            else
                secondTimer.SetFinish();
        }
        
        if (secondTimer.IsFinish && signedTimer.IsPressDown)
        {
            if (SDKInit.NOADS.Value == 0)
            {
                AppLovinCrossPromo.Instance().ShowMRec(3, 55, 25, 25, 0);
            }
            circle.gameObject.SetActive(false);
            button_SkipLevel.gameObject.SetActive(false);
            button_PlayAgain.transform.DOScale(1, .5f);
            button_Upgrade.transform.DOScale(1, .5f);
            zombieText.transform.localScale = Vector3.zero;
            zombieText.transform.DOScale(Vector3.one, .3f);
            humanText.transform.localScale = Vector3.zero;
            humanText.transform.DOScale(Vector3.one, .3f);
            defeatImage.localScale = Vector3.zero;
            defeatImage.DOScale(Vector3.one, .15f);
        }
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
        KillerKill = false;
        AppLovinCrossPromo.Instance().HideMRec();
    }
    #endregion

}
