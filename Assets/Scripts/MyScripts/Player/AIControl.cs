
/****************************************************
 * FileName:		AIControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-11-14:33:28
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

[RequireComponent(typeof(Animator))]
public class AIControl : HumanBase
{

    #region --- Public Variable ---
    public AI_Parameter aiParameter;

    public bool hasProp;
    public PropType propName;
    #endregion


    #region --- Private Variable ---
    private Animator AIStateAnim;
    private MyTimer updateTime ;

    #endregion


    override protected void Awake()
    {
        base.Awake();
        TryGetComponent(out AIStateAnim);
    }

    override protected void Start()
    {
        base.Start();
        updateTime = new MyTimer(Random.Range(aiParameter.HumanIntervalTime.x, aiParameter.HumanIntervalTime.y));
    }


    private Vector3 targetDir;
    override protected void Update()
    {
        base.Update();
        if (!IsStop && !IsDead)
            attackJudge?.UpdateAttackJudge();
        if (IsWin)
        {
            AIStateAnim.enabled = false;
            coll.enabled = false;
            if (!GameManager.isWin && !GameManager.isDead)
            {
                targetDir = WinTargetDir;
                if (targetDir != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), 10 * Time.deltaTime);
                agent.velocity = targetDir * p_Para.Human_BaseSpeed;
            }

            return;
        }

        if (IsStop)
            agent.velocity = Vector3.zero;
        if (agent.isOnOffMeshLink && !IsStop )
            PlayLinkForWallAnimation(.7f);

        if (IsHuman)
        {
            if (helpTarget && agent.enabled && agent.velocity.magnitude < .3f)
            {
                animControl.SetValue(ConstValue.AnimatorStr.Help, true);
                transform.forward = Vector3.Lerp(transform.forward, (helpTarget.Position - Position).normalized,Time.deltaTime * 10) ;
            }
            else
            {
                animControl.SetValue(ConstValue.AnimatorStr.Help, false);
            }
        }
    }

    protected override void ChangeStateAction()
    {
        if (IsHuman)
            updateTime.DurationTime = Random.Range(aiParameter.HumanIntervalTime.x, aiParameter.HumanIntervalTime.y);
        else
            updateTime.DurationTime = Random.Range(aiParameter.ZombieIntervalTime.x, aiParameter.ZombieIntervalTime.y);
    }

    /// <summary>
    /// 获取范围内随机目标点
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public Vector3 GetRamdomPos(Vector3 pos ,float range)
    {
        range = Mathf.Abs(range);
        Vector3 tpos = Vector3.zero;
        Vector2 WalkRangeX = Vector2.zero , WalkRangeZ = Vector2.zero;
        WalkRangeX.x = Mathf.Clamp(pos.x - range, -20, 20);
        WalkRangeX.y = Mathf.Clamp(pos.x + range, -20, 20);
        WalkRangeZ.x = Mathf.Clamp(pos.z - range, -20, 20);
        WalkRangeZ.y = Mathf.Clamp(pos.z + range, -20, 20);

        int i = 0;
        for (; ;i++ )
        {
            tpos.x = Random.Range(WalkRangeX.x, WalkRangeX.y);
            tpos.z = Random.Range(WalkRangeZ.x, WalkRangeZ.y);

            if (i >= 5)
                return pos;
            if (PositionHasWall(tpos))
                continue;
            return GroundPosition(tpos);
        }
    }

    private bool IsInRange(float value,float min,float max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// 该位置是否有墙体
    /// </summary>
    /// <param name="tpos"></param>
    /// <returns></returns>
    public bool PositionHasWall(Vector3 tpos)
    {
        return Physics.Raycast(tpos + Vector3.up * 3, Vector3.down, 4, 1 << LayerMask.NameToLayer(ConstValue.LayerName.Wall));
    }


    public void FllowTarget(HumanBase _target)
    {
        agent.SetDestination(_target.transform.position);
    }

    /// <summary>
    /// 获取目标地板位置
    /// </summary>
    /// <param name="tpos"></param>
    /// <returns></returns>
    public Vector3 GroundPosition(Vector3 tpos)
    {
        if (Physics.Raycast(tpos + Vector3.up * 3, Vector3.down, out RaycastHit hit, 4, 1 << LayerMask.NameToLayer(ConstValue.LayerName.Ground)))
            return hit.point;
        return Vector3.zero;
    }

}
