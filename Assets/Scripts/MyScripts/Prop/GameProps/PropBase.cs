
/****************************************************
 * FileName:		PropBase.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-24-10:21:20
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PropBase :MonoBehaviour
{
    public PlayerType TgType = PlayerType.All;
    virtual public void PlayAction(HumanBase human) { }


    protected Collider coll;
    protected Transform model;
    virtual protected void Awake()
    {
        model = transform.Find("in");
        TryGetComponent(out coll);
    }

    /// <summary>
    /// ×Ô¶¨ÒåÅÐ¶Ï
    /// </summary>
    /// <returns></returns>
    virtual protected bool CustonBool(HumanBase human)
    {
        return true;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstValue.TagName.Player))
        {
            other.TryGetComponent(out HumanBase humanBase);
            if (humanBase && !humanBase.IsDead 
                && ((int)humanBase.playerType & (int)TgType) != 0 
                && CustonBool(humanBase))
            {
                PlayAction(humanBase);
                coll.enabled = false;
            }
        }
    }

    public bool isTouch
    {
        get
        {
            if (coll == null)   TryGetComponent(out coll);
            return !coll.enabled;
        }
    }
}
