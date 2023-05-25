
/****************************************************
 * FileName:		GuideArrow.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-01-11:30:03
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GuideArrow : MonoBehaviour
{

    #region --- Public Variable ---

    public ButtonProp target;

    #endregion


    #region --- Private Variable ---

    private Transform model;

    #endregion

    private void Awake()
    {
        model = transform.Find("Model");
        model.DOMoveY(.5f, .5f).SetLoops(-1, LoopType.Yoyo);
        Messenger.AddListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ButtonDownListener);
    }

    public void ButtonDownListener(HumanBase human,ButtonProp prop)
    {
        if (prop == target)
        {
            Messenger.RemoveListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ButtonDownListener);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        model.Rotate(0, 60 * Time.deltaTime, 0);
    }
}
