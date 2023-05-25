
/****************************************************
 * FileName:		GameUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-25-18:13:48
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using DG.Tweening;

public class GameUI : BasePanel
{
    public Sprite livePic;
    public Sprite deadPic;

    public Sprite btnSwitch1;
    public Sprite btnSwitch2;

    #region UI Variable Statement 
    private Image image_clrcleTime;
    private Text text_Time;
    private RectTransform left_Group;
    private RectTransform right_Group;
    private GameObject time_circle;
    private Image time_circle_img;
    private Text time_circle_text;
    private Image[] left_Image;
    private Image[] left_Exit;
    private Image[] right_Image;

    private bool isInit = false;

    private Transform root;
    private Button addTimeAds;
    private bool gameOverPlayAds;

    private GameObject TimeData;
    public static GameObject Data;


    #endregion

    private void Awake()
    {
        if (isInit)
            return;
        InitUI();
        left_Image = new Image[4];
        left_Exit = new Image[4];
        for (int i = 0; i < left_Image.Length; i++)
        {
            left_Group.GetChild(i).TryGetComponent(out left_Image[i]);
            left_Image[i].transform.GetChild(0).TryGetComponent(out left_Exit[i]);
        }

        right_Image = right_Group.GetComponentsInChildren<Image>();
        Messenger.AddListener<HumanBase,HumanBase>(ConstValue.CallBackFun.PlayerDead, PlayerListener);
        Messenger.AddListener<HumanBase,HumanBase>(ConstValue.CallBackFun.PlayerRebone, PlayerListener);
        Messenger.AddListener<HumanBase,ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, ZombieShowFunc);
        Messenger.AddListener<Door, HumanBase>(ConstValue.CallBackFun.PlayerGoOut, PlayerGoOutListener);
        Messenger.AddListener(ConstValue.CallBackFun.GameOver, GameOverFunc);

        isInit = true;
    }
    private Color text_Time_baseColor;
    #region UI Variable Assignment 
    private void InitUI()
    {
        image_clrcleTime = transform.Find("Data/TimeData/clrcleTime").GetComponent<Image>();
        text_Time = transform.Find("Data/TimeData/Time").GetComponent<Text>();
        text_Time_baseColor = text_Time.color;
        left_Group = transform.Find("Data/left/Group").GetComponent<RectTransform>();
        right_Group = transform.Find("Data/right/Group").GetComponent<RectTransform>();
        time_circle = transform.Find("Data/circle").gameObject;
        time_circle_img = time_circle.transform.Find("ctimer").GetComponent<Image>();
        time_circle_text = time_circle.transform.Find("ctext").GetComponent<Text>();
        TimeData = transform.Find("Data/TimeData").gameObject;
        Data = transform.Find("Data").gameObject;
        root = transform.Find("Data/TimeData/root").transform;
        root.GetChild(0).TryGetComponent(out addTimeAds);
        root.DORotate(new Vector3(0, 0, 10), .5f).SetLoops(-1, LoopType.Yoyo);
        addTimeAds.onClick.AddListener(  ()=> {
            gameOverPlayAds = true;
            ZombieShowTimer.Instance.GameTimer.timer -= 30f;
            root.gameObject.SetActive(false);
        } );
    }
    #endregion




    private int second = -1;

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            time_circle.SetActive(false);
            return;
        }
        time_circle.SetActive(GameManager.Speed > 0.01f && !ZombieShowTimer.HasZombie);

        if (time_circle.activeSelf)
        {
            var rr = ZombieShowTimer.Instance.ZombieTimer.GetRatioRemaining;
            time_circle_img.fillAmount = rr;
            time_circle_text.text = Mathf.Ceil(rr * ZombieShowTimer.Instance.ZombieTimer.DurationTime).ToString();
        }
        if (TimeData.activeSelf && !GameManager.isWin && !GameManager.isDead)
        {
            var value = ZombieShowTimer.Instance.GameTimer.DurationTime - ZombieShowTimer.Instance.GameTimer.timer;
            image_clrcleTime.fillAmount = ZombieShowTimer.Instance.GameTimer.GetRatioRemaining;
            int tSecond = Mathf.RoundToInt( Mathf.Ceil( value ) );
            if (tSecond != second)
            {
                text_Time.text = "00:" + tSecond.ToString("00");
                second = tSecond;
            }
            if (value < 10 && !GameManager.isWin && !GameManager.isDead && LevelSetting.Value >= 3 && SDKInit.Instance.RewardedAdsIsReady() && !gameOverPlayAds)
                root.gameObject.SetActive(true);
            else
                root.gameObject.SetActive(false);
            if (value < 10)
            {
                var yushu = Mathf.Clamp01(1 - (value - Mathf.Floor(value))) ; //0-1
                yushu = (.5f - Mathf.Abs(.5f - yushu))*2;//0-1-0
                text_Time.color = Color.Lerp(Color.black, Color.red, yushu);
                text_Time.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one* 1.2f,yushu) ;
            }
            else
            {
                text_Time.color = text_Time_baseColor;
            }
        }
    }

    public override void OnEnter()
    {
        Awake();
        Data.SetActive(true);
        gameObject.SetActive(true);
        for (int i = 0; i < left_Image.Length; i++)
        {
            left_Image[i].gameObject.SetActive(false);
            left_Exit[i].enabled = false;
            right_Image[i].enabled = false;
        }
        this.AttachTimer(.1f, () => {
            ShowLeftImage(GameManager.Instance.humanBases.Count - 1 * (ZombieShowTimer.Instance.IsNotZomble ? 0 : 1) );
            ShowRightImage(GameManager.Instance.upDoor.propSize);
        });
        TimeData.SetActive(false);
        text_Time.rectTransform.localScale = Vector3.one;
        text_Time.color = Color.black;
        root.gameObject.SetActive(false);
    }

    public void PlayerListener(HumanBase target)
    {
        this.AttachTimer(.1f, () => { UpdatePlayerData(); });
    }

    public void PlayerListener(HumanBase human, HumanBase help)
    {
        this.AttachTimer(.1f, () => { UpdatePlayerData(); });
        
    }

    public void PlayerGoOutListener(Door door,HumanBase target)
    {
        this.AttachTimer(.1f,()=> { UpdatePlayerData(); });
    }

    public void UpdatePlayerData()
    {
        var liveSize = GameManager.LiveHumanSize;
        for (int i = 0; i < liveSize; i++)
        {
            left_Image[i].sprite = livePic;
            left_Exit[i].enabled = false;
        }

        for (int i = liveSize; i < liveSize + GameManager.GoOutHumanSize; i++)
        {
            left_Image[i].sprite = livePic ;
            left_Exit[i].enabled = true;
        }

        for (int i = liveSize + GameManager.GoOutHumanSize; i < left_Image.Length; i++)
        {
            left_Image[i].sprite = deadPic;
            left_Exit[i].enabled = false;
        }
    }

    public void ListenerDoor(HumanBase human,ButtonProp button)
    {
        this.AttachTimer(.1f, () => {
            for (int i = 0; i < right_Image.Length; i++)
                right_Image[i].sprite = i < GameManager.Instance.upDoor.propSize ? btnSwitch1 : btnSwitch2;
        });
    }

    public void ZombieShowFunc(HumanBase human)
    {
        TimeData.SetActive(true);
    }

    public void GameOverFunc()
    {
        Data.SetActive(false);
        if (gameOverPlayAds)
        {
            gameOverPlayAds = false;
            SDKInit.rewardType = RewardType.Null;
            SDKInit.Instance.ShowRewardedAds("moretime_30s");
        }
    }


    public void ShowLeftImage(int count)
    {
        for (int i = 0; i < count; i++)
        {
            left_Image[i].gameObject.SetActive(true);
            left_Image[i].sprite = livePic;
            left_Exit[i].enabled = false;
        }
    }

    public void ShowRightImage(int count)
    {
        for (int i = 0; i < count; i++)
        {
            right_Image[i].enabled = true;
            right_Image[i].sprite = btnSwitch1;
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

    }


}
