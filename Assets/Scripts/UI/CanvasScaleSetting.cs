
/****************************************************
 * FileName:		CanvasScaleSetting.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-04-13:58:52
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasScaleSetting : MonoBehaviour
{


    #region --- Private Variable ---

    private CanvasScaler canvasScaler;

    #endregion

    private void Awake()
    {
        TryGetComponent(out canvasScaler);
        if (Screen.height >= Screen.width * 2)
            canvasScaler.matchWidthOrHeight = 0;
        else
            canvasScaler.matchWidthOrHeight = 1;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Screen.height >= Screen.width * 2)
            canvasScaler.matchWidthOrHeight = 0;
        else
            canvasScaler.matchWidthOrHeight = 1;
    }
#endif

}
