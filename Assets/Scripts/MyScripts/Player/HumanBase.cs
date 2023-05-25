
/****************************************************
 * FileName:		HumanBase.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-11-16:50:03
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
[RequireComponent(typeof(NavMeshAgent), typeof(AnimControl))]
public class HumanBase : MonoBehaviour
{

    #region --- Public Variable ---
    /// <summary>
    /// 是否本人
    /// </summary>
    public bool IsMe { get { return this == PlayerControl.Instance; } }

    public bool IsZombie { get { return playerType == PlayerType.Zombie; } }
    public bool IsHuman { get { return playerType == PlayerType.Human; } }

    public PlayerType playerType = PlayerType.Human;
    public float Magnitude { get { return agent.velocity.magnitude; } }
    public Vector3 Position { get { return transform.position; } }
    public bool IsStop { get;protected set; } = false;
    public void SetStop(bool flag) => IsStop = flag;
    public bool IsWin { get; set; } = false;
    public Vector3 WinTargetDir { get; set; } = Vector3.zero;
    public AnimationCurve WallCurve;

    public ModelsGroup modelsGroup { get; private set; }

    public List<HumanBase> targets { get; set; } = new List<HumanBase>();//丧尸的追踪名单

    public bool BeDiscovered { get; set; } = false; //幸存者被发现
    public HumanBase helpTarget { get; set; }

    public Player_Parameter p_Para;
    public bool IsWalk
    {
        get
        {
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
                return false;
            return true;
        }
    }

    public void ApplyParameter()
    {
        if (IsMe)
            return;
        if (IsZombie)
        {
            agent.speed = p_Para.Kill_BaseSpeed;
        }
        else
        {
            agent.speed = p_Para.Human_BaseSpeed;
        }
    }

    protected IPlayerAttackJudge attackJudge;

    public void ChangeAttackJudge(string name)
    {
        switch (name)
        {
            case "gun":
                attackJudge = new PlayerGun(this, 15f);
                break;
            case "dun":
                attackJudge = new PlayerShield(this);
                break;
            case "hook":
                attackJudge = new PlayerPirateAtk(this, p_Para.BaseAttackDeltaTime);
                break;
            default:
                attackJudge = new PlayerNormalAttack(this,p_Para.BaseAttackDeltaTime);
                break;
        }
        this.AttachTimer(.2f, () => {
            animControl.SetValue(ConstValue.AnimatorStr.AttackIndex, name == "" ? 0f : 1f);
        });
    }

    #endregion

    #region --- Private Variable ---

    public AnimControl animControl { get; private set; }
    protected NavMeshAgent agent;
    protected Collider coll;
    protected float Speed = 10;
    public float State { get; set; } = 0;
    #endregion

    protected virtual void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out coll);
        animControl = GetComponent<AnimControl>();
        modelsGroup = GetComponent<ModelsGroup>();
        var new_Para = ScriptableObject.CreateInstance<Player_Parameter>();
        new_Para.CopyValue(p_Para);
        p_Para = new_Para;
        Speed = p_Para.Human_BaseSpeed;
        agent.speed = Speed;
        agent.autoTraverseOffMeshLink = false;
    }

    public float speedMul = 1;
    public void SetSpeedMul(float newMul)
    {
        speedMul = newMul;
        agent.speed = IsHuman ? p_Para.Human_BaseSpeed : p_Para.Kill_BaseSpeed;
        agent.speed *= speedMul;
    }

    /// <summary>
    /// 更改速度基础值（为了实现攻击后的减速仍在加速的基础上）
    /// </summary>
    /// <param name="changeExtent">改动幅度，分正负</param>
    public void ChangeSpeed(float changeExtent)
    {
        agent.speed = IsHuman ? p_Para.Human_BaseSpeed += changeExtent : p_Para.Kill_BaseSpeed += changeExtent;
        agent.speed *= speedMul;
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        animControl.SetAnimVelocity(Magnitude / (IsHuman ? p_Para.Human_BaseSpeed : p_Para.Kill_BaseSpeed )
            , State, Time.deltaTime );

    }

    protected virtual void FixedUpdate()
    {
        if (Physics.Raycast(Position, Vector3.down, out RaycastHit hit, 1f, 1 << LayerMask.NameToLayer(ConstValue.LayerName.Ground)) && hit.collider.CompareTag("Rigi"))
        {
            hit.collider.TryGetComponent(out Rigidbody ri);
            if (ri != null)
            {
                ri.AddForceAtPosition(Vector3.down * 5, hit.point);
            }
        }
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public void Move(Vector3 offset)
    {
        if (agent.enabled)
            agent.Move(offset);
    }


    #region 死亡复活

    public bool IsDead { get; private set; } = false;

    public void PlayerDead(HumanBase killer)
    {
        if (IsDead || GameManager.isWin || GameManager.isDead || IsWin)
            return;
        p_Para.Human_Health = Mathf.Clamp(p_Para.Human_Health - 1,0,100);
        if (p_Para.Human_Health > 0)
        {
            Messenger.Broadcast<HumanBase>(ConstValue.CallBackFun.Damage, this);
            return;
        }
        Messenger.Broadcast<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, killer, this);
        IsDead = true;

        animControl.SetValue(ConstValue.AnimatorStr.DeadIndex, 
                AIChase.DirAngle((killer.Position - Position).normalized, transform.forward) <= 60 ? 1f : 0f);

        animControl.SetValue(ConstValue.AnimatorStr.Dead, IsDead);
        if (agent.enabled)
            agent.ResetPath();
        coll.enabled = false;

        if (this == PlayerControl.Instance)
            InputManager.OpenTouch = false;
        else
        {
            this.AttachTimer(.3f, () => { GameManager.Instance.CreatehelpRebone(Position, this); });
        }

        if (ZombieShowTimer.HasZombie)
        {
            var targets = ZombieShowTimer.ZombiePlayer.targets;
            if (targets.Contains(this))
                targets.Remove(this);
        }
    }

    [ContextMenu("复活")]
    public void PlayerRebone()
    {
        if (IsDead == false || GameManager.isWin || GameManager.isDead)
            return;
        IsDead = false;
        agent.enabled = true;
        animControl.SetValue(ConstValue.AnimatorStr.Dead, IsDead);
        animControl.OnFOVLeave();
        agent.enabled = true;
        coll.enabled = true;
        SetSpeedMul(1);
        if (this == PlayerControl.Instance)
            InputManager.OpenTouch = true;
    }

    #endregion

    #region 爬墙跳跃

    protected void PlayLinkForWallAnimation(float value)
    {
        if (agent.isOnOffMeshLink == false)
            return;
        StartCoroutine(WallAnimation(value));
    }


    private IEnumerator WallAnimation(float _timer)
    {
        var data = agent.currentOffMeshLinkData;
        IsStop = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        endPos.y = startPos.y;
        animControl.SetValue(ConstValue.AnimatorStr.JumpForWall);
        float normalizedTime = 0.0f;

        Vector3 targetForward = endPos - startPos;
        targetForward.y = 0;

        Vector3 startForward = transform.forward;
        startForward.y = 0;
        while (normalizedTime < _timer)
        {
            float curve = normalizedTime / _timer;
            transform.forward = Vector3.Lerp(startForward, targetForward, curve);
            float yOffset = WallCurve.Evaluate(curve) ;
            transform.position = Vector3.Lerp(startPos, endPos, curve) + yOffset * Vector3.up;
            normalizedTime += GameManager.DeltaTime;
            yield return null;
        }
        agent.CompleteOffMeshLink();
        IsStop = false;
    }

    #region 玩家跳跃
    private Coroutine jumpWindows;
    protected void PlayLinkForWallAnimation(float value, Vector3 endPos)
    {
        if (jumpWindows != null)
            StopCoroutine(jumpWindows);
        jumpWindows = StartCoroutine(WallAnimation(value, endPos));
    }

    private IEnumerator WallAnimation(float _timer, Vector3 endPos)
    {
        agent.enabled = false;
        IsStop = true;
        Vector3 startPos = transform.position;
        animControl.SetValue(ConstValue.AnimatorStr.JumpForWall);
        float normalizedTime = 0.0f;

        Vector3 targetForward = endPos - startPos;

        Vector3 startForward = transform.forward;
        startForward.y = 0;
        while (normalizedTime < _timer)
        {

            float curve = normalizedTime / _timer;
            transform.forward = Vector3.Lerp(startForward, targetForward, curve);
            float yOffset = WallCurve.Evaluate(curve) ;
            transform.position = Vector3.Lerp(startPos, endPos, curve) + yOffset * Vector3.up;
            normalizedTime += GameManager.DeltaTime;
            yield return null;
        }
        agent.enabled = true;
        IsStop = false;
        StopMove();
    }

    #endregion


    public void SetTarget(Vector3 pos)
    {
        if (agent.enabled)
        agent.SetDestination(pos);
    }


    /// <summary>
    /// 移动到距离敌人包围盒最近的点,如果是远程英雄的话 stoppingDistance  就是攻击距离,
    /// </summary>
    /// <param name="stoppingDistance"></param>
    /// <param name="enemyCollider"></param>
    public void MoveToEnemy(Collider enemyCollider, float stoppingDistance = 0.17f)
    {
        Vector3 destination = enemyCollider.ClosestPointOnBounds(transform.position);
        agent.SetDestination(destination);
    }


    //停止移动
    public void StopMove()
    {
        if (agent.enabled)
            agent.ResetPath();
    }

    #endregion

    #region 变成僵尸
    /// <summary>
    /// 变成僵尸
    /// </summary>
    public void ChangeToZombie()
    {
        agent.areaMask &= ~(1 << 3);
        if (playerType == PlayerType.Zombie)
            return;
        playerType = PlayerType.Zombie;
        animControl.SetValue(ConstValue.AnimatorStr.Morph);
        IsStop = true;

        this.AttachTimer(2.25f, () => { IsStop = false; });
        State = 1;
        ChangeStateAction();
    }

    /// <summary>
    /// 变成人类
    /// </summary>
    public void ChangeToHuman()
    {
        agent.areaMask = (1 << 4)-1;
        if (IsHuman)
            return;
        playerType = PlayerType.Human;
        ChangeStateAction();
        animControl.OnFOVLeave();
    }

    /// <summary>
    /// 自定义行为
    /// </summary>
    protected virtual void ChangeStateAction()
    {

    }

    #endregion

    #region Test


    /// <summary>
    /// 绘制移动路线
    /// </summary>
    void OnDrawGizmos()
    {
        if (agent != null)
        {
            var path = agent.path;
            Color c = Color.white;
            switch (path.status)
            {
                case NavMeshPathStatus.PathComplete:
                    c = Color.blue;
                    break;

                case NavMeshPathStatus.PathInvalid:
                    c = Color.red;
                    break;

                case NavMeshPathStatus.PathPartial:
                    c = Color.yellow;
                    break;
            }
            for (int i = 1; i < path.corners.Length; ++i)
                Debug.DrawLine(path.corners[i - 1], path.corners[i], c);
        }

    }

    #endregion

}
public enum PlayerType
{
    Human = 1<<0 ,
    Zombie = 1<<1 ,
    All = (1<<2)-1
}
