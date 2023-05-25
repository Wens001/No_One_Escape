/****************************************************
 * FileName:		PropBase.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-04-16:13:41
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePropBase
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    protected HumanBase human;

    public GamePropBase(HumanBase human)
    {
        this.human = human;
        Init();
    }

    #endregion

    public abstract void Init();

    public abstract void StartUsing();


    public virtual void BreakOut()
    {
        Debug.Log(this.GetType().Name + "¡£¡£¼¼ÄÜÖÐ¶Ï");
        OnOverUsing();
    }

    protected abstract void OnOverUsing();

    public abstract void Execute();


}
