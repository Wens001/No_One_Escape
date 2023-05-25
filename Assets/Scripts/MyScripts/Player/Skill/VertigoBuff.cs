
/****************************************************
 * FileName:		VertigoBuff.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-01-14:50:18
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertigoBuff : BuffBase
{
    public float Speed = 60;
    // Update is called once per frame
    protected override void Update()
    {
        target.SetStop(true);
        base.Update();
    }

    public override void OnRelease()
    {
        target.SetStop(false);
        base.OnRelease();
    }

}


