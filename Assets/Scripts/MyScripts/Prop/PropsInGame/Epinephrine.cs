
/****************************************************
 * FileName:		Epinephrine.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-04-14:52:08
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epinephrine : GamePropBase
{

    #region --- Public Variable ---

    #endregion


    #region --- Private Variable ---
    private float changeExtent = 2f;
    private float CDTime = 3f;
    private float durationTime = 2f;

    private MyTimer CDTimer;
    private MyTimer DurationTimer;

    private bool isDurTime = false;

    public Epinephrine(HumanBase human) : base(human)
    {

    }

    #endregion
    
    public override void Init()
    {
        CDTimer = new MyTimer(CDTime);
        CDTimer.SetFinish();

        DurationTimer = new MyTimer(durationTime);
        DurationTimer.SetFinish();
    }
    

    public override void StartUsing()
    {
        //throw new System.NotImplementedException();
        if (CDTimer.IsFinish)
        {
            DurationTimer.ReStart();
            isDurTime = true;

            Debug.Log("速度提升");
            human.ChangeSpeed(+changeExtent);
        }
    }

    protected override void OnOverUsing()
    {
        //throw new System.NotImplementedException();
        DurationTimer.SetFinish();
        isDurTime = false;
        CDTimer.ReStart();

        Debug.Log("速度回归原值");
        human.ChangeSpeed(-changeExtent);
    }

    public override void Execute()
    {
        //throw new System.NotImplementedException();
        CDTimer.OnUpdate(Time.deltaTime);
        DurationTimer.OnUpdate(Time.deltaTime);

        if (isDurTime && DurationTimer.IsFinish)
        {
            OnOverUsing();
        }

    }
}
