
/****************************************************
 * FileName:		RotateObj.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-30-14:16:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour
{

    #region --- Public Variable ---

    public float angle = 120f;

    #endregion


    #region --- Private Variable ---



    #endregion



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * angle * GameManager.DeltaTime );
    }
}
