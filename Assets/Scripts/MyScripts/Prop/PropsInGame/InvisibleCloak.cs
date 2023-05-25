
/****************************************************
 * FileName:		InvisibleCloak.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-05-15:19:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvisibleCloak : GamePropBase
{
    public InvisibleCloak(HumanBase human) : base(human)
    {
        List<AIControl> aiList = Object.FindObjectsOfType<AIControl>().ToList();
        foreach (var item in aiList)
        {
            if (item.IsZombie)
            {
                zombie = item;
                break;
            }
        }
    }

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---
    private float CDTime = 5f;
    private float DurTime = 3f;

    private MyTimer CDTimer;
    private MyTimer DurTimer;

    private bool isDurTime;
    private AIControl zombie;

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
            DurTimer.ReStart();
            isDurTime = true;
            
            Debug.Log("开启隐身");
            /****执行隐身方法****/
            human.TryGetComponent(out AnimControl ac);
            human.GetComponent<Collider>().enabled = false; 
            ac?.SetMaterial(0.3f);
            ac?.SetMaterialTransparent();
            if (zombie.targets.Contains(human))
            {
                zombie.targets.Remove(human);
            }
        }
    }

    public override void Execute()
    {
        //throw new System.NotImplementedException();
        CDTimer.OnUpdate(Time.deltaTime);
        DurTimer.OnUpdate(Time.deltaTime);

        if (isDurTime && DurTimer.IsFinish)
        {
            Debug.Log("到达时长");
            OnOverUsing();
            return;
        }
    }



    protected override void OnOverUsing()
    {
        //throw new System.NotImplementedException();
        DurTimer.SetFinish();
        isDurTime = false;
        CDTimer.ReStart();

        Debug.Log("退出隐身");
        /*关闭隐身方法*/
        human.TryGetComponent(out AnimControl ac);
        human.GetComponent<Collider>().enabled = true;
        ac?.SetMaterial(1);
        ac?.SetMaterialOpaque();
    }
}
