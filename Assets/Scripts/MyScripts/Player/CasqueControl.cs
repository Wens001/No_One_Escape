
/****************************************************
 * FileName:		CasqueControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-13-15:36:39
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CasqueControl : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private MeshRenderer[] mrs;

    #endregion

    private void Awake()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        HideAllCasque();
    }

    public void HideAllCasque()
    {
        for (int i = 0; i < mrs.Length; i++)
            mrs[i].enabled = false;
    }

    public void SetCasque(int index)
    {
        if (index >= mrs.Length)
            return;
        HideAllCasque();
        mrs[index].enabled = true;
    }


}
