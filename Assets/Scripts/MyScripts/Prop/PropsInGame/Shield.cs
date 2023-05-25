
/****************************************************
 * FileName:		Shield.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-05-09:37:12
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : GamePropBase
{
    public Shield(HumanBase human) : base(human)
    {

    }

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private float CDTime = 3f;
    private float DurTime = 2f;

    private MyTimer CDTimer;
    private MyTimer DurTimer;

    private bool isDurTime = false;

    #endregion

    public override void Init()
    {
        //throw new System.NotImplementedException();
        CDTimer = new MyTimer(CDTime);
        CDTimer.SetFinish();

        DurTimer = new MyTimer(DurTime);
        DurTimer.SetFinish();
    }
  

    public override void StartUsing()
    {
        //throw new System.NotImplementedException();
        if (CDTimer.IsFinish)
        {
            Debug.Log("护盾开启");
            Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.WillKillPlayer, ShieldProtect);
            DurTimer.ReStart();
            isDurTime = true;
        }
    }

    public override void Execute()
    {
        //throw new System.NotImplementedException();
        CDTimer.OnUpdate(Time.deltaTime);
        DurTimer.OnUpdate(Time.deltaTime);

        if (isDurTime && DurTimer.IsFinish)
        {
            Debug.Log("护盾时长结束");
            OnOverUsing();
        }

    }

    protected override void OnOverUsing()
    {
        //throw new System.NotImplementedException();
        DurTimer.SetFinish();
        isDurTime = false;
        CDTimer.ReStart();

        Debug.Log("护盾消失");
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.WillKillPlayer, ShieldProtect);
    }


    private void ShieldProtect(HumanBase hb)
    {
        if (hb != human)
            return;
        hb.p_Para.Human_Health += 1;
        BreakOut();
        //human.animControl.SetValue(ConstValue.AnimatorStr.AttackIndex, 1f);
        //human.animControl.SetValue(ConstValue.AnimatorStr.Attack);
    }


}
