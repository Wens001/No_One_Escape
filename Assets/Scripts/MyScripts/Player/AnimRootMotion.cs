
/****************************************************
 * FileName:		AnimRootMotion.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-22-00:48:21
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimRootMotion : MonoBehaviour
{

    #region --- Public Variable ---

    public float closeNavTime = .5f;
    #endregion


    #region --- Private Variable ---

    private Animator anim;
    private HumanBase humanBase;
    private MyTimer timer;
    private SignedTimer deadSigned = new SignedTimer();
    #endregion


    private void Awake()
    {
        TryGetComponent(out anim);
        
        timer = new MyTimer(closeNavTime);

        anim.SetLayerWeight(0, .5f);
    }

    private void Start()
    {
        transform.parent.TryGetComponent(out humanBase);
    }

    bool canAnimMove = true;
    private void OnAnimatorMove()
    {
        bool isdead = anim.GetBool(ConstValue.AnimatorStr.Dead);
        deadSigned.OnUpdate(isdead);
        if (deadSigned.IsPressDown)
        {
            timer.ReStart();
            canAnimMove = true;
        }
        if (isdead)
        {
            if (Physics.Raycast(humanBase.Position, humanBase.transform.forward, .7f, 1 << LayerMask.NameToLayer(ConstValue.LayerName.Wall)))
            {
                humanBase.Move(-anim.deltaPosition);
                canAnimMove = false;
            }
            else if (canAnimMove)
                humanBase.Move(anim.deltaPosition);
            timer.OnUpdate(Time.deltaTime);
            if (timer.IsFinish)
                humanBase.GetAgent().enabled = false;
        }
        if (deadSigned.IsPressUp)
        {
            humanBase.GetAgent().enabled = true;
        }
    }

}
