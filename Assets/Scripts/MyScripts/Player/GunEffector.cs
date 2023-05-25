
/****************************************************
 * FileName:		GunEffector.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-13-22:02:14
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffector : MonoBehaviour
{
    #region --- Private Variable ---

    private ParticleSystem[] systems;
    private HumanBase player;
    #endregion


    private bool isinit = false;
    void Awake()
    {
        if (isinit)
            return;
        systems = transform.GetComponentsInChildren<ParticleSystem>();
        isinit = true;
    }

    private void Start()
    {
        player = transform.parent.parent.parent.GetComponent<HumanBase>();
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.PlayEffect, Play);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.PlayEffect, Play);
    }

    public void Play(HumanBase human)
    {
        if (human != player)
            return;
        Awake();
        foreach (var sys in systems)
        {
            sys.time = 0;
            sys.Play();
        }
    }

}
