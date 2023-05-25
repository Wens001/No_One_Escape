
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
/// �����ڹ��ӵĸ�����Ʒ�ϣ����Ź��ӵ��ƶ�������/��������
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
    /// �����¼�����ĳ�ʼ���ɵ�
    /// </summary>
    /// <param name="hook">�������еĹ���</param>
    public void InitData(Hook hook)
    {
        Debug.Log("��ʼ��");
        isInit = true;
        isBackMove = false; 

        this.hook = hook;

        //���������������ڵ����ж�
        Renderer renderer = chain.GetComponent<Renderer>();
        while (renderer == null)
        {
            renderer = chain.GetComponentInChildren<Renderer>();
        }
        chainLength = renderer.bounds.size.x - chainOffset;

        //��ȡ���ӳ��ȵ�һ��
        hookHalf = hook.GetComponent<Renderer>().bounds.size.x * .5f;
        //���������ĳ�ʼ��
        initLengthOffset = /*hookHalf*/ chainLength * .5f;
        //initChainPos = hook.transform.position - hook.transform.right * initLengthOffset;
        //initChainPos = transform.position;
    }
    /// <summary>
    /// ���ӷ���
    /// </summary>
    public void BackHook()
    {
        isBackMove = true;
        //Debug.LogError("backKnock");
    }
    /// <summary>
    /// �����ͷŽ���
    /// </summary>
    public void OverShoot()
    {
        isInit = false;
        isBackMove = false;
        this.hook = null;
        Debug.Log("�����ͷ����,����ʣ�������" + chainList.Count);
        chainList.Clear();
    }

    private void Update()
    {
        ChainMgr();
    }


    /// <summary>
    /// ��������ʱ��ִ�У��쳤���ӣ����̼��٣�
    /// </summary>
    private void ChainMgr()
    {
        //���ͷż���ʱ�����г�ʼ��
        if (!isInit)
        {
            return;
        }
        
        float lengthOffset = LengthJudge();
        //Debug.Log("��ǰ���룺"  + lengthOffset);

        //���ӷ��أ���������
        if (isBackMove)
        {
            DestroyChain(lengthOffset);
        }
        //���ӷɳ��������쳤
        else
        {
            CreateChain(lengthOffset);
        }

        
    }
    
    /// <summary>
    /// �жϵ�ǰ���ɵ�Ӧ���빳�ӵ�ƫ��
    /// </summary>
    /// <returns></returns>
    private float LengthJudge()
    {
        float lengthOffset = Vector3.Distance(hook.transform.position, transform.position) + chainLength * 1f;
        return lengthOffset;
    }

    /// <summary>
    /// ���ӷɳ�ʱ�����ݳ���ƫ����������
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
                //Debug.Log("���빳�ӣ�" + Vector3.Distance(pos, hook.transform.position));
                ChainRank newChain = Instantiate(chain, pos, hook.transform.rotation, hook.transform);
                newChain.InitData(hook.gameObject, transform.gameObject);
                chainList.Add(newChain);

                //Debug.Log("���ӷɳ��������������ɡ�������ǰ������" + chainList.Count);
            }
        }
    }

    /// <summary>
    /// ���ӷ���ʱ�����ݳ���ƫ����������
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
                //Debug.Log("���ӷ��ء������������١�������ǰ������" + chainList.Count);
            }

        }

    }

    private Vector3 ChainDir()
    {
        return (hook.transform.position - transform.position).normalized;
        
    }
}
