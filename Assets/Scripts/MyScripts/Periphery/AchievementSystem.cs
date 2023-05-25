
/****************************************************
 * FileName:		AchievementSystem.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-24-14:30:43
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 成就系统，需挂载在全局物体上
/// </summary>
public class AchievementSystem : Singleton<AchievementSystem>
{
    #region --- Public Variable ---
    public List<AchievementData> achievementList;
    public Dictionary<string, AchievementData> achievementDic;
    #endregion

    #region --- Private Variable ---
    private int lastMoney = 0;
    private int lastGems = 0;
    #endregion

    private void Awake()
    {
        achievementDic = ES3.Load<Dictionary<string, AchievementData>>("achievementDic", new Dictionary<string, AchievementData>());
        if (achievementDic.Count == 0)
        {
            Debug.Log("成就数据初始化");
            foreach (var item in achievementList)
            {
                achievementDic.Add(item.achievemenName.ToString(), item);
            }
            ES3.Save<Dictionary<string, AchievementData>>("achievementDic", achievementDic);
        }
        //Debug.Log(achievementDic.Count);
    }

    public void SaveData()
    {
        ES3.Save<Dictionary<string, AchievementData>>("achievementDic", achievementDic);
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, HasPlayerDead);
        Messenger.AddListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, HasButtonDown);
        //Messenger.AddListener(ConstValue.CallBackFun.GameOver, HasGameOver);
        Messenger.AddListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerRebone, HasPlayerRebone);
        Messenger.AddListener<Door, HumanBase>(ConstValue.CallBackFun.PlayerGoOut, HasPlayerEscape);
        GameSetting.Money.Property.Subscribe(HasCoinSpent);
        GameSetting.Coin.Property.Subscribe(HasGemSpent);

        Messenger.AddListener<int>(ConstValue.CallBackFun.CharcterUpgrade, HasCharacterUpgrade);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, HasPlayerDead);
        Messenger.RemoveListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, HasButtonDown);
        //Messenger.RemoveListener(ConstValue.CallBackFun.GameOver, HasGameOver);
        Messenger.RemoveListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerRebone, HasPlayerRebone);
        Messenger.RemoveListener<Door, HumanBase>(ConstValue.CallBackFun.PlayerGoOut, HasPlayerEscape);

        Messenger.RemoveListener<int>(ConstValue.CallBackFun.CharcterUpgrade, HasCharacterUpgrade);
    }

    /******测试用，清空成就数据******/
    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetKeyDown(KeyCode.E))
        {
            ClearAchievement();
        }
    }

    #region 成就Callback

    //kill成就
    private void HasPlayerDead(HumanBase killer, HumanBase player)
    {
        if (killer.IsMe && killer.IsZombie)
        {
            //achievementData.killHuman++;
            AchievementDegreeUP(AchievementType.KillTarget);
        }
    }

    //开关触碰成就
    private void HasButtonDown(HumanBase player, ButtonProp prop)
    {
        if (player == PlayerControl.Instance)
        {
            AchievementDegreeUP(AchievementType.TouchButtonTarget);
        }
        //achievementData.touchButton++;
    }

    /*
    private void HasGameOver()
    {
        if (GameManager.isWin)
        {
            AchievementDegreeUP(AchievementType.WinTarget);
            //achievementData.winTheGame++;
        }
        if (GameManager.isDead)
        {
            AchievementDegreeUP(AchievementType.FailTarget);
            //achievementData.lostTheGame++;
        }
    }
    */

    //救援成就
    private void HasPlayerRebone(HumanBase target, HumanBase source)
    {
        if (source == PlayerControl.Instance)
        {
            AchievementDegreeUP(AchievementType.HelpTarget);
            //achievementData.helpHuman++;
        }
    }

    //逃脱成就
    private void HasPlayerEscape(Door door, HumanBase human)
    {
        if (human == PlayerControl.Instance)
        {
            AchievementDegreeUP(AchievementType.EscapeTarget);
        }
    }

    //金币花费成就
    private void HasCoinSpent(int curMoney)
    {
        if (lastMoney == 0)
        {
            lastMoney = curMoney;
        }
        if (lastMoney > curMoney)
        {
            int num = lastMoney - curMoney;
            AchievementDegreeUP(AchievementType.CoinSpentTarget, num);
        }
        lastMoney = curMoney;
        //Debug.Log("金钱显示：" + money);
    }

    //宝石花费成就
    private void HasGemSpent(int curGems)
    {
        if (lastGems == 0)
        {
            lastGems = curGems;
        }

        int num = lastGems - curGems;
        lastGems = curGems;

        if (num > 0)
        {
            AchievementDegreeUP(AchievementType.GemSpentTarger, num);
        }

        //Debug.Log("宝石显示：" + curGems);
    }

    //角色升级成就（包含升级次数成就和满级达成次数成就）
    private void HasCharacterUpgrade(int curLevel)
    {
        AchievementDegreeUP(AchievementType.UpgradeTarget);

        if (curLevel == 9)
        {
            AchievementDegreeUP(AchievementType.UpgradeFullTarget);
        }
    }

    #endregion

    /// <summary>
    /// 成就完成度提升
    /// </summary>
    /// <param name="achievementType">需要判定的成就</param>
    /// <param name="UPExtent">提升程度</param>
    private void AchievementDegreeUP(AchievementType achievementType, int UPExtent = 1)
    {
        var tem = achievementDic[achievementType.ToString()];
        tem.curNum += UPExtent;
        while (!tem.isFinished && tem.curNum >= tem.CurTarget)
        {
            /*此处给予奖励*/
            GameSetting.Coin.Value += 2;
            Debug.Log(achievementType.ToString() + "。。达成阶段目标，给予奖励。。已达成目标：" + tem.CurTarget);

            //成就等级提升
            tem.curLevel += 1;
            if (tem.curLevel == tem.targetLevels.Count)
            {
                tem.isFinished = true;
                Debug.Log(achievementType.ToString() + "。。达成全目标");
            }
        }
        Debug.Log(achievementType.ToString() + "。。当前进度：" + tem.curNum + "。。。当前目标：" + tem.CurTarget);
        SaveData();
    }

    /// <summary>
    /// 成就数据清空
    /// </summary>
    public void ClearAchievement()
    {
        Debug.Log("成就系统数据清空");
        //ES3.DeleteKey("achievementDic");
        foreach (var item in achievementDic.Values)
        {
            item.curNum = 0;
            item.curLevel = 0;
            item.isFinished = false;
        }
        SaveData();
    }
}
