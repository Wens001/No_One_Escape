
/****************************************************
 * FileName:		PropsManager.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-04-16:11:31
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropsManager : Singleton<PropsManager>
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private Dictionary<HumanBase, Dictionary<string, GamePropBase>> props = new Dictionary<HumanBase, Dictionary<string, GamePropBase>>();


    #endregion

    

    /// <summary>
    /// ʹ�õ���
    /// </summary>
    /// <param name="human">ʹ����</param>
    /// <param name="prop">����</param>
    public void UseProp(HumanBase human, PropType prop)
    {
        bool isExist = PropExist(human, prop);
        if (!isExist)
        {
            GamePropBase gameProp = null;
            switch (prop)
            {
                case PropType.Epinephrine:
                    gameProp = new Epinephrine(human);
                    break;
                case PropType.Shield:
                    gameProp = new Shield(human);
                    break;
                case PropType.SetTrap:
                    gameProp = new SetTrap(human);
                    break;
                case PropType.Radar:
                    gameProp = new Radar(human);
                    break;
                case PropType.InvisibleCloak:
                    gameProp = new InvisibleCloak(human);
                    break;
                default:
                    break;
            }

            UsingProp(human, gameProp);
        }
        else
        {
            string propName = prop.ToString();
            UsingProp(human, propName);
        }

    }

    /// <summary>
    /// ��õ���
    /// </summary>
    /// <param name="prop"></param>
    public void GetProp(PropType prop)
    {
        int num = PlayerPrefs.GetInt(prop.ToString() + "Num", 0);
        num += 1;
        PlayerPrefs.SetInt(prop.ToString() + "Num", num);
        Messenger.Broadcast(ConstValue.CallBackFun.PropNumAdd, prop);
    }

    /// <summary>
    /// ʹ�õ��ߣ��ֵ���δ���ڣ�
    /// </summary>
    /// <param name="human">ʹ����</param>
    /// <param name="prop">����</param>
    private void UsingProp(HumanBase human, GamePropBase prop)
    {
        string propName = prop.GetType().ToString();

        if (!props.ContainsKey(human))
        {
            Dictionary<string, GamePropBase> selfProp = new Dictionary<string, GamePropBase>();
            selfProp.Add(propName, prop);
            props.Add(human, selfProp);
        }
        else if (!props[human].ContainsKey(propName))
        {
            props[human].Add(propName, prop);
        }

        props[human][propName].StartUsing();
    }

    /// <summary>
    /// ʹ�õ��ߣ��ֵ����Ѵ��ڣ�
    /// </summary>
    /// <param name="human"></param>
    /// <param name="propName"></param>
    private void UsingProp(HumanBase human, string propName)
    {
        props[human][propName].StartUsing();
    }


    /// <summary>
    /// ��ֹ��������
    /// </summary>
    /// <param name="human">ʹ����</param>
    /// <param name="prop">����</param>
    public void BreakDur(HumanBase human, PropType prop)
    {
        string propName = prop.ToString();
        if (!PropExist(human, prop))
        {
            Debug.LogError(human.name + "����δʹ�õ��ߡ���" + propName);
            return;
        }
        props[human][propName].BreakOut();

    }

    /// <summary>
    /// �жϵ����Ƿ�ʹ�ù�
    /// </summary>
    /// <param name="human">ʹ����</param>
    /// <param name="prop">����</param>
    /// <returns></returns>
    public bool PropExist(HumanBase human, PropType prop)
    {
        string propName = prop.ToString();
        if (!props.ContainsKey(human) || !props[human].ContainsKey(propName))
        {
            return false;
        }
        return true;
    }

    private void Update()
    {
        //�����ֵ������еķǿյ��ߣ���ִ����ÿ֡��Ϊ
        foreach (var human in props)
        {
            if (human.Value == null)
            {
                props.Remove(human.Key);
                continue;
            }

            foreach (var porp in human.Value)
            {
                if (porp.Value == null)
                {
                    human.Value.Remove(porp.Key);
                    continue;
                }

                porp.Value.Execute();
            }

        }
    }



}



public enum PropType
{
    Epinephrine,
    Shield,
    SetTrap,
    Radar,
    InvisibleCloak
}