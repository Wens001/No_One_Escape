
/****************************************************
 * FileName:		HelpRebone.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-22-11:29:19
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class HelpRebone : MonoBehaviour
{

    #region --- Public Variable ---
    public float radio = 1;

    public HumanBase helpHuman { get; set; }
    public float helpTime { get; set; } = 3f;

    #endregion


    #region --- Private Variable ---
    private float lastTime;
    private MyTimer timer ;
    private MyTimer effectTimer;
    private Transform panelRoot;

    private ParticleSystem[] pss;
    private List<HumanBase> helpHumans = new List<HumanBase>();
    #endregion

    private void Awake()
    {
        panelRoot = transform.Find("panelRoot");
        panelRoot.DOScale(Vector3.one * 1.2f, .5f).SetLoops(-1, LoopType.Yoyo);

        pss = GetComponentsInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        timer = new MyTimer(helpTime);
        effectTimer = new MyTimer(.4f);
        panelRoot.gameObject.SetActive(PlayerControl.Instance.IsHuman);
    }

    private void OnDisable()
    {
        for (int i = 0; i < helpHumans.Count; i++)
        {
            helpHumans[i].helpTarget = null;
        }
        helpHumans.Clear();
    }

    private void Update()
    {
        SetLookAt();
        if (helpHuman)
            transform.position = helpHuman.Position;

        var allHumans = GameManager.Instance.humanBases;
        for (int i = 0; i < allHumans.Count; i++)
        {

            if (allHumans[i] == helpHuman || allHumans[i].IsZombie 
                || (allHumans[i].helpTarget && allHumans[i].helpTarget != helpHuman ) )
                continue;

            //¾ÈÔ®ÅÐ¶¨
            if (allHumans[i].IsDead || Vector3.Distance(transform.position, allHumans[i].Position) > radio
                ||  Physics.Raycast(transform.position + Vector3.up * .1f, allHumans[i].Position - transform.position,
                Vector3.Distance(allHumans[i].Position , transform.position), 1 << LayerMask.NameToLayer(ConstValue.LayerName.Wall)))
            {
                if (helpHumans.Contains(allHumans[i]))
                {
                    allHumans[i].helpTarget = null;
                    helpHumans.Remove(allHumans[i]);
                }
                continue;
            }
            if (!helpHumans.Contains(allHumans[i]))
            {
                allHumans[i].helpTarget = helpHuman;
                helpHumans.Add(allHumans[i]);
            }
            timer.OnUpdate(GameManager.DeltaTime * allHumans[i].p_Para.Help_BaseSpeed);
            effectTimer.ReStart();
        }

        effectTimer.OnUpdate(GameManager.DeltaTime);
        for (int i = 0; i < pss.Length; i++)
            pss[i].gameObject.SetActive(!effectTimer.IsFinish);

        //¾ÈÔ®¼ÆÊý
        if (timer.IsFinish)
        {
            helpHuman.PlayerRebone();
            if (allHumans.Contains(PlayerControl.Instance))
                Messenger.Broadcast<HumanBase, HumanBase>
                    (ConstValue.CallBackFun.PlayerRebone,helpHuman, PlayerControl.Instance);
            else
                Messenger.Broadcast<HumanBase, HumanBase>
                    (ConstValue.CallBackFun.PlayerRebone, helpHuman, allHumans[0]);
            PoolManager.ReleaseObject(gameObject);
            return;
        }
    }

    private void SetLookAt()
    {
        panelRoot.LookAt(CameraFllow.Instance.transform.position, Vector3.up);
        var angle = panelRoot.localEulerAngles;
        angle.y = 90;
        angle.z = 0;
        panelRoot.rotation = Quaternion.Euler(angle);
    }

}
