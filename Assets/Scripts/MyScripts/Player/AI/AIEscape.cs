/****************************************************
 * FileName:		AIChase.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-15-10:08:15
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		AI׷Ѱ
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//幸存者逃跑
public class AIEscape : StateMachineBehaviour
{
    private bool IsInit = false;
    private AIControl player;
    private HumanBase humanBase;
    private MyTimer delayTimer;

    private void Init(Animator animator)
    {
        animator.TryGetComponent(out humanBase);
        animator.TryGetComponent(out player);
        delayTimer = new MyTimer(1.5f);
        IsInit = true;
    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsInit == false)
            Init(animator);
        targetPoint = player.Position;
        player.StopMove();
    }

    private Vector3 targetPoint;
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.IsStop || GameManager.Speed < 0.01f)
            return;

        if (player.IsDead)
        {
            ExitState(animator);
            return;
        }
        if (player.IsWalk == false)
        {
            var dir = (humanBase.Position - ZombieShowTimer.ZombiePlayer.Position).normalized;
            targetPoint = humanBase.Position + dir * 2f;
            player.SetTarget(player.GetRamdomPos(targetPoint, 1f));
        }
        //自身是人类且被发现
        if (humanBase.BeDiscovered)
            delayTimer.ReStart();
        delayTimer.OnUpdate(GameManager.DeltaTime);
        if (delayTimer.IsFinish)
            ExitState(animator);
    }

    private void ExitState(Animator animator)
    {
        animator.SetBool(ConstValue.AIAnimStr.Escape, false);
    }

}
