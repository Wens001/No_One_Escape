
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

public class AIStroll : StateMachineBehaviour
{
    private bool IsInit = false;
    private AIControl player;
    private MyTimer updateTime;

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
    }


    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
    }

    private void ListenerDoor(HumanBase human ,ButtonProp bp)
    {
        if(player && player.IsZombie)
        {
            player.GetAgent().SetDestination(bp.transform.position);
        }
    }

    private void Init(Animator animator)
    {
        animator.TryGetComponent(out player);
        updateTime = new MyTimer(Random.Range(2f, 3f));
        updateTime.SetFinish();
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
        if (player.IsStop || player.IsDead || GameManager.Speed <= 0.01f)
        {
            player.StopMove();
            return;
        }
        //存在丧尸
        if (ZombieShowTimer.HasZombie)
        {
            //自身是丧尸
            if (player.playerType == PlayerType.Zombie)
            {
                //丧尸有目标,激活攻击模式
                if (ZombieShowTimer.ZombiePlayer.targets.Count != 0)
                {
                    animator.SetBool(ConstValue.AIAnimStr.Chase, true);
                    PropOnce();
                    return;
                }
                else
                {
                    once = true;
                }
            }
            //自身是人类
            if (player.playerType == PlayerType.Human )
            {
                //被丧尸发现，则激活逃跑
                if (player.BeDiscovered && !player.IsDead)
                {
                    animator.SetBool(ConstValue.AIAnimStr.Escape, true);
                    PropOnce();
                    return;
                }
                else
                {
                    once = true;
                }
            }
        }
        updateTime.OnUpdate(GameManager.DeltaTime);
        if (updateTime.IsFinish)
        {
            updateTime.ReStart();

            if (ZombieShowTimer.HasZombie && player.IsHuman)
            {
                if (GameManager.Instance.ButtonSize > 0)
                {
                    var value = Random.Range(0, 1f);
                    if (value <= player.aiParameter.HumanTouchRange)
                    {
                        animator.SetBool(ConstValue.AIAnimStr.Touch, true);
                        animator.SetBool(ConstValue.AIAnimStr.Open, false);
                        return;
                    }
                }

                {
                    var value = Random.Range(0, 1f);
                    if (value <= player.aiParameter.HumanOpenRange)
                    {
                        animator.SetBool(ConstValue.AIAnimStr.Open, true);
                        animator.SetBool(ConstValue.AIAnimStr.Touch, false);
                        return;
                    }
                }
            }



            player.SetTarget(player.GetRamdomPos(player.Position , 4f));
        }

    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        updateTime.SetFinish();
    }

    private bool once = true;

    private void PropOnce()
    {
        if (once && player.hasProp)
        {
            once = false;
            int num = Random.Range(0, 4);
            if (num == 0)
            {
                PropsManager.Instance.UseProp(player, player.propName);
                player.hasProp = false;
            }
        }

    }

}
