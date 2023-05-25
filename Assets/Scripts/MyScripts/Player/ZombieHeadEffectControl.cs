
/****************************************************
 * FileName:		ZombieHeadEffectControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-27-15:48:04
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZombieHeadEffectControl : MonoBehaviour
{

    #region --- Public Variable ---
    public GameObject StepPrefab;
    public float HeightMul = 1.5f;
    public float Radius = 3;

    private GameObject wenhao, gantan;

    #endregion


    #region --- Private Variable ---
    private List<MyTimer> humanTimer = new List<MyTimer>();
    private const float deltaShow = 2f;
    #endregion

    void Awake()
    {
        wenhao = transform.Find("wenhao").gameObject;
        gantan = transform.Find("gantan").gameObject;
    }

    private void Start()
    {
        SetMesh(-1);
        this.AttachTimer(.1f, () => {
            for (int i = 0; i < GameManager.Instance.humanBases.Count; i++)
                humanTimer.Add(new MyTimer(deltaShow));
        });
        audioDelty.SetFinish();
    }

    private MyTimer audioDelty = new MyTimer(5);
    private MyTimer clearMeshTime = new MyTimer(1f);
    private SignedTimer signed = new SignedTimer();

    void Update()
    {
        var zombie = ZombieShowTimer.ZombiePlayer;
        if (!ZombieShowTimer.HasZombie)
            return;

        UpdatePosition();

        signed.OnUpdate(zombie.targets.Count > 0);
        audioDelty.OnUpdate(GameManager.DeltaTime);
        //视野内有幸存者
        if (signed.IsPressDown)
        {
            SetMesh(1);
            if (audioDelty.IsFinish)
            {
                AudioManager.Instance.PlaySound(7);
                audioDelty.ReStart();
            }
            clearMeshTime.ReStart();
        }
        //幸存者消失
        if (signed.IsPressUp)
        {
            SetMesh(0);
            clearMeshTime.ReStart();
        }
        clearMeshTime.OnUpdate(GameManager.DeltaTime);
        if (clearMeshTime.IsFinish)
            SetMesh(-1);


        //  脚步的提示特效
        if (zombie.IsMe)
            for (int i = 0; i < humanTimer.Count; i++)
            {
                var hb = GameManager.Instance.humanBases[i];
                if (hb == zombie || hb.IsDead)
                    continue;
                if (Vector3.Distance(zombie.Position, hb.Position) < Radius)
                {
                    humanTimer[i].OnUpdate(Time.deltaTime);
                    if (humanTimer[i].IsFinish)
                    {
                        humanTimer[i].ReStart();
                        var go = PoolManager.SpawnObject(StepPrefab);
                        go.transform.parent = hb.transform;
                        go.transform.localPosition = Vector3.up * .04f;
                        go.transform.rotation = Quaternion.identity ;
                        go.transform.localScale = Vector3.one ;
                        this.AttachTimer(deltaShow-.02f, () => { PoolManager.ReleaseObject(go); });
                    }
                }
            }
    }

    public void UpdatePosition()
    {
        var pos = ZombieShowTimer.ZombiePlayer.Position + Vector3.up * HeightMul;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void SetMesh(int index)
    {
        switch (index)
        {
            case -1:
                wenhao.SetActive(false);
                gantan.SetActive(false);
                break;
            case 0:
                wenhao.SetActive(true);
                gantan.SetActive(false);
                break;
            case 1:
                wenhao.SetActive(false);
                gantan.SetActive(true);
                break;
        }
    }

}
