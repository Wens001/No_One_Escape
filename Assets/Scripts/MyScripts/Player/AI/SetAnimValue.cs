/****************************************************
 * FileName:		AIStroll.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-13-10:20:45
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SetAnimValue : StateMachineBehaviour
{
    [System.Serializable]
    public class Value
    {
        public string a;
        public bool b;
    }

    public Value value;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetFloat(ConstValue.AnimatorStr.AttackIndex)<.1f)
            animator.SetBool(value.a, value.b);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
