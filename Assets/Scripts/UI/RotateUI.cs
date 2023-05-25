
/****************************************************
 * FileName:		RotateUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-26-15:15:47
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUI : MonoBehaviour
{

    #region --- Public Variable ---

    public float angle = 60f;
    #endregion


    #region --- Private Variable ---

    private RectTransform rt;
    private Vector3 angleV;

    #endregion


    
    void Start()
    {
        rt = transform as RectTransform;
        angleV = new Vector3(0,0,-angle);
    }

    // Update is called once per frame
    void Update()
    {
        rt.Rotate(angleV * Time.deltaTime);
    }
}
