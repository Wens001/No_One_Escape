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

public class AITouch : StateMachineBehaviour
{
    private bool IsInit = false;
    private AIControl player;


    private void Init(Animator animator)
    {
        animator.TryGetComponent(out player);
        IsInit = true;
    }

    public int Index { get; private set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (IsInit == false)
            Init(animator);
        Index = FindNearButtonIndex();
        if (Index != -1)
            player.SetTarget(GetPos(Index));

    }


    public Vector3 GetPos(int index)
    {
        if (index == -1)
            return Vector3.one * 10000;
        return GameManager.Instance.ButtonProps[index].transform.position;
    }

    public bool IsTouch(int index)
    {
        if (index <= -1)
            return false;
        return GameManager.Instance.ButtonProps[index].isTouch;
    }

    public int FindNearButtonIndex()
    {
        var Props = GameManager.Instance.ButtonProps;
        var index = -1;
        var dis = 9999f;
        var pos = Vector3.one * 10000;
        for (int i = 0; i < Props.Length; i++)
        {
            if (!Props[i].isTouch)
            {
                var tdis = Vector3.Distance(player.Position, Props[i].transform.position);
                if (tdis < dis)
                {
                    index = i;
                    dis = tdis;
                }
            }
        }
        return index;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.IsStop || player.IsDead || GameManager.Speed <= 0.01f)
        {
            animator.SetBool(ConstValue.AIAnimStr.Touch, false);
            return;
        }

        if (player.IsHuman && player.BeDiscovered)
        {
            animator.SetBool(ConstValue.AIAnimStr.Touch, false);
            animator.SetBool(ConstValue.AIAnimStr.Escape, true);
            return;
        }

        if (GameManager.Instance.ButtonSize <= 0 || IsTouch(Index))
            animator.SetBool(ConstValue.AIAnimStr.Touch, false);
    }

}
