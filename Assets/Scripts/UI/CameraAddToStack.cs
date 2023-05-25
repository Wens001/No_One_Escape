
/****************************************************
 * FileName:		CameraAddToStack.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-04-14:53:46
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAddToStack : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---



    #endregion

    private Camera _camera;
    private void Awake()
    {
        TryGetComponent(out _camera);
    }

    private void OnEnable()
    {
        EnterScene.TryToAddStack(_camera);
    }

    private void OnDisable()
    {
        EnterScene.TryToRemoveStack(_camera);
    }

}
