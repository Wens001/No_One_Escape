
/****************************************************
 * FileName:		MainGuide.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-20-14:22:55
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Facebook.Unity;
using DG.Tweening;
using UniRx;
public class MainGuide : Singleton<MainGuide>
{
    public ReactiveProperty<int> index ;

    public List<Button> Targets;
    private List<UnityAction> TargetActions;
    private List<Transform> TargetParents;
    private List<int> TargetSiblingIndex;
    private Transform Root;
    private RectTransform HandRoot;
    private Transform circle;
    private GameObject bg;

    private void ResetButtonParent()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            Targets[i].transform.parent = TargetParents[i];
            Targets[i].transform.SetSiblingIndex(TargetSiblingIndex[i]);
        }
    }

    private void AddListener()
    {
        for (int i = 1; i < Targets.Count; i++)
        {
            Targets[i].onClick.AddListener(TargetActions[i]);
        }
    }
    private void RemoveListener()
    {
        for (int i = 1; i < Targets.Count; i++)
        {
            Targets[i].onClick.RemoveListener(TargetActions[i]);
        }
    }

    private void Awake()
    {
        index = new ReactiveProperty<int>() { Value = -1 };
        index.Subscribe(ChangeIndex);
        Root = transform.Find("Root");
        bg = transform.Find("bg").gameObject;

        HandRoot = transform.Find("HandRoot") as RectTransform;
        var hand = HandRoot.Find("hand") as RectTransform;
        hand.DOLocalMoveY(-35f,.5f).SetLoops(-1, LoopType.Yoyo);
        circle = hand.Find("circle");
        circle.DOScale(2.5f, .5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

        TargetActions = new List<UnityAction>();
        foreach (var tg in Targets)
            TargetActions.Add( () => { index.Value++; } );

        TargetParents = new List<Transform>();
        foreach (var tg in Targets)
            TargetParents.Add( tg.transform .parent );

        TargetSiblingIndex = new List<int>();
        foreach (var tg in Targets)
            TargetSiblingIndex.Add(tg.transform.GetSiblingIndex());

        
    }

    private void Start()
    {
        SetActive(false);
    }

    private void SetActive(bool flag)
    {
        bg.SetActive(flag);
        HandRoot.gameObject.SetActive(flag);
    }

    private void ChangeIndex(int _value)
    {
        if (_value < 0)
            return;

        ResetButtonParent();
        if (_value >= Targets.Count)
        {
            OnExit();
            return;
        }
        Targets[_value].gameObject.SetActive(true);
        Targets[_value].transform.parent = Root;

        HandRoot.DOKill();
        if (_value != 3)
            HandRoot.DOMove(Targets[_value].transform.position , .7f);
        else
        {
            HandRoot.DOMove(Targets[_value].transform.position + Vector3.up * 1.2f, .7f);
            HandRoot.DORotate(new Vector3(0, 0, 180), .3f);
        }
    }

    public void OnEnter()
    {
        SetActive(true);
        SceneMainUI.Instance.index.Value = 3;
        AddListener();
        var click = new Dictionary<string, object>{  ["Guide_Start"] = 1,  };
        FB.LogAppEvent("Guide", 1, click);

        index.Value = 0;
    }
    public void OnExit()
    {
        RemoveListener();
        var click = new Dictionary<string, object>{  ["Guide_Complete"] = 1, };
        FB.LogAppEvent("Guide", 1, click);
        SetActive(false);
    }

}
