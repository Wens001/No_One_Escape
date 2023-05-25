
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
    [LabelText("�ɾ�����")]
    public AchievementType achievemenName;
    public List<int> targetLevels;
    [LabelText("��ǰ�ɾ͵ȼ�")]
    public int curLevel;
    /// <summary>
    /// ��ǰ���ɵ�Ŀ��
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

    [LabelText("��ǰ����ɵ�����")]
    public int curNum;

    [LabelText("�Ƿ��Ѵ��ȫ���ɾ�")]
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
