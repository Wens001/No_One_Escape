
/****************************************************
 * FileName:		EnvirmentDebug.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-23-15:44:59
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class EnvirmentDebug : MonoBehaviour
{
 
    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---



    #endregion



    void Start()
    {
        CheckPassword.IsClearBG.Subscribe( a=> gameObject.SetActive(!a)).AddTo(this);
    }

}
