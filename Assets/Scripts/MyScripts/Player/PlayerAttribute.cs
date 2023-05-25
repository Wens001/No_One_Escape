
/****************************************************
 * FileName:		PlayerAttribute.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-23-10:28:51
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class PlayerAttribute : MonoBehaviour
{

    #region --- Public Variable ---

    public Player_Parameter _startParameter;    //初始属性
    #endregion


    #region --- Private Variable ---

    private HumanBase humanBase;
    private ModelsGroup modelsGroup;

    private ReactiveProperty<Player_Parameter>  _deltaPara;   //当前增幅

    #endregion



    void Start()
    {
        TryGetComponent(out humanBase);
        TryGetComponent(out modelsGroup);

        _deltaPara = new ReactiveProperty<Player_Parameter>( ScriptableObject.CreateInstance<Player_Parameter>());
        _deltaPara.Value.Reset();
        InitDeltaPara();
        _deltaPara.Subscribe(_=> { SetAttribute(); }).AddTo(this);
    }

    private void InitDeltaPara()
    {
        _deltaPara.Value.Human_BaseSpeed = ChangeSpeed(PlayerPrefs.GetInt("P_Speed" + GetIndex(ConstValue.SaveDataStr.HumanIndex)));;
        _deltaPara.Value.Help_BaseSpeed = PlayerPrefs.GetInt("P_Help_Speed" 
            + GetIndex(ConstValue.SaveDataStr.HumanIndex)) * 0.15f;
        _deltaPara.Value.Kill_BaseSpeed = ChangeSpeed(PlayerPrefs.GetInt("P_Speed2" + GetIndex(ConstValue.SaveDataStr.KillerIndex)));
        _deltaPara.Value.BaseAttackAngle = PlayerPrefs.GetInt("P_Range"
            + GetIndex(ConstValue.SaveDataStr.KillerIndex)) * 1.6667f;
        _deltaPara.Value.BaseMaxViewRadius = PlayerPrefs.GetInt("P_Range"
            + GetIndex(ConstValue.SaveDataStr.KillerIndex)) * 0.16667f ;
        _deltaPara.Value.BaseMinViewRadius = PlayerPrefs.GetInt("P_Range"
            + GetIndex(ConstValue.SaveDataStr.KillerIndex)) * 0.07778f;
    }

    private float ChangeSpeed(int index)
    {
        var speedDelta = 0.12667f;
        if (index <= 6)
            return index * speedDelta;
        return (6 * speedDelta) + (index - 6) * speedDelta / 2f;
    }

    private string GetIndex(string str)
    {
        return PlayerPrefs.GetInt(str, 0).ToString();
    }

    /// <summary>
    /// 应用属性
    /// </summary>
    public void SetAttribute()
    {
        humanBase.p_Para.Kill_BaseSpeed = _startParameter.Kill_BaseSpeed + _deltaPara.Value.Kill_BaseSpeed;
        humanBase.p_Para.BaseAttackDistance = _startParameter.BaseAttackDistance + _deltaPara.Value.BaseAttackDistance;
        humanBase.p_Para.BaseAttackDeltaTime = _startParameter.BaseAttackDeltaTime + _deltaPara.Value.BaseAttackDeltaTime;
        humanBase.p_Para.BaseAttackAngle = _startParameter.BaseAttackAngle + _deltaPara.Value.BaseAttackAngle;
        humanBase.p_Para.Human_BaseSpeed = _startParameter.Human_BaseSpeed + _deltaPara.Value.Human_BaseSpeed;
        humanBase.p_Para.Help_BaseSpeed = _startParameter.Help_BaseSpeed + _deltaPara.Value.Help_BaseSpeed;

        humanBase.p_Para.BaseViewAngle = _startParameter.BaseViewAngle + _deltaPara.Value.BaseViewAngle;
        humanBase.p_Para.BaseMaxViewRadius = _startParameter.BaseMaxViewRadius + _deltaPara.Value.BaseMaxViewRadius;
        humanBase.p_Para.BaseMinViewRadius = _startParameter.BaseMinViewRadius + _deltaPara.Value.BaseMinViewRadius;

    }

}
