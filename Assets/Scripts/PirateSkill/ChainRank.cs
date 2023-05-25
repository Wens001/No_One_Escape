
/****************************************************
 * FileName:		ChainRank.cs
 * CompanyName:		
 * Author:			贾浩南
 * Email:			
 * CreateTime:		2020-07-30-14:37:57
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainRank : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---
    private GameObject hook;
    private GameObject shooter;

    private bool isInit;
    
    #endregion


    public void InitData(GameObject hook, GameObject shooter)
    {
        this.hook = hook;
        this.shooter = shooter;
        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit)
        {
            transform.right = hook.transform.position - shooter.transform.position;
        }
    }
}
