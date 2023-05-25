
/****************************************************
 * FileName:		AIMoveToTarget.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-11-17:01:40
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIMoveToTarget : MonoBehaviour
{

    #region --- Public Variable ---

    public HumanBase targtHuman;
    public Transform target;

    #endregion


    #region --- Private Variable ---

    private NavMeshAgent agent;
    #endregion

    private void Awake()
    {
        targtHuman.TryGetComponent(out agent);
    }

    private void Update()
    {
        if (!ZombieShowTimer.HasZombie)
            return;
        if (targtHuman && !targtHuman.IsWin && !targtHuman.IsDead && agent.enabled)
            agent.SetDestination(target.position);
    }


}
