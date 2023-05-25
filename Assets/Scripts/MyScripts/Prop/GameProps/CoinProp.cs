
/****************************************************
 * FileName:		CoinProp.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-24-11:06:34
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinProp : PropBase
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---



    #endregion

    override protected  bool CustonBool(HumanBase human)
    {
        return human == PlayerControl.Instance;
    }

    override public void PlayAction(HumanBase human)
    {
        Messenger.Broadcast<int>(ConstValue.CallBackFun.AddMoney, 1);
        if (human == PlayerControl.Instance)
            GameManager.Instance.ShowCoinEffect(transform, 5);
    }
}
