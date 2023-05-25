
/****************************************************
 * FileName:		InputManager.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-11-15:46:43
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{

    #region --- Public Variable ---

    public static bool OpenTouch { get; set; } = true;

    public static bool IsTouch { get; private set; } = false;
    public static Vector3 beginPos { get; private set; }

    public static Vector3 DeltaPos
    {
        get
        {
            var res = Vector3.zero;
            if (IsTouch == false || Input.mousePosition == beginPos || OpenTouch == false || GameManager.Speed <= 0.01f)
            {
                return res;
            }
            res.x = (Input.mousePosition.y - beginPos.y) ;
            res.z = (Input.mousePosition.x - beginPos.x) ;
            res = res.normalized;
            res.x = -res.x;
            //if (Vector3.Distance(beginPos, Input.mousePosition) > 200)
            //   beginPos = Vector3.Lerp(beginPos, Input.mousePosition, Time.deltaTime * 3);

            return res;
        }
    }

    #endregion


    #region --- Private Variable ---



    #endregion



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsTouch = true;
            beginPos = Input.mousePosition;
        }

        if (IsTouch && GameManager.Speed > 0.01f && OpenTouch)
        {
            if (!SDKInit.IsDebug)
            {
                JoyStickTest.Show();
                JoyStickTest.SetPosition(beginPos);
            }
        }
        else
            JoyStickTest.Hide();

        if (Input.GetMouseButtonUp(0))
        {
            IsTouch = false;
        }

        


    }
}
