/****************************************************
 * FileName:		Door.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-13-11:18:56
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{

    #region --- Public Variable ---
    public ButtonGroup groups;
    /// <summary>
    /// 还剩多少个按钮
    /// </summary>
    public int propSize { get; private set; }

    #endregion


    #region --- Private Variable ---
    private Collider coll ;
    public Vector3 dirdir { get;private set; }
    private GameObject effects; 
    #endregion

    private void Awake()
    {
        TryGetComponent(out coll);
        var dir = transform.Find("dir");
        dir.parent = null;
        dirdir = dir.right;
        effects = transform.Find("Effects").gameObject;
        effects.transform.parent = null;
        effects.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (propSize > 0)
            return;
        if (GameManager.isWin || GameManager.isDead)
            return;
        if (!other.CompareTag(ConstValue.TagName.Player))
            return;
        other.TryGetComponent(out HumanBase human);
        if ( human == null || human.IsZombie || human.IsWin || human.IsDead )
            return;
        human.IsWin = true;
        Messenger.Broadcast<Door,HumanBase>(ConstValue.CallBackFun.PlayerGoOut, this, human);
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase,ButtonProp>(ConstValue.CallBackFun.ButtonDown, HasButtonDown);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, HasButtonDown);
    }

    void Start()
    {
        propSize = groups.bounds.Length;
    }

    /// <summary>
    /// 开门
    /// </summary>
    public void OpenDoor()
    {
        StartCoroutine(DoorDown());
    }

    private IEnumerator DoorDown()
    {
        float t=0, timer = .5f;
        for (; t < timer; )
        {
            transform.SetY(Mathf.Lerp(0,-1.1f * transform.localScale.y,t / timer));
            t += Time.deltaTime;
            yield return null;
        }
        transform.SetY(-1.1f * transform.localScale.y);
        effects.SetActive(true);
    }

    /// <summary>
    /// 有按钮被按下
    /// </summary>
    /// <param name="pb"></param>
    public void HasButtonDown(HumanBase human,ButtonProp pb)
    {
        bool isThisDoor = false;
        for (int i = 0; i < groups.bounds.Length; i++)
            if (pb == groups.bounds[i])
            {
                isThisDoor = true;
                break;
            }
        if (isThisDoor == false)
            return;
        propSize = Mathf.Clamp(propSize -1, 0, 9999);
    }


}
