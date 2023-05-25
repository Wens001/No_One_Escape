
/****************************************************
 * FileName:		DataTest.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-07-14:19:45
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementTargetData", menuName = "new AchievementTarget")]
public class AchievementData : ScriptableObject
{
    [LabelText("成就名称")]
    public AchievementType achievemenName;
    public List<int> targetLevels;
    [LabelText("当前成就等级")]
    public int curLevel;
    /// <summary>
    /// 当前需达成的目标
    /// </summary>
    public int CurTarget
    {
        get
        {
            int index = curLevel;
            if (index >= targetLevels.Count)
            {
                index = targetLevels.Count - 1;
            }
            return targetLevels[index];
        }
    }

    [LabelText("当前已完成的数量")]
    public int curNum;

    [LabelText("是否已达成全部成就")]
    public bool isFinished;
}

public enum AchievementType
{
    KillTarget,
    HelpTarget,
    TouchButtonTarget,
    EscapeTarget,
    UpgradeFullTarget,
    UpgradeTarget,
    CoinSpentTarget,
    GemSpentTarger
}
