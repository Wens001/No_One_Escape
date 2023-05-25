
/****************************************************
 * FileName:		EffectColors.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-08-10:40:51
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColors : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private ParticleSystem[] pss;
    private bool isinit = false;
    #endregion

    private void Awake()
    {
        if (isinit)
            return;
        isinit = true;
        pss = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        Awake();
        if (EnterScene.IsChinese.Value)
        {
            pss = GetComponentsInChildren<ParticleSystem>();
            foreach (var item in pss)
            {
                var module = item.main;
                module.startColor = Color.green;
            }
        }
    }


}
