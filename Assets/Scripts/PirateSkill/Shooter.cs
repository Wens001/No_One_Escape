
/****************************************************
 * FileName:		Shooter.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-29-15:13:07
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在钩子的父级物品上，随着钩子的移动逐步生成/销毁锁链
/// </summary>

public class Shooter : MonoBehaviour
{

    #region --- Public Variable ---

    public ChainRank chain;

    #endregion


    #region --- Private Variable ---

    private Hook hook;
    private float chainLength;
    private const float chainOffset = 0.02f;
    private float hookHalf;
    private float initLengthOffset;
    //private Vector3 initChainPos;
    private List<ChainRank> chainList = new List<ChainRank>();

    private bool isInit = false;
    private bool isBackMove = false;

    #endregion

    /// <summary>
    /// 计算记录锁链的初始生成点
    /// </summary>
    /// <param name="hook">即将飞行的钩子</param>
    public void InitData(Hook hook)
    {
        Debug.Log("初始化");
        isInit = true;
        isBackMove = false; 

        this.hook = hook;

        //根据锁链长度所在的轴判定
        Renderer renderer = chain.GetComponent<Renderer>();
        while (renderer == null)
        {
            renderer = chain.GetComponentInChildren<Renderer>();
        }
        chainLength = renderer.bounds.size.x - chainOffset;

        //获取钩子长度的一半
        hookHalf = hook.GetComponent<Renderer>().bounds.size.x * .5f;
        //生成锁链的初始点
        initLengthOffset = /*hookHalf*/ chainLength * .5f;
        //initChainPos = hook.transform.position - hook.transform.right * initLengthOffset;
        //initChainPos = transform.position;
    }
    /// <summary>
    /// 钩子返回
    /// </summary>
    public void BackHook()
    {
        isBackMove = true;
        //Debug.LogError("backKnock");
    }
    /// <summary>
    /// 技能释放结束
    /// </summary>
    public void OverShoot()
    {
        isInit = false;
        isBackMove = false;
        this.hook = null;
        Debug.Log("技能释放完毕,锁链剩余节数：" + chainList.Count);
        chainList.Clear();
    }

    private void Update()
    {
        ChainMgr();
    }


    /// <summary>
    /// 锁链管理（时刻执行，伸长增加；缩短减少）
    /// </summary>
    private void ChainMgr()
    {
        //在释放技能时，进行初始化
        if (!isInit)
        {
            return;
        }
        
        float lengthOffset = LengthJudge();
        //Debug.Log("当前距离："  + lengthOffset);

        //钩子返回，锁链缩短
        if (isBackMove)
        {
            DestroyChain(lengthOffset);
        }
        //钩子飞出，锁链伸长
        else
        {
            CreateChain(lengthOffset);
        }

        
    }
    
    /// <summary>
    /// 判断当前生成点应距离钩子的偏差
    /// </summary>
    /// <returns></returns>
    private float LengthJudge()
    {
        float lengthOffset = Vector3.Distance(hook.transform.position, transform.position) + chainLength * 1f;
        return lengthOffset;
    }

    /// <summary>
    /// 钩子飞出时，根据长度偏差生成锁链
    /// </summary>
    /// <param name="position"></param>
    /// <param name="hook"></param>
    private void CreateChain(float lengthOffset)
    {
        Vector3 dir = ChainDir();
        for (int j = 0; j < chainList.Count; j++)
        {
            Vector3 pos = hook.transform.position - dir * (initLengthOffset + chainLength * j);
            chainList[j].transform.position = pos;
        }

        int needNum = (int)((lengthOffset /*- initLengthOffset*/) / chainLength) + 1;
        if (needNum >= chainList.Count)
        {
            int addNum = needNum - chainList.Count;
            

            for (int i = 0; i < addNum; i++)
            {
                Vector3 pos = hook.transform.position - dir * (initLengthOffset + chainLength * chainList.Count);
                //Debug.Log("距离钩子：" + Vector3.Distance(pos, hook.transform.position));
                ChainRank newChain = Instantiate(chain, pos, hook.transform.rotation, hook.transform);
                newChain.InitData(hook.gameObject, transform.gameObject);
                chainList.Add(newChain);

                //Debug.Log("钩子飞出。。。锁链生成。。。当前节数：" + chainList.Count);
            }
        }
    }

    /// <summary>
    /// 钩子返回时，根据长度偏差销毁锁链
    /// </summary>
    /// <param name="position"></param>
    /// <param name="hook"></param>
    private void DestroyChain(float lengthOffset)
    {
        int count = chainList.Count;
        if (count == 0)
        {
            return;
        }

        int needNum = (int)((lengthOffset /*- initLengthOffset*/) / chainLength);
        if (count >= needNum)
        {
            int destoryNum = count - needNum;

            for (int i = 0; i < destoryNum; i++)
            {
                DestroyImmediate(chainList[count - 1].gameObject);
                chainList.Remove(chainList[--count]);
                //Debug.Log("钩子返回。。。锁链销毁。。。当前节数：" + chainList.Count);
            }

        }

    }

    private Vector3 ChainDir()
    {
        return (hook.transform.position - transform.position).normalized;
        
    }
}
