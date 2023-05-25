
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
        
        //kill�ɾ�
        var killItem = temDir[AchievementType.KillTarget.ToString()];
        KillSlider.value = killItem.curNum / killItem.CurTarget;

        //��Ԯ�ɾ�
        var HelpItem = temDir[AchievementType.HelpTarget.ToString()];
        HelpSlider.value = HelpItem.curNum / HelpItem.CurTarget;

        //�������سɾ�
        var TouchItem = temDir[AchievementType.TouchButtonTarget.ToString()];
        TouchSlider.value = TouchItem.curNum / TouchItem.CurTarget;

        //���ѳɾ�
        var EscapeItem = temDir[AchievementType.EscapeTarget.ToString()];
        EscapeSlider.value = EscapeItem.curNum / EscapeItem.CurTarget;

        //�����ɾ�
        var UpgradeItem = temDir[AchievementType.UpgradeTarget.ToString()];
        UpgradeSlider.value = UpgradeItem.curNum / UpgradeItem.CurTarget;

        //�������ɾ�
        var UpgradeFullItem = temDir[AchievementType.UpgradeFullTarget.ToString()];
        UpgradeFullSlider.value = UpgradeFullItem.curNum / UpgradeFullItem.CurTarget;

        //��һ��ѳɾ�
        var SpentCoinItem = temDir[AchievementType.CoinSpentTarget.ToString()];
        SpentCoinSlider.value = SpentCoinItem.curNum / SpentCoinItem.CurTarget;

        //��ʯ���ѳɾ�
        var SpentGemsItem = temDir[AchievementType.GemSpentTarger.ToString()];
        SpentGemsSlider.value = SpentGemsItem.curNum / SpentGemsItem.CurTarget;

    }

}
