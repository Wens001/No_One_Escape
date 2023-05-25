
/****************************************************
 * FileName:		SetTrap.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-05-11:27:53
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTrap : GamePropBase
{
    public SetTrap(HumanBase human) : base(human)
    {
    }

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---
    private float CDTime = 2f;

    private MyTimer CDTimer;
    
    private Trap trap;

    #endregion


    public override void Init()
    {
        //throw new System.NotImplementedException();
        CDTimer = new MyTimer(CDTime);
        CDTimer.SetFinish();
        
        trap = Resources.Load<Trap>("Trap");
    }

    public override void StartUsing()
    {
        //throw new System.NotImplementedException();
        if (CDTimer.IsFinish)
        {
            CDTimer.ReStart();

            Vector3 pos = new Vector3(human.transform.position.x, 0, human.transform.position.z);
            Trap tem = Transform.Instantiate(trap, pos, Quaternion.identity);
            tem.Init(human);
            
        }

    }


    public override void Execute()
    {
        //throw new System.NotImplementedException();
        CDTimer.OnUpdate(Time.deltaTime);
    }
    

    protected override void OnOverUsing()
    {
        //throw new System.NotImplementedException();
        Debug.Log(this.GetType().ToString() + "。。无需结束释放");
    }
}
