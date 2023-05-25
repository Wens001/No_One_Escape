
/****************************************************
 * FileName:		Trap.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-05-11:39:54
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Trap : MonoBehaviour
{
    private HumanBase master;
    private HumanBase target;
    private float trappingTime = 2f;
    private Animator animator;

    private Image trapHint;
    private Canvas canvas;

    float iconOffsetX;
    float iconOffsetY;
    float screenX;
    float screenY;

    public void Init(HumanBase human)
    {
        master = human;
        animator = GetComponent<Animator>();
        trapHint = Instantiate(Resources.Load<Image>("PosHint"));
        trapHint.gameObject.SetActive(false);


        canvas = GameObject.Find("CanvasUI").GetComponent<Canvas>();
        iconOffsetX = trapHint.GetComponent<RectTransform>().rect.width * .5f;
        iconOffsetY = trapHint.GetComponent<RectTransform>().rect.height * .5f;
        screenX = Screen.width - iconOffsetX;
        screenY = Screen.height - iconOffsetY;
    }

    private void OnTriggerEnter(Collider other)
    {
        HumanBase tem = other.GetComponent<HumanBase>();
        if (tem != null && tem.IsHuman && other.gameObject != master.gameObject)
        {
            Debug.Log("knock");
            target = tem;
            target.SetSpeedMul(0);
            target.animControl.SetMaterial(1);
            animator.SetTrigger("EnterTrap");
            OnTrapping();
        }

    }

    private void OnTrapping()
    {
        GetComponent<Collider>().enabled = false;
        int num = 0;
        DOTween.To(() => num, x => num = x, 5, trappingTime)
            .OnUpdate(() =>
            {
                //玩家，显示提示
                if (master.IsMe)
                {
                    TrapHintShow();
                }
                //AI，加入目标清单
                else
                {
                    master.targets.Add(target);
                }
            })
            .OnComplete(() =>
            {
                animator.SetTrigger("UnlockTrap");
            });
    }
    
    //打开夹子后执行，添加在夹子动画帧中
    public void UnlockTrap()
    {
        target.SetSpeedMul(1);
        target.animControl.SetMaterial(0);
        if (!master.IsMe)
        {
            master.targets.Remove(target);
        }
        Destroy(trapHint.gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// 显示位置提示
    /// </summary>
    /// <param name="targets"></param>
    private void TrapHintShow()
    {
        Vector2 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
        if (posInScreen.x < iconOffsetX || posInScreen.x > screenX || posInScreen.y < iconOffsetY || posInScreen.y > screenY)
        {
            posInScreen.x = Mathf.Clamp(posInScreen.x, 0 + iconOffsetX, screenX - iconOffsetX);
            posInScreen.y = Mathf.Clamp(posInScreen.y, 0 + iconOffsetY, screenY - iconOffsetY);
            trapHint.transform.position = posInScreen;

            trapHint.transform.Find("Ring").transform.up = (Camera.main.WorldToScreenPoint(PlayerControl.Instance.Position) - trapHint.transform.position).normalized;

            trapHint.gameObject.SetActive(true);
        }
        else
        {
            trapHint.gameObject.SetActive(false);
        }
    }
}
