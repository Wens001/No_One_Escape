
/****************************************************
 * FileName:		BuffBase.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-01-14:53:32
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase : MonoBehaviour
{
    private float timer;

    public HumanBase target;
    public float DurationTime = 3;


    protected virtual void Update()
    {
        timer += GameManager.DeltaTime;
        if (timer >= DurationTime)
            OnRelease();
    }

    public virtual void OnRelease()
    {
        Destroy(gameObject);
    }

}
