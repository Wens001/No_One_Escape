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

public class AIOpen : StateMachineBehaviour
{
    private bool IsInit = false;
    private AIControl player;
    private MyTimer updateTime;

    private void Init(Animator animator)
    {
        animator.TryGetComponent(out player);
        updateTime = new MyTimer(7);
        IsInit = true;
    }

    public int Index { get; private set; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsInit == false)
            Init(animator);

        player.SetTarget(FindDoorPos());
    }

    public Vector3 FindDoorPos()
    {
        var doors = GameManager.Instance.doors;
        int index = 0;
        float dis = Vector3.Distance(player.Position, doors[0].transform.position);
        for (int i = 1; i < doors.Length; i++)
        {
            var tdis = Vector3.Distance(player.Position, doors[i].transform.position);
            if (tdis < dis)
            {
                dis = tdis;
                index = i;
            }
        }
        return doors[index].transform.position + doors[index].dirdir.normalized * 2f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.IsStop || player.IsDead || GameManager.Speed <= 0.01f)
        {
            animator.SetBool(ConstValue.AIAnimStr.Open, false);
            return;
        }

        if (player.IsHuman && player.BeDiscovered)
        {
            animator.SetBool(ConstValue.AIAnimStr.Open, false);
            animator.SetBool(ConstValue.AIAnimStr.Escape, true);
            return;
        }

        updateTime.OnUpdate(Time.deltaTime);
        if (updateTime.IsFinish)
            animator.SetBool(ConstValue.AIAnimStr.Open, false);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        updateTime.SetFinish();
    }

}
