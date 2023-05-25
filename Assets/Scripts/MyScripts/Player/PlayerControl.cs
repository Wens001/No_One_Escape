/****************************************************
 * FileName:		PlayerControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-11-15:56:16
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
public class PlayerControl : HumanBase 
{

    #region --- Public Variable ---

    public static PlayerControl Instance { get; private set; }
    #endregion


    #region --- Private Variable ---

    private GameObject PlayerEffect;

    #endregion



    override protected void Awake()
    {
        base.Awake();
        Instance = this;
        targetDir = transform.forward;
        
        PlayerEffect = transform.Find("PlayerEffect").gameObject;

    }


    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, SetEffect);
    }

    private void OnDisable()
    {
        attackJudge?.UpdateAttackJudge();
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, SetEffect);
    }

    private void SetEffect(HumanBase human)
    {
        if (human != this)
            return;
        if (PlayerEffect)
            Destroy(PlayerEffect);
    }

    private Vector3 targetDir;



    override protected void Update()
    {
        base.Update();
        if (IsStop || IsDead || GameManager.Speed < 0.01f)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PropsManager.Instance.UseProp(this, PropType.InvisibleCloak);
        }

        if (targetDir != Vector3.zero)
        {
            if (SceneSettingUI.View.Value)
            {

            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), 30 * Time.deltaTime);
            }
        }

        if (IsWin && IsHuman )
        {
            coll.enabled = false;
            targetDir = WinTargetDir;
            agent.velocity = targetDir * p_Para.Human_BaseSpeed;
            return;
        }


        if (InputManager.IsTouch && !GameManager.isWin) //InputManager.IsTouch
        {
            targetDir = InputManager.DeltaPos; //targetDir = InputManager.DeltaPos;
            Speed = IsHuman ? p_Para.Human_BaseSpeed : p_Para.Kill_BaseSpeed ;

            if (SceneSettingUI.View.Value)
            {
                var temp = Vector3.zero;
                temp.x = targetDir.z;
                temp.z = -targetDir.x;
                var nor = transform.TransformDirection(temp).normalized;
                agent.velocity = nor * Speed * speedMul;
                if(targetDir != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nor), .3f * Time.deltaTime);
            }
            else
            {
                agent.velocity = targetDir.normalized * Speed * speedMul;

            }
        }

        JumpForWallFunc();
        attackJudge?.UpdateAttackJudge();


        HelpAnimation();
    }

    private void HelpAnimation()
    {
        if (IsZombie)
            return;
        if (helpTarget && !InputManager.IsTouch && agent.enabled && agent.velocity.magnitude < .3f)
        {
            animControl.SetValue(ConstValue.AnimatorStr.Help, true);
            targetDir = (helpTarget.Position - Position).normalized;
        }
        else
        {
            animControl.SetValue(ConstValue.AnimatorStr.Help, false);
        }
    }


    private void JumpForWallFunc()
    {
        if ( ForwardHasWall(.4f) && CanJump())
        {
            IsStop = true;
            _coll.transform.GetChild(0).TryGetComponent(out NavMeshLink link);
            var lpos = link.transform.position;
            Vector3 point1 = lpos + Vector3.up * link.startPoint.y 
                + link.transform.forward * link.startPoint.z;
            Vector3 point2 = lpos + Vector3.up * link.endPoint.y 
                + link.transform.forward * link.endPoint.z;
            var dis1 = Vector3.Distance(point1, transform.position);
            var dis2 = Vector3.Distance(point2, transform.position);
            if (dis1 > dis2)
                agent.destination = point1;
            else
                agent.destination = point2;

            var tt = agent.destination;
            tt.y = transform.position.y;
            PlayLinkForWallAnimation(.7f, tt);
        }
    }

    private bool CanJump()
    {
        return agent.enabled && (agent.areaMask & (1 << 3)) != 0;
    }

    private Collider _coll;
    private bool ForwardHasWall(float length)
    {
        var pos = transform.position + Vector3.up * .15f;
        if (Physics.Raycast(pos, transform.forward ,out RaycastHit hit , length, 1 << LayerMask.NameToLayer(ConstValue.LayerName.Window)))
        {
            _coll = hit.collider;
            return true;
        }
        return false;
    }


}

/// <summary>
/// 攻击基类
/// </summary>
public abstract class IPlayerAttackJudge
{
    public HumanBase human { get; protected set; }
    public MyTimer AttackDeltaTimer { get; protected set; }
    public IPlayerAttackJudge(HumanBase human,float DeltaTime)
    {
        this.human = human;
        AttackDeltaTimer = new MyTimer(DeltaTime);
        AttackDeltaTimer.SetFinish();
    }
    public virtual void UpdateAttackJudge() { }
}
/// <summary>
/// 普通杀手
/// </summary>
public class PlayerNormalAttack : IPlayerAttackJudge
{
    public HumanBase killTarget { get; private set; }
    public PlayerNormalAttack(HumanBase human, float DeltaTime) : base(human, DeltaTime)
    {
    }

    override public void UpdateAttackJudge()
    {
        if (human.IsHuman)
            return;
        AttackDeltaTimer.OnUpdate(GameManager.DeltaTime);

        if (!human.IsMe)
        {
            int index = AIChase.FindNearIndex(human.Position, human.targets);
            if (index != -1)
            {
                var lastPos = human.targets[index].Position;
                human.SetTarget(lastPos);
            }
        }

        if (AttackDeltaTimer.IsFinish && !human.animControl.GetValue(ConstValue.AnimatorStr.IsClean) && human.targets.Count != 0)
        {
            var index = AIChase.FindNearIndex(human.Position, human.targets);
            if (index != -1 && AIChase.IsInRange(human.Position, human.targets[index].Position, human.p_Para.BaseAttackDistance)
                && AIChase.IsInAngle(human.transform.forward,
                    (human.targets[index].Position - human.Position).normalized,
                    human.p_Para.BaseAttackAngle))
            {
                if (human.modelsGroup.index == 0)
                    AudioManager.Instance.PlaySound(3);
                else 
                    AudioManager.Instance.PlaySound(3);
                killTarget = human.targets[index];
                AttackDeltaTimer.ReStart();
                human.animControl.SetValue(ConstValue.AnimatorStr.Attack);
                human.SetSpeedMul(0);
                human.SetStop(true);
                Messenger.Broadcast(ConstValue.CallBackFun.WillKillPlayer, killTarget);

                Timer.Register(ConstValue.DelayDeadTime, () => {
                    killTarget.PlayerDead(human);
                    if (human.modelsGroup.index == 0)
                        AudioManager.Instance.PlaySound(4);
                    if (human.modelsGroup.index == 1)
                        AudioManager.Instance.PlaySound(11);
                    human.AttachTimer(2.25f, () => { human.SetSpeedMul(1); });
                });
                
                human.AttachTimer(1.25f, () => {
                    human.SetSpeedMul(ConstValue.DeadSpeedMul); human.SetStop(false);
                });
            }
        }
    }
}

/// <summary>
/// 海盗攻击类
/// </summary>
public class PlayerPirateAtk : PlayerNormalAttack
{
    public MyTimer CDTimer;
    public MyTimer ChargeTimer;
    public Hook hook;

    private float skillRadius;

    private GameObject frontSight;
    private MeshRenderer mr_frontSignt;
    private Vector3 initScale;

    public PlayerPirateAtk(HumanBase human, float DeltaTime, float skillRadius = 2.5f, float SkillCD = 5f, float chargeTime = 1.5f) : base(human, DeltaTime)
    {
        CDTimer = new MyTimer(SkillCD);
        CDTimer.SetFinish();

        ChargeTimer = new MyTimer(chargeTime);

        this.skillRadius = skillRadius;

        hook = human.transform.GetComponentInChildren<Hook>();
    }



    public override void UpdateAttackJudge()
    {
        if (frontSight == null)
        {
            frontSight = GameObject.Find("FrontSight");
        }
        else if (mr_frontSignt == null)
        {
            mr_frontSignt = frontSight.GetComponent<MeshRenderer>();
            initScale = frontSight.transform.localScale;
        }

        base.UpdateAttackJudge();

        //技能CD,准备发动技能
        if (CDTimer.IsFinish)
        {
            //显示钩子，代表技能CD结束
            hook.gameObject.SetActive(true);
            //获取视线中的最近目标
            var index = AIChase.FindNearIndex(human.Position, human.targets);
            if (index != -1
                && AIChase.IsInRange(human.Position, human.targets[index].Position, skillRadius)
                && AIChase.IsInAngle(human.transform.forward, (human.targets[index].Position - human.Position).normalized, human.p_Para.BaseViewAngle))
            {
                if (ChargeTimer.IsFinish)
                {
                    //Debug.Log("蓄力结束。。。。。");
                    //蓄力结束，发动技能，进入CD(单次执行)
                    human.transform.forward = (human.targets[index].transform.position - human.transform.position).normalized;
                    LaunchSkill(hook, 8f, human.transform.forward, 2f);
                    CDTimer.ReStart();
                    ChargeTimer.ReStart();
                    human.SetSpeedMul(0);
                    human.SetStop(true);

                    if (mr_frontSignt)
                    {
                        mr_frontSignt.enabled = false;
                        frontSight.transform.localScale = initScale;
                    }
                }
                else
                {
                    //蓄力中
                    human.animControl.SetValue("ExitMotion", false);
                    human.animControl.SetValue("ChargeStatus", true);
                    ChargeTimer.OnUpdate(Time.deltaTime);
                    //Debug.Log("蓄力中。。。。当前蓄力：" + ChargeTimer.timer);

                    if (mr_frontSignt)
                    {
                        mr_frontSignt.enabled = true;
                        frontSight.transform.position = human.targets[index].transform.position + Vector3.up * 2f;
                        frontSight.transform.localScale = Vector3.MoveTowards(frontSight.transform.localScale, initScale * .5f, .7f / (ChargeTimer.DurationTime / Time.deltaTime));
                    }
                }
            }
            else
            {
                //没有目标，停止蓄力
                ChargeTimer.ReStart();
                human.animControl.SetValue("ExitMotion", true);
                human.animControl.SetValue("ChargeStatus", false);
                //Debug.Log("目标丢失。。。。。。");
                //Debug.Log("退出行为。。。");

                if (mr_frontSignt)
                {
                    mr_frontSignt.enabled = false;
                    frontSight.transform.localScale = initScale;
                }
            }
        }
        else
        {
            //CD中
            CDTimer.OnUpdate(Time.deltaTime);
            //隐藏钩子，表示技能CD中
            hook.gameObject.SetActive(false);
            //human.animControl.SetValue("ExitMotion");
            //Debug.Log("技能CD中。。。。。。");
            //Debug.Log("退出行为。。。");
        }
    }

    /// <summary>
    /// 发动技能
    /// </summary>
    /// <param name="hook">钩子主体</param>
    /// <param name="hookLength">攻击距离</param>
    /// <param name="dir">攻击方向</param>
    /// <param name="atkDurTime">技能持续时长</param>
    public void LaunchSkill(Hook hook, float hookLength, Vector3 dir, float atkDurTime)
    {

        human.animControl.SetValue("ExitMotion", false);
        human.animControl.SetValue("ChargeStatus", false);
        hook.LaunchSkill(human, hookLength, dir, atkDurTime);
    }

}



/// <summary>
/// 猎人
/// </summary>
public class PlayerGun : IPlayerAttackJudge
{
    private float distance = 1.5f;
    private float turnTimer = .2f;
    public PlayerGun(HumanBase human, float DeltaTime) : base(human, DeltaTime)
    {

    }
    private SignedTimer signedTimer = new SignedTimer();
    override public void UpdateAttackJudge()
    {
        if (human.IsZombie)
            return;
        AttackDeltaTimer.OnUpdate(GameManager.DeltaTime);

        signedTimer.OnUpdate(AttackDeltaTimer.IsFinish);
        
        if (signedTimer.IsPressDown && human.IsMe)
            AudioManager.Instance.PlaySound(10);
        if ( AttackDeltaTimer.IsFinish && human.BeDiscovered && 
            Vector3.Distance(human.Position , ZombieShowTimer.ZombiePlayer.Position ) < distance )
        {
            AttackDeltaTimer.ReStart();
            if (human.IsMe)
            {
                InputManager.OpenTouch = false;
                human.AttachTimer(turnTimer, () => {
                    if (!human.IsDead)
                        InputManager.OpenTouch = true;
                });
            }

            var cor1 = Observable.FromCoroutine(RotateToKiller);
            Observable.WhenAll(cor1).Subscribe(
                _ => {
                    GameManager.Instance.XuanYun(ZombieShowTimer.ZombiePlayer, 2f);
                    AudioManager.Instance.PlaySound(9);
                    Messenger.Broadcast(ConstValue.CallBackFun.PlayEffect,human);
                    human.animControl.SetValue(ConstValue.AnimatorStr.Attack);
                }).AddTo(human);
        }
    }

    private IEnumerator RotateToKiller()
    {
        human.SetStop(true);
        float t = 0;
        var begin = human.transform.rotation;
        var dir = ZombieShowTimer.ZombiePlayer.Position - human.Position;
        dir.y = 0;
        dir = dir.normalized;
        var end = Quaternion.LookRotation(dir);
        for (; t < turnTimer; )
        {
            human.transform.rotation = Quaternion.Lerp(begin, end, t / turnTimer);
            t += GameManager.DeltaTime;
            yield return null;
        }
        human.transform.rotation = end;
        human.AttachTimer(.3f, () => human.SetStop(false));
    }

}

/// <summary>
/// 防爆警察
/// </summary>
public class PlayerShield : IPlayerAttackJudge
{
    public int size = 1;
    public PlayerShield(HumanBase human) : base(human, 1)
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.WillKillPlayer, WillKillPlayerCallBack);
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.Damage, PlayerDamageCallBack);
    }

    ~PlayerShield()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.WillKillPlayer, WillKillPlayerCallBack);
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.Damage, PlayerDamageCallBack);
    }

    private void PlayerDamageCallBack(HumanBase target)
    {
        if (target != human)
            return;
        human.AttachTimer(.3f, () => human.SetStop(false));
        
    }

    private void WillKillPlayerCallBack(HumanBase hb)
    {
        if (hb != human)
            return;
        if (size == 0)
            return;
        size--;
        hb.p_Para.Human_Health += 1;
        human.StartCoroutine(RotateToKiller());
        human.animControl.SetValue(ConstValue.AnimatorStr.AttackIndex, 1f);
        human.animControl.SetValue(ConstValue.AnimatorStr.Attack);
    }

    private readonly float turnTimer = .3f;
    private IEnumerator RotateToKiller()
    {
        human.SetStop(true);
        float t = 0;
        var begin = human.transform.rotation;
        var dir = ZombieShowTimer.ZombiePlayer.Position - human.Position;
        dir.y = 0;
        dir = dir.normalized;
        var end = Quaternion.LookRotation(dir);
        for (; t < turnTimer;)
        {
            human.transform.rotation = Quaternion.Lerp(begin, end, t / turnTimer);
            t += GameManager.DeltaTime;
            yield return null;
        }
        human.transform.rotation = end;

    }
}
