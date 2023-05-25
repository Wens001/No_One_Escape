
/****************************************************
 * FileName:		NewBehaviourScript.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-13-09:57:49
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{

    #region --- Public Variable ---
    public Slider KillSlider;
    public Slider HelpSlider;
    public Slider TouchSlider;
    public Slider EscapeSlider;
    public Slider UpgradeSlider;
    public Slider UpgradeFullSlider;
    public Slider SpentCoinSlider;
    public Slider SpentGemsSlider;


    #endregion


    #region --- Private Variable ---



    #endregion

    private void OnEnable()
    {
        var temDir = AchievementSystem.Instance.achievementDic;
        
        //kill成就
        var killItem = temDir[AchievementType.KillTarget.ToString()];
        KillSlider.value = killItem.curNum / killItem.CurTarget;

        //救援成就
        var HelpItem = temDir[AchievementType.HelpTarget.ToString()];
        HelpSlider.value = HelpItem.curNum / HelpItem.CurTarget;

        //触碰开关成就
        var TouchItem = temDir[AchievementType.TouchButtonTarget.ToString()];
        TouchSlider.value = TouchItem.curNum / TouchItem.CurTarget;

        //逃脱成就
        var EscapeItem = temDir[AchievementType.EscapeTarget.ToString()];
        EscapeSlider.value = EscapeItem.curNum / EscapeItem.CurTarget;

        //升级成就
        var UpgradeItem = temDir[AchievementType.UpgradeTarget.ToString()];
        UpgradeSlider.value = UpgradeItem.curNum / UpgradeItem.CurTarget;

        //升满级成就
        var UpgradeFullItem = temDir[AchievementType.UpgradeFullTarget.ToString()];
        UpgradeFullSlider.value = UpgradeFullItem.curNum / UpgradeFullItem.CurTarget;

        //金币花费成就
        var SpentCoinItem = temDir[AchievementType.CoinSpentTarget.ToString()];
        SpentCoinSlider.value = SpentCoinItem.curNum / SpentCoinItem.CurTarget;

        //宝石花费成就
        var SpentGemsItem = temDir[AchievementType.GemSpentTarger.ToString()];
        SpentGemsSlider.value = SpentGemsItem.curNum / SpentGemsItem.CurTarget;

    }

}
