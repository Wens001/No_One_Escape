
/****************************************************
 * FileName:		KillerMoveToTarget.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-12-17:25:49
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerMoveToTarget : MonoBehaviour
{

    #region --- Public Variable ---

    public HumanBase killer;
    public HumanBase target;

    public ZombieHeadEffectControl effectControl;

    public Material mat;
    #endregion


    void Start()
    {
        if ( killer == null || target == null )
            enabled = false ;
    }

    // Update is called once per frame
    void Update()
    {
        if (killer.IsZombie)
        {
            var li = killer.targets;
            if (!li.Contains(target))
                li.Add(target);
            effectControl.enabled = false;
            effectControl.UpdatePosition();
            effectControl.SetMesh(1);
        }

        if (mat)
        {
            mat.SetColor("_BaseColor", Color.green);
        }
    }
}
