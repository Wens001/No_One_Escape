
/****************************************************
 * FileName:		PlayerPrefsTime.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-24-10:04:38
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/

/****************************************************
 * FileName:		PlayerPrefsTime.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-24-17:05:18
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using PP = UnityEngine.PlayerPrefs;
public class PlayerPrefsTime
{

    /// <summary>
    /// �Զ����ַ����Ե�ǰʱ�䱣��
    /// </summary>
    /// <param name="name"></param>
    public static void SetDateTime(string name)
    {
        PP.SetString(name, DateTime.Now.ToShortTimeString());
    }

    /// <summary>
    /// �Զ����ַ�����ָ��ʱ�䱣��
    /// </summary>
    /// <param name="name"></param>
    public static void SetDateTime(string name, DateTime dt)
    {
        PP.SetString(name, dt.ToShortTimeString());
    }

    public static DateTime GetDateTime(string name, DateTime dt = default)
    {
        if (!PP.HasKey(name))
            return dt;
        return DateTime.Parse(PP.GetString(name));
    }


}
