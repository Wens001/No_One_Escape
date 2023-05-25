/****************************************************
 * FileName:		Player_Parameter.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-22-11:12:07
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Player_Parameter : ScriptableObject
{
    [Space(10)]
    //杀手
    [TabGroup("杀手")] [LabelText("移动速度")] [Range(0,4)] public float Kill_BaseSpeed = 2.6f;
    [TabGroup("杀手")] [LabelText("攻击距离")] [Range(0, 2)] public float BaseAttackDistance = .7f;
    [TabGroup("杀手")] [LabelText("攻击间隔时间")] [Range(0, 2)] public float BaseAttackDeltaTime = 1.5f;
    [TabGroup("杀手")] [LabelText("攻击角度范围")] [Range(0, 90)] public float BaseAttackAngle = 30f;
    [TabGroup("杀手")] [LabelText("视野角度")] [Range(0, 360)] public float BaseViewAngle = 60f;
    [TabGroup("杀手")] [LabelText("最大视野半径")] [Range(0, 10)] public float BaseMaxViewRadius = 5f;
    [TabGroup("杀手")] [LabelText("最小视野半径")] [Range(0, 10)] public float BaseMinViewRadius = 1.2f;
    //幸存者
    [TabGroup("幸存者")] [LabelText("生命")] [Range(1, 4)] public int Human_Health = 1;
    [TabGroup("幸存者")] [LabelText("移动速度")] [Range(0, 4)] public float Human_BaseSpeed = 2.6f;
    [TabGroup("幸存者")] [LabelText("救援速度")] [Range(0, 4)] public float Help_BaseSpeed = 1;

    public void CopyValue(Player_Parameter player_)
    {
        this.Kill_BaseSpeed = player_.Kill_BaseSpeed;
        this.BaseAttackDistance = player_.BaseAttackDistance;
        this.BaseAttackDeltaTime = player_.BaseAttackDeltaTime;
        this.BaseAttackAngle = player_.BaseAttackAngle;
        this.Human_Health = player_.Human_Health;
        this.Human_BaseSpeed = player_.Human_BaseSpeed;
        this.Help_BaseSpeed = player_.Help_BaseSpeed;
        this.BaseViewAngle = player_.BaseViewAngle;
        this.BaseMaxViewRadius = player_.BaseMaxViewRadius;
        this.BaseMinViewRadius = player_.BaseMinViewRadius;
    }

    public Player_Parameter() { }

    public void Reset()
    {
        this.Kill_BaseSpeed = 0;
        this.BaseAttackDistance = 0;
        this.BaseAttackDeltaTime = 0;
        this.BaseAttackAngle = 0;
        this.Human_Health = 0;
        this.Human_BaseSpeed = 0;
        this.Help_BaseSpeed = 0;
        this.BaseViewAngle = 0;
        this.BaseMaxViewRadius = 0;
        this.BaseMinViewRadius = 0;
    }


}
