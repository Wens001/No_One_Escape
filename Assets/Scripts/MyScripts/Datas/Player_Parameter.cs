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
    //ɱ��
    [TabGroup("ɱ��")] [LabelText("�ƶ��ٶ�")] [Range(0,4)] public float Kill_BaseSpeed = 2.6f;
    [TabGroup("ɱ��")] [LabelText("��������")] [Range(0, 2)] public float BaseAttackDistance = .7f;
    [TabGroup("ɱ��")] [LabelText("�������ʱ��")] [Range(0, 2)] public float BaseAttackDeltaTime = 1.5f;
    [TabGroup("ɱ��")] [LabelText("�����Ƕȷ�Χ")] [Range(0, 90)] public float BaseAttackAngle = 30f;
    [TabGroup("ɱ��")] [LabelText("��Ұ�Ƕ�")] [Range(0, 360)] public float BaseViewAngle = 60f;
    [TabGroup("ɱ��")] [LabelText("�����Ұ�뾶")] [Range(0, 10)] public float BaseMaxViewRadius = 5f;
    [TabGroup("ɱ��")] [LabelText("��С��Ұ�뾶")] [Range(0, 10)] public float BaseMinViewRadius = 1.2f;
    //�Ҵ���
    [TabGroup("�Ҵ���")] [LabelText("����")] [Range(1, 4)] public int Human_Health = 1;
    [TabGroup("�Ҵ���")] [LabelText("�ƶ��ٶ�")] [Range(0, 4)] public float Human_BaseSpeed = 2.6f;
    [TabGroup("�Ҵ���")] [LabelText("��Ԯ�ٶ�")] [Range(0, 4)] public float Help_BaseSpeed = 1;

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
