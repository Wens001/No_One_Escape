
/****************************************************
 * FileName:		AI_Parameter.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-14-10:43:12
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AI_Parameter : SerializedScriptableObject
{

    [BoxGroup("杀手")]
    [PropertyRange(0, 1)] [LabelText("遇到幸存者后的攻击概率")]
    public float AttackChance;

    [BoxGroup("杀手")]
    [PropertyRange(0, 1)] [LabelText("在触发按钮后的查看概率")]
    public float ExamineChance;

    [BoxGroup("杀手")]
    [PropertyRange(.05f, 1)] [LabelText("更新巡逻点概率")]
    public float ZombiePatrolRange;

    
    [BoxGroup("杀手")]
    [LabelText("丢失视野后继续追踪时间")] [MinMaxSlider(.1f, 50f)]
    public Vector2 ZombieGiveUpChaseTime;

    [BoxGroup("杀手")]
    [LabelText("刷新状态时间")] [MinMaxSlider(0.5f, 10f)]
    public Vector2 ZombieIntervalTime;


    [BoxGroup("幸存者")]
    [Range(0, 1)] [LabelText("被发现后的逃跑概率")]
    public float EscapeChance;

    [BoxGroup("幸存者")]
    [Range(.05f, 1)] [LabelText("更新巡逻点概率")]
    public float HumanPatrolRange;

    [BoxGroup("幸存者")]
    [LabelText("刷新状态时间")] [MinMaxSlider(1f, 10f)]
    public Vector2 HumanIntervalTime = new Vector2(3,8);

    [BoxGroup("幸存者")]
    [LabelText("踩开关概率")] [PropertyRange(0,1)]
    public float HumanTouchRange = .1f;

    [BoxGroup("幸存者")]
    [LabelText("逃离大门概率")][PropertyRange(0, 1)]
    public float HumanOpenRange = .2f;

}
