
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
    /// 今日是否为首次登陆
    /// </summary>
    /// <returns></returns>
    static bool TodayIsFirstLanding()
    {
        var today = PlayerPrefs.GetInt(todayStr, -1);
        return today != DateTime.Today.Day;
    }

    /// <summary>
    /// 清除今日首次登陆标志
    /// </summary>
    public static void ClearTodayFirstLandingFlag()
    {
        PlayerPrefs.SetInt(todayStr, DateTime.Today.Day);
    }

    #region 离线数据

    /// <summary>
    /// 设置离线时间
    /// </summary>
    /// <param name="dateTime"></param>
    public static void SetOfflineDateTime(DateTime dateTime)
    {
        PlayerPrefs.SetString(OfflineTime, dateTime.ToShortTimeString());
    }

    /// <summary>
    /// 设置离线时间
    /// </summary>
    public static void SetOfflineTimeData()
    {
        PlayerPrefs.SetString(OfflineTime, DateTime.Now.ToShortTimeString());
    }

    /// <summary>
    /// 获取离线时间差数据
    /// </summary>
    /// <returns></returns>
    public static TimeSpan GetOfflineTimeSpan()
    {
        DateTime nowTime = DateTime.Now;
        DateTime oldTime = DateTime.Parse(PlayerPrefs.GetString(OfflineTime));
        return nowTime - oldTime;
    }

    /// <summary>
    /// 获取离线时间差天数
    /// </summary>
    /// <returns></returns>
    public static int GetOfflineDay()
    {
        var ts = GetOfflineTimeSpan();
        return ts.Days;
    }

    /// <summary>
    /// 获取离线时间差小时数
    /// </summary>
    /// <returns></returns>
    public static int GetOfflineHour()
    {
        var ts = GetOfflineTimeSpan();
        return ts.Hours + ts.Days * 24;
    }
    #endregion

    #region 首天时间差

    /// <summary>
    /// 获取与首天的时间差数据
    /// </summary>
    /// <returns></returns>
    public static TimeSpan GetFirstDayDifferenceTimeSpan()
    {

        DateTime nowTime = DateTime.Now;
        DateTime oldTime = DateTime.Parse(PlayerPrefs.GetString(firstDay));
        return nowTime - oldTime;
    }

    /// <summary>
    /// 获取与首天的时间差天数
    /// </summary>
    /// <returns></returns>
    public static int GetFirstDayDifferenceDay()
    {
        var ts = GetFirstDayDifferenceTimeSpan();
        return ts.Days;
    }

    /// <summary>
    /// 获取与首天的时间差小时数
    /// </summary>
    /// <returns></returns>
    public static int GetFirstDayDifferenceHour()
    {
        var ts = GetFirstDayDifferenceTimeSpan();
        return ts.Hours + ts.Days * 24;
    }

    #endregion


}
