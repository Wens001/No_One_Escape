/****************************************************
 * FileName:		Hook.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-27-15:08:15
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{

    #region --- Public Variable ---

    public Animator pirateAni;
    public Shooter shooter;

    #endregion


    #region --- Private Variable ---
    private float moveDurTime;
    private Vector3 initPos;
    private Vector3 endPos;
    private GameObject curTarget;
    private bool isAtking;
    private HumanBase human;
    private Hook initHook;


    #endregion
    private void Awake()
    {
        if (!shooter)
        {
            shooter = transform.GetComponentInParent<Shooter>();
        }
        if (!shooter)
        {
            Debug.LogError("Error");
        }
    }

    /// <summary>
    /// 生成一个新的钩子去执行技能操作，需在此将所有用到的参数变量赋值过去
    /// </summary>
    /// <param name="human"></param>
    /// <param name="hookLength"></param>
    /// <param name="dir"></param>
    /// <param name="atkDurTime"></param>
    public void LaunchSkill(HumanBase human, float hookLength, Vector3 dir, float atkDurTime)
    {
        initPos = transform.position + transform.forward * 0.2f;
        moveDurTime = atkDurTime * .5f;
        this.gameObject.SetActive(false);

        //复制钩子去移动，保证钩子回来后的位置不变
        GameObject obj = Instantiate(this.gameObject, initPos, transform.rotation);
        Hook temHook = obj.GetComponent<Hook>();
        temHook.human = human;
        temHook.isAtking = true;
        temHook.initPos = this.initPos;
        temHook.endPos = transform.position + dir.normalized * hookLength;
        temHook.moveDurTime = atkDurTime * .5f;
        temHook.initHook = this;
        temHook.shooter = this.shooter;

        obj.gameObject.SetActive(true);
        temHook.ShootMoveHook(moveDurTime);
        human.animControl.SetValue("LaunchSkill");

        //this.human = human;
        //isAtking = true;
        //initPos = transform.position;
        //endPos = transform.position + dir.normalized * hookLength;
        //moveDurTime = atkDurTime * .5f;

    }

    #region 钩子复制体的移动
    /// <summary>
    /// 钩子射向目标时的移动（复制体）
    /// </summary>
    /// <param name="durTime">值越小，飞行速度越快</param>
    void ShootMoveHook(float durTime)
    {
        shooter.InitData(this);
        Tween shootTween = DOTween.To(() => transform.position, x => transform.position = x, endPos, durTime)
            .OnComplete(() => BackMoveHook(durTime));

    }

    /// <summary>
    /// 钩子返回时的移动
    /// </summary>
    /// <param name="durTime">值越小，飞行速度越快</param>
    void BackMoveHook(float durTime)
    {
        Debug.Log("钩子返回。。。");
        shooter.BackHook();
        Tween backTween = DOTween.To(() => transform.position, x => transform.position = x, initPos, durTime)
            .OnUpdate(()=> {
                if (curTarget != null)
                {
                    //curTarget.transform.position = transform.position;
                    curTarget.transform.position = new Vector3(transform.position.x, curTarget.transform.position.y, transform.position.z);
                }
            })
            .OnComplete(()=> {
                curTarget = null;
                isAtking = false;

                human.SetSpeedMul(1);
                human.SetStop(false);
                
                pirateAni.SetTrigger("ExitMotion");
                if (initHook)
                {
                    initHook.gameObject.SetActive(true);
                    Destroy(this.gameObject);
                }
                Debug.Log("技能结束。。。。。。");
                shooter.OverShoot();
            });

    }

    //攻击中，碰到物体，即刻返回（碰到角色钩住他）
    private void OnTriggerEnter(Collider other)
    {
        if (isAtking)
        {
            Debug.Log("技能发动，碰到。。" + other.name);
            if (other.CompareTag("Player") && other.GetComponent<HumanBase>() != human)
            {
                curTarget = other.gameObject;
            }

            DOTween.KillAll();

            float curRate = Vector3.Distance(transform.position, initPos) / Vector3.Distance(transform.position, endPos);
            float hookBackTime = moveDurTime * curRate;
            BackMoveHook(hookBackTime);
        }
        
    }

    #endregion
}
