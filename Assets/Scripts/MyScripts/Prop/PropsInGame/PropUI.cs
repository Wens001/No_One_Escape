
/****************************************************
 * FileName:		PropUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-12-16:10:38
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropUI : MonoBehaviour
{

    #region --- Public Variable ---
    public PropType propName;
    public Text numText;

    #endregion


    #region --- Private Variable ---
    private int num;


    #endregion

    private void OnEnable()
    {
        num = PlayerPrefs.GetInt(propName.ToString() + "Num", 0);
        PropNumShow();
        Messenger.AddListener<PropType>(ConstValue.CallBackFun.PropNumAdd, OnPropNumAdd);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<PropType>(ConstValue.CallBackFun.PropNumAdd, OnPropNumAdd);
    }

    public void OnPropUIClick()
    {
        if (num > 0)
        {
            num -= 1;
            PropsManager.Instance.UseProp(PlayerControl.Instance, propName);
            PlayerPrefs.SetInt(propName.ToString() + "Num", num);
            PropNumShow();
        }
    }

    public void OnPropNumAdd(PropType propType)
    {
        if (propType == propName)
        {
            num = PlayerPrefs.GetInt(propName.ToString() + "Num", 0);
            PropNumShow();
        }
    }

    private void PropNumShow()
    {
        numText.text = num.ToString();
    }

}
