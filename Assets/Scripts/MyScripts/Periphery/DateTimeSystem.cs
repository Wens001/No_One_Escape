
/****************************************************
 * FileName:		DateTimeSystem.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-24-17:05:18
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System;
public class DateTimeSystem
{
    private const string firstDay = "FirstDay";
    private const string todayStr = "Today";
    private const string OfflineTime = "OfflineTime";

    public DateTimeSystem()
    {
        if (!PlayerPrefs.HasKey(firstDay))
            PlayerPrefs.SetString(firstDay, DateTime.Now.ToShortTimeString());
    }

    /// <summary>
    /// �����Ƿ�Ϊ�״ε�½
    /// </summary>
    /// <returns></returns>
    static bool TodayIsFirstLanding()
    {
        var today = PlayerPrefs.GetInt(todayStr, -1);
        return today != DateTime.Today.Day;
    }

    /// <summary>
    /// ��������״ε�½��־
    /// </summary>
    public static void ClearTodayFirstLandingFlag()
    {
        PlayerPrefs.SetInt(todayStr, DateTime.Today.Day);
    }

    #region ��������

    /// <summary>
    /// ��������ʱ��
    /// </summary>
    /// <param name="dateTime"></param>
    public static void SetOfflineDateTime(DateTime dateTime)
    {
        PlayerPrefs.SetString(OfflineTime, dateTime.ToShortTimeString());
    }

    /// <summary>
    /// ��������ʱ��
    /// </summary>
    public static void SetOfflineTimeData()
    {
        PlayerPrefs.SetString(OfflineTime, DateTime.Now.ToShortTimeString());
    }

    /// <summary>
    /// ��ȡ����ʱ�������
    /// </summary>
    /// <returns></returns>
    public static TimeSpan GetOfflineTimeSpan()
    {
        DateTime nowTime = DateTime.Now;
        DateTime oldTime = DateTime.Parse(PlayerPrefs.GetString(OfflineTime));
        return nowTime - oldTime;
    }

    /// <summary>
    /// ��ȡ����ʱ�������
    /// </summary>
    /// <returns></returns>
    public static int GetOfflineDay()
    {
        var ts = GetOfflineTimeSpan();
        return ts.Days;
    }

    /// <summary>
    /// ��ȡ����ʱ���Сʱ��
    /// </summary>
    /// <returns></returns>
    public static int GetOfflineHour()
    {
        var ts = GetOfflineTimeSpan();
        return ts.Hours + ts.Days * 24;
    }
    #endregion

    #region ����ʱ���

    /// <summary>
    /// ��ȡ�������ʱ�������
    /// </summary>
    /// <returns></returns>
    public static TimeSpan GetFirstDayDifferenceTimeSpan()
    {

        DateTime nowTime = DateTime.Now;
        DateTime oldTime = DateTime.Parse(PlayerPrefs.GetString(firstDay));
        return nowTime - oldTime;
    }

    /// <summary>
    /// ��ȡ�������ʱ�������
    /// </summary>
    /// <returns></returns>
    public static int GetFirstDayDifferenceDay()
    {
        var ts = GetFirstDayDifferenceTimeSpan();
        return ts.Days;
    }

    /// <summary>
    /// ��ȡ�������ʱ���Сʱ��
    /// </summary>
    /// <returns></returns>
    public static int GetFirstDayDifferenceHour()
    {
        var ts = GetFirstDayDifferenceTimeSpan();
        return ts.Hours + ts.Days * 24;
    }

    #endregion


}
