
/****************************************************
 * FileName:		AIMoveToTarget.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-11-17:01:40
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StateReverse : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var mirror = animator.GetBool(ConstValue.AnimatorStr.Mirror);
        animator.SetBool(ConstValue.AnimatorStr.Mirror,!mirror);
    }


}
