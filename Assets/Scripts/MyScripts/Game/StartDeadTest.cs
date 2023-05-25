
/****************************************************
 * FileName:		StartDeadTest.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-29-23:16:03
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StartDeadTest : MonoBehaviour
{

    public HumanBase[] beginSetDeadHumans;

    void Start()
    {


        this.AttachTimer(0.12f, () =>
        {
            foreach (var human in beginSetDeadHumans)
                human.PlayerDead(human);
            enabled = false;
        });
    }

    private void Update()
    {
        foreach (var human in beginSetDeadHumans)
            human.StopMove();
    }


}
