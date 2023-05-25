
/****************************************************
 * FileName:		Radar.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-05-15:08:38
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Radar : GamePropBase
{
    public Radar(HumanBase human) : base(human)
    {
    }

    private float CDTime = 2f;
    private float DurTime = 4f;

    private MyTimer CDTimer;
    private MyTimer DurTimer;

    private bool isDurTime;


    private List<HumanBase> humen;

    private GameObject effect;
    private Image posHint;
    private List<Image> posHints = new List<Image>();
    private Canvas canvas;

    float iconOffsetX;
    float iconOffsetY;
    float screenX;
    float screenY;

    public override void Init()
    {
        //throw new System.NotImplementedException();
        CDTimer = new MyTimer(CDTime);
        CDTimer.SetFinish();

        DurTimer = new MyTimer(DurTime);
        DurTimer.SetFinish();

        effect = Resources.Load<GameObject>("PlayerEffect");
        posHint = Resources.Load<Image>("PosHint");
        canvas = GameObject.Find("CanvasUI").GetComponent<Canvas>();

        humen = Object.FindObjectsOfType<HumanBase>().ToList();
        for (int i = 0; i < humen.Count; i++)
        {
            if (humen[i].gameObject == human.gameObject)
            {
                humen.RemoveAt(i);
                --i;
                continue;
            }

            Image temHint = Object.Instantiate(posHint, canvas.transform);
            temHint.gameObject.SetActive(false);
            posHints.Add(temHint);
        }

        iconOffsetX = posHint.GetComponent<RectTransform>().rect.width * .5f;
        iconOffsetY = posHint.GetComponent<RectTransform>().rect.height * .5f;
        screenX = Screen.width - iconOffsetX;
        screenY = Screen.height - iconOffsetY;
    }
    

    public override void StartUsing()
    {
        //throw new System.NotImplementedException();
        
        if (CDTimer.IsFinish)
        {
            /****播放雷达动画****/
            GameObject tem = GameObject.Instantiate(effect, human.transform);
            tem.transform.localScale.Set(0.1f, 1f, 0.1f);

            Debug.Log("开始扫描");
            float scale = 0.1f;
            DOTween.To(()=> scale, x => scale= x, 20f, 1.5f)
                .OnUpdate(()=> {
                    tem.transform.localScale = new Vector3(scale, 1f, scale);
                })
                .OnComplete(()=> {
                    
                    //正式开启技能
                    Debug.Log("扫描结束");
                    DurTimer.ReStart();
                    isDurTime = true;
                    
                    Object.Destroy(tem.gameObject);
                    if (human.IsMe)
                    {
                        /******关闭迷雾******/
                        FogControl(false);
                    }
                });


        }
    }

    public override void Execute()
    {
        //throw new System.NotImplementedException();
        CDTimer.OnUpdate(Time.deltaTime);
        DurTimer.OnUpdate(Time.deltaTime);

        if (isDurTime)
        {
            if (DurTimer.IsFinish)
            {
                Debug.Log("雷达扫描结束");
                OnOverUsing();
                return;
            }
            else
            {
                //AI 追击
                if (!human.IsMe)
                {
                    int index = AIChase.FindNearIndex(human.transform.position, humen);
                    if (index != -1)
                    {
                        var lastPos = humen[index].Position;
                        human.SetTarget(lastPos);
                    }
                }
                //玩家 提示方位
                else
                {
                    PosHintShow(humen);
                }
            }
        }
    }

   

    protected override void OnOverUsing()
    {
        //throw new System.NotImplementedException();
        DurTimer.SetFinish();
        isDurTime = false;
        CDTimer.ReStart();

        Debug.Log("雷达关闭");
        PosHintHide();

        /*开启迷雾*/
        FogControl(true);
    }

    /// <summary>
    /// 显示位置提示
    /// </summary>
    /// <param name="targets"></param>
    private void PosHintShow(List<HumanBase> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].IsDead)
            {
                posHints[i].gameObject.SetActive(false);
                continue;
            }

            Vector2 posInScreen = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            if (posInScreen.x < iconOffsetX || posInScreen.x > screenX || posInScreen.y < iconOffsetY || posInScreen.y > screenY)
            {
                posInScreen.x = Mathf.Clamp(posInScreen.x, 0 + iconOffsetX, screenX - iconOffsetX);
                posInScreen.y = Mathf.Clamp(posInScreen.y, 0 + iconOffsetY, screenY - iconOffsetY);
                posHints[i].transform.position = posInScreen;
                
                posHints[i].transform.Find("Ring").transform.up = (Camera.main.WorldToScreenPoint(human.Position) - posHints[i].transform.position).normalized;
                
                posHints[i].gameObject.SetActive(true);
            }
            else
            {
                posHints[i].gameObject.SetActive(false);
            }

        }
    }

    /// <summary>
    /// 取消位置提示
    /// </summary>
    private void PosHintHide()
    {
        for (int i = 0; i < posHints.Count; i++)
        {
            posHints[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 迷雾（隐藏）控制
    /// </summary>
    /// <param name="open">true隐藏</param>
    private void FogControl(bool open)
    {
        foreach (var tem in humen)
        {
            if (tem == null || tem.IsDead)
                continue;
            tem.TryGetComponent(out AnimControl ac);

            if (open)
            {
                ac?.SetMaterial(0);
            }
            else
            {
                ac?.SetMaterial(1);
            }
        }

    }

}
