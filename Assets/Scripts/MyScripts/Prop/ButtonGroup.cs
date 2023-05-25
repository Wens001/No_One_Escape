
/****************************************************
 * FileName:		ButtonGroup.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-20-14:54:38
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using UniRx;
using System;

public class ButtonGroup : MonoBehaviour
{

    #region --- Public Variable ---
    private ButtonProp[] _bound;

    public ButtonProp[] bounds { get {
            if (IsInit == false)
            {
                _bound = GetComponentsInChildren<ButtonProp>();
                IsInit = true;
            }
            return _bound;
        } private set { bounds = value; } }

    #endregion

    private bool IsInit = false;

    private void Awake()
    {
        if (ZombieShowTimer.Instance.IsNotZomble == false)
        {
            foreach (var item in bounds)
            {
                item.transform.position -= Vector3.up * .3f;
                item.SetColl(false);
            }
        }
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, ZombieShowFunc);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, ZombieShowFunc);
    }

    public void ZombieShowFunc(HumanBase _base)
    {
        foreach (var item in bounds)
            item.transform.DOMoveY(item.transform.position.y + .3f, 2f);
        this.AttachTimer(2, () =>
        {
            foreach (var item in bounds)
                item.SetColl(true);
        });
    }


}
