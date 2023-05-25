
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
/// �ɾ�ϵͳ���������ȫ��������
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
            Debug.Log("�ɾ����ݳ�ʼ��");
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

    /******�����ã���ճɾ�����******/
    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetKeyDown(KeyCode.E))
        {
            ClearAchievement();
        }
    }

    #region �ɾ�Callback

    //kill�ɾ�
    private void HasPlayerDead(HumanBase killer, HumanBase player)
    {
        if (killer.IsMe && killer.IsZombie)
        {
            //achievementData.killHuman++;
            AchievementDegreeUP(AchievementType.KillTarget);
        }
    }

    //���ش����ɾ�
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

    //��Ԯ�ɾ�
    private void HasPlayerRebone(HumanBase target, HumanBase source)
    {
        if (source == PlayerControl.Instance)
        {
            AchievementDegreeUP(AchievementType.HelpTarget);
            //achievementData.helpHuman++;
        }
    }

    //���ѳɾ�
    private void HasPlayerEscape(Door door, HumanBase human)
    {
        if (human == PlayerControl.Instance)
        {
            AchievementDegreeUP(AchievementType.EscapeTarget);
        }
    }

    //��һ��ѳɾ�
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
        //Debug.Log("��Ǯ��ʾ��" + money);
    }

    //��ʯ���ѳɾ�
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

        //Debug.Log("��ʯ��ʾ��" + curGems);
    }

    //��ɫ�����ɾͣ��������������ɾͺ�������ɴ����ɾͣ�
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
    /// �ɾ���ɶ�����
    /// </summary>
    /// <param name="achievementType">��Ҫ�ж��ĳɾ�</param>
    /// <param name="UPExtent">�����̶�</param>
    private void AchievementDegreeUP(AchievementType achievementType, int UPExtent = 1)
    {
        var tem = achievementDic[achievementType.ToString()];
        tem.curNum += UPExtent;
        while (!tem.isFinished && tem.curNum >= tem.CurTarget)
        {
            /*�˴����轱��*/
            GameSetting.Coin.Value += 2;
            Debug.Log(achievementType.ToString() + "������ɽ׶�Ŀ�꣬���轱�������Ѵ��Ŀ�꣺" + tem.CurTarget);

            //�ɾ͵ȼ�����
            tem.curLevel += 1;
            if (tem.curLevel == tem.targetLevels.Count)
            {
                tem.isFinished = true;
                Debug.Log(achievementType.ToString() + "�������ȫĿ��");
            }
        }
        Debug.Log(achievementType.ToString() + "������ǰ���ȣ�" + tem.curNum + "��������ǰĿ�꣺" + tem.CurTarget);
        SaveData();
    }

    /// <summary>
    /// �ɾ��������
    /// </summary>
    public void ClearAchievement()
    {
        Debug.Log("�ɾ�ϵͳ�������");
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
