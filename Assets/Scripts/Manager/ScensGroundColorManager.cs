
/****************************************************
 * FileName:		ScensGroundColorManager.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-09-09:53:21
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class ScensGroundColorManager : Singleton<ScensGroundColorManager>
{
    public MatProperty matProperty;
    void Start()
    {
        var index = LevelSetting.Value;
        matProperty.UseMatProperty(index+1);
        
    }
}
