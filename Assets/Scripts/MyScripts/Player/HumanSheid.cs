
/****************************************************
 * FileName:		HumanSheid.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-07-14:30:36
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSheid : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private Transform child;
    private SkinnedMeshRenderer smr;
    private HumanBase human;

    #endregion

    private void Start()
    {
        child = transform.GetChild(0);
        child.gameObject.SetActive(false);
        transform.parent.TryGetComponent(out smr);
        smr.transform.parent.parent.TryGetComponent(out human);
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.Damage, GetDamage);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.Damage, GetDamage);
    }

    private void GetDamage(HumanBase _human)
    {
        if (human != _human)
            return;
        AudioManager.Instance.PlaySound(8);
        if (ZombieShowTimer.ZombiePlayer.modelsGroup.index == 1)
            AudioManager.Instance.PlaySound(12);
        child.parent = null;
        child.gameObject.SetActive(true);
        smr.enabled = false;
        Destroy(this);
    }


}
