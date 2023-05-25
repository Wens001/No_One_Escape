
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

public class AIChase : StateMachineBehaviour
{
    private bool IsInit = false;
    private AIControl player;
    private AnimControl animControl;
    private MyTimer updateTime;

    private void Init(Animator animator)
    {
        animator.TryGetComponent(out player);
        animator.TryGetComponent(out animControl);
        updateTime = new MyTimer(Random.Range(player.aiParameter.ZombieGiveUpChaseTime.x,
            player.aiParameter.ZombieGiveUpChaseTime.y));
        
        
        IsInit = true;
    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsInit == false)
            Init(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.IsStop || player.IsDead || GameManager.Speed < 0.01f)
        {
            animator.SetBool(ConstValue.AIAnimStr.Chase, false);
            return;
        }
        //存在攻击目标
        if (player.targets.Count != 0)
        {
            updateTime.ReStart();
        }
        else
        {
            updateTime.OnUpdate(GameManager.DeltaTime);
            if (updateTime.IsFinish)
            {
                animator.SetBool(ConstValue.AIAnimStr.Chase, false);
            }
        }
    }

    public static int FindNearIndex(Vector3 pos,List<HumanBase> targets)
    {
        int index = -1;
        var dis = 999f;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].IsDead)
                continue;
            var tdis = Vector3.Distance(targets[i].Position, pos);
            if (tdis < 0.1f )
                continue;
            if (tdis < dis)
            {
                dis = tdis;
                index = i;
            }
            
        }
        return index;
    }

    public static bool IsInRange(Vector3 pos,Vector3 point ,float value)
    {
        if (Vector3.Distance(pos, point) < value)
            return true;
        return false;
    }

    public static float DirAngle(Vector3 forward, Vector3 dir)
    {
        var t = Vector3.Angle(forward, dir);
        if (t < 0)
            t += 180;
        if (t > 180)
            t -= 180;
        return t;
    }

    public static bool IsInAngle(Vector3 forward, Vector3 dir, float angle)
    {
        return DirAngle(forward, dir) <= angle;
    }

}
