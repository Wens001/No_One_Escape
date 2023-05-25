
/****************************************************
 * FileName:		ZombieShowTimer.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-13-14:53:33
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UniRx;
using System;

public class ZombieShowTimer : Singleton<ZombieShowTimer>
{

    #region --- Public Variable ---

    public Light mainLight;
    public float ShowTime = 3;


    public GameObject[] ZombieEffects;
    public bool SetZombieIsPlayer { get; set; }
    public bool SetZombieIsNotPlayer { get; set; }
    public bool IsNotZomble = false;
    public MyTimer ZombieTimer { get; private set; }
    public MyTimer GameTimer { get; private set; }
    public static HumanBase ZombiePlayer { get; private set; } = null;

    public static bool HasZombie { get; private set; } = false;

    #endregion

    #region --- Private Variable ---
    GameObject[] exitLabels ;
    private Transform VisualField;
    private Light pointLight;
    private FieldOfView fieldView;
    private Material fvMat;

    #endregion

    private void Awake()
    {
        VisualField = transform.Find("VisualField");
        transform.Find("pointLight").TryGetComponent(out pointLight);
        pointLight.gameObject.SetActive(false);
        VisualField.TryGetComponent(out fieldView);
        VisualField.TryGetComponent(out Renderer rend);
        fvMat = rend.sharedMaterial;
        fieldView.gameObject.SetActive(false);
        exitLabels = GameObject.FindGameObjectsWithTag(ConstValue.TagName.Exit);
        foreach (var item in exitLabels)
        {
            var pos = item.transform.position;
            item.transform.SetParent(null);
            item.transform.position = pos;
            item.transform.localScale = Vector3.one;
            item.SetActive(false);
        }
        ZombieTimer = new MyTimer(ShowTime);
        GameTimer = new MyTimer(ConstValue.GameTime);
        ZombiePlayer = null;
        HasZombie = false;

        SceneLevelSetting.Instance.ApplyBeginSetting();
    }

    private SignedTimer timeShowCharacter = new SignedTimer();


    #region SceneKillerEffects

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, KillerSceneEffectFunc);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, KillerSceneEffectFunc);
    }

    private void KillerSceneEffectFunc(HumanBase killer)
    {
        if (killer != PlayerControl.Instance)
        {
            var color = fvMat.color;
            color.a = .8f;
            fvMat.color = color;
            return;
        }

        if ( Mathf.Approximately(SDKInit.KillerEffectAB,1))
            StartCoroutine(LightLerp(killer));
    }

    IEnumerator LightLerp(HumanBase killer)
    {
        //主灯光消失
        float needTimer = .4f , timer = 0;
        for (; timer < needTimer; )
        {
            mainLight.intensity = Mathf.Lerp(1, 0, timer / needTimer);
            timer += Time.deltaTime;
            yield return null;
        }
        mainLight.intensity = 0;
        mainLight.enabled = false;

        //点光源出现
        timer = 0;
        pointLight.gameObject.SetActive(true);
        pointLight.transform.SetParent(killer.transform);
        pointLight.transform.localPosition = Vector3.up * 2.25f + Vector3.forward * .4f;
        pointLight.intensity = 0;

        for (; timer < needTimer;)
        {
            pointLight.intensity = Mathf.Lerp(0, 2, timer / needTimer);
            RenderSettings.reflectionIntensity = Mathf.Lerp(1, .7f, timer / needTimer);
            RenderSettings.ambientIntensity = Mathf.Lerp(1, .7f, timer / needTimer);
            timer += Time.deltaTime;
            yield return null;
        }
        var color = fvMat.color;
        color.a = .4f;
        fvMat.color = color;
        pointLight.intensity = 2;
        RenderSettings.reflectionIntensity = .5f;
    }

    #endregion


    void Update()
    {
        //开始三秒倒计时
        if (!ZombieTimer.IsFinish)
        {
            ZombieTimer.OnUpdate(GameManager.DeltaTime);
            //如果不存在丧尸
            if (IsNotZomble)
            {
                PlayerControl.Instance.ChangeToHuman();
                ZombieTimer.SetFinish();
                foreach (var item in exitLabels)
                    item.SetActive(true);
                return;
            }

            timeShowCharacter.OnUpdate(ZombieTimer.timer >= 0.05f);
            if (timeShowCharacter.IsPressDown)
            {
                Messenger.Broadcast(ConstValue.CallBackFun.OpenNewCharacterUI);
            }

            //倒计时完成
            if (ZombieTimer.IsFinish)
            {
                SceneSettingUI.View.Value = false;
                var humans = GameManager.Instance.humanBases;
                //指定自己是杀手
                if (SetZombieIsPlayer)
                {
                    ZombiePlayer = PlayerControl.Instance;
                }
                //指定自己是人类
                else if (SetZombieIsNotPlayer)
                {
                    foreach (var item in humans)
                    {
                        if (item.IsMe)
                            continue;
                        if (item.IsDead)
                            continue;
                        ZombiePlayer = item;
                        break;
                    }
                }
                //随机选择
                else
                {
                    var isMe = UnityEngine.Random.Range(0f, 1f) >= .65f;
                    if (isMe)
                        ZombiePlayer = PlayerControl.Instance;
                    else
                        ZombiePlayer = humans[0] == PlayerControl.Instance ? humans[1] : humans[0];
                }
                HasZombie = true;
                SceneLevelSetting.Instance.ApplyGameSetting();
                for (int i = 0; i < humans.Count; i++)
                    if (humans[i] != ZombiePlayer)
                        humans[i].ChangeToHuman();
                PlayerToZombie(ZombiePlayer);
            }
            return;
        }

        GameTimer.OnUpdate(GameManager.DeltaTime);


        //时间到了，最后结算
        if (GameTimer.IsFinish && !GameManager.isDead && !GameManager.isWin)
        {
            //幸存者没走出，失败
            if (PlayerControl.Instance.IsHuman)
            {
                GameManager.Instance.SetDefeat();
            }
            else
            {
                KillerWin();
                GameManager.Instance.SetWin();
            }
        }
    }

    public void KillerWin()
    {
        PlayerControl.Instance.animControl.SetValue(ConstValue.AnimatorStr.DanceIndex, 0f);
        PlayerControl.Instance.animControl.SetValue(ConstValue.AnimatorStr.Dance);

        PlayerControl.Instance.transform.forward = Vector3.right;
        PlayerControl.Instance.enabled = false;
        PlayerControl.Instance.TryGetComponent(out NavMeshAgent nma);
        nma.enabled = false;

        CameraFllow.Instance.SetOtherShow(PlayerControl.Instance.transform, new Vector3(2, 3, 0));
        FieldOfView.Instance.gameObject.SetActive(false);
        
    }


    private void PlayerToZombie(HumanBase player)
    {
        Messenger.Broadcast<HumanBase>(ConstValue.CallBackFun.ZombieShow, player);
        foreach (var human in GameManager.Instance.humanBases)
            human.StopMove();
        GameManager.Instance.GameStop();
        CameraFllow.Instance.SetOtherShow(ZombiePlayer.transform, new Vector3(2f, 3f, 0));

        var alpha = fvMat.GetColor("_Color").a;
        var _color = player == PlayerControl.Instance ? Color.green : Color.red;
        _color.a = alpha;
        fvMat.SetColor("_Color", _color );

        Register(player);
    }

    private void Register(HumanBase player)
    {
        this.AttachTimer(.25f, () => {
            if (CameraFllow.Instance.IsAtTarget)
            {
                
                player.ChangeToZombie();
                StartCoroutine(SetForward(player.transform));

                //显示特效
                foreach (var eff in ZombieEffects)
                {
                    this.AttachTimer( .5f,()=> {
                        var go = PoolManager.SpawnObject(eff);
                        go.transform.localScale = eff.transform.localScale;
                        go.transform.rotation = eff.transform.rotation;
                        go.transform.position = player.transform.position + Vector3.up * .2f;
                        this.AttachTimer(8f, 
                            ()=> { PoolManager.ReleaseObject(go); }
                            );
                    } );
                }
                //显示武器头盔
                this.AttachTimer(.35f, () => {
                    player.modelsGroup.casqueControl?.SetCasque(0);
                    player.modelsGroup.weaponControl?.SetCasque(0);
                    AudioManager.Instance.PlaySound(6);
                    AudioManager.Instance.PlaySound(5);
                });

                //镜头回归，设置 ，游戏继续
                this.AttachTimer(2,
                    () => {
                        
                        AudioManager.Instance.ChangeMusicVolume(GameSetting.Sound.Value);
                        if (PlayerControl.Instance.IsHuman)
                            AudioManager.Instance.PlayMusic(101, true);
                        else
                            AudioManager.Instance.PlayMusic(104, true);
                        CameraFllow.Instance.SetPlayer();
                        GameManager.Instance.GameContinue();
                        VisualField.SetParent(ZombiePlayer.transform);
                        VisualField.localPosition = Vector3.up * 0.05f + Vector3.forward * -0.1f;
                        VisualField.localRotation = Quaternion.identity;
                        fieldView.gameObject.SetActive(true);
                        foreach (var item in exitLabels)
                            item.SetActive(true);
                        Messenger.Broadcast<HumanBase>(ConstValue.CallBackFun.KillerShow, ZombiePlayer);
                    }
                );
            }
            else
            {
                Register(player);
            }
        });
    }

    public void SetFieldOfViewData(float outRadius,float minRadius , float angle)
    {
        if (!fieldView)
            VisualField.TryGetComponent(out fieldView);
        fieldView.SetTargetData(outRadius, minRadius, angle);
    }


    private IEnumerator SetForward(Transform target)
    {
        float timer = 0, maxTimer = .2f;
        var nowRot = target.rotation;
        var tarRot = Quaternion.Euler(0, 90, 0);
        for (; timer <= maxTimer; )
        {
            target.rotation = Quaternion.Lerp(nowRot, tarRot, timer / maxTimer);
            timer += Time.deltaTime;
            yield return null;
        }
    }

}
