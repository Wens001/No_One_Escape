
/****************************************************
 * FileName:		SceneLevelSetting.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-28-16:44:27
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public class LevelSaveSetting
{
    [TabGroup("全局设置")] [LabelText("指定为杀手")] public bool palyerIsZombie;
    [TabGroup("全局设置")] [LabelText("指定为幸存者")] public bool palyerIsNotZombie;
    [TabGroup("全局设置")] [LabelText("幸存者AI形象")] public List<int> aI_Datas = new List<int>() { 0, 0, 0, 0 };
    [TabGroup("杀手")] [LabelText("指定形象")] [PropertyRange(-1, 4)] public int ZombieIndex = -1;
    [TabGroup("杀手")] [LabelText("攻击距离")] [PropertyRange(0, 2)] public float BaseAttackDistance = .7f;
    [TabGroup("杀手")] [LabelText("攻击间隔时间")] [PropertyRange(0, 2)] public float BaseAttackDeltaTime = 1.5f;
    [TabGroup("杀手")] [LabelText("攻击角度范围")] [PropertyRange(0, 90)] public float BaseAttackAngle = 30f;
    [TabGroup("杀手")] [LabelText("视野范围")] [PropertyRange(0, 360)] public float BaseViewAngle = 60f;
    [TabGroup("杀手")] [LabelText("最大视野")] [PropertyRange(0, 10)] public float BaseViewLength = 5f;
    [TabGroup("杀手")] [LabelText("最小视野")] [PropertyRange(0, 10)] public float BaseMinViewLength = 1f;
}

public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> valueData = new List<TValue>();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
        {
            this[this.keyData[i]] = this.valueData[i];
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        this.keyData.Clear();
        this.valueData.Clear();

        foreach (var item in this)
        {
            this.keyData.Add(item.Key);
            this.valueData.Add(item.Value);
        }
    }
}
[System.Serializable]
public class KeyCodeGameObjectListDictionary : UnitySerializedDictionary<int, LevelSaveSetting> { }

public class SceneLevelSetting : SerializedMonoBehaviour
{
    #region --- Public Variable ---
    public static SceneLevelSetting Instance;

    [LabelText("默认设置")] public LevelSaveSetting defaultSetting;

    public KeyCodeGameObjectListDictionary overriveSetting;
    #endregion

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 应用初始设置
    /// </summary>
    public void ApplyBeginSetting()
    {
        var index = LevelSetting.Value;
        var dic = overriveSetting;
        if (dic == null)
            Debug.LogError(dic);
        if (!dic.ContainsKey(index))
            ApplyBegin(defaultSetting);
        else
            ApplyBegin(dic[index]);
    }

    private void ApplyBegin(LevelSaveSetting ls)
    {
        if (ls == null)
            return;
        ZombieShowTimer.Instance.SetZombieIsPlayer = ls.palyerIsZombie;
        ZombieShowTimer.Instance.SetZombieIsNotPlayer = ls.palyerIsNotZombie;

        var humans = GameManager.Instance.humanBases;
        var t = 0;
        for (int i = 0; i < humans.Count; i++)
        {
            if (humans[i].IsMe)
                continue;
            humans[i].TryGetComponent(out ModelsGroup group);
            if (ls.aI_Datas.Count > t)
            {
                group.index = ls.aI_Datas[t];
                group?.Start();
            }
            t++;
        }
        if (PlayUI.IsAdsSelect.Value != -1)
        {
            ZombieShowTimer.Instance.SetZombieIsPlayer = PlayUI.IsAdsSelect.Value == 0;
            ZombieShowTimer.Instance.SetZombieIsNotPlayer = PlayUI.IsAdsSelect.Value == 1;
            PlayUI.IsAdsSelect.Value = -1;
        }
    }

    /// <summary>
    /// 出现杀手时应用游戏设置
    /// </summary>
    public void ApplyGameSetting()
    {
        var index = LevelSetting.Value;
        var dic = overriveSetting;
        if (dic == null)
            Debug.LogError(dic);
        if (!dic.ContainsKey(index))
            ApplySetting(defaultSetting);
        else
            ApplySetting(dic[index]);


        foreach (var hb in GameManager.Instance.humanBases)
        {
            if (hb.IsMe)
                continue;
            hb.p_Para.Human_BaseSpeed = Mathf.Clamp(GetSpeed(false), 0, 3);
            hb.p_Para.Kill_BaseSpeed = Mathf.Clamp(GetSpeed(true), 0, 3);
            hb.ApplyParameter();
        }
    }

    public float GetSpeed(bool isKiller)
    {
        if (Mathf.Approximately(SDKInit.DifficultyAB, 0f))
            return 2.1f;

        var myIsKiller = ZombieShowTimer.ZombiePlayer == PlayerControl.Instance;
        var mySpeed = myIsKiller ? PlayerControl.Instance.p_Para.Kill_BaseSpeed : PlayerControl.Instance.p_Para.Human_BaseSpeed;
        if (LevelSetting.Value <= 6)
        {
            if (Mathf.Approximately(SDKInit.DifficultyAB, 1f))
                return mySpeed + 0.1f;
            return mySpeed - 0.15f;
        }
        else if (LevelSetting.Value <= 9)
        {
            if (Mathf.Approximately(SDKInit.DifficultyAB, 1f))
                return mySpeed + 0.1f;
            return mySpeed - 0.1f;
        }
        else
        {
            //设置
            if (PlayerPrefs.GetInt("KaDian", 0) != LevelSetting.Value)
            {
                PlayerPrefs.SetInt("KaDian", LevelSetting.Value);
                PlayerPrefs.SetFloat("KaDianSpeedHuman", PlayerControl.Instance.p_Para.Human_BaseSpeed);
                PlayerPrefs.SetFloat("KaDianSpeedKiller", PlayerControl.Instance.p_Para.Kill_BaseSpeed);
            }

            var HumanMySpeed =  PlayerPrefs.GetFloat("KaDianSpeedHuman", PlayerControl.Instance.p_Para.Human_BaseSpeed);
            var KillerMySpeed = PlayerPrefs.GetFloat("KaDianSpeedKiller", PlayerControl.Instance.p_Para.Kill_BaseSpeed);
            //卡点
            if (LevelSetting.Value >= 13 && LevelSetting.Value % 3 == 1)
                return myIsKiller ? (KillerMySpeed + 0.05f) : (HumanMySpeed + .1f);
            //非卡点
            return myIsKiller ? (KillerMySpeed - 0.1f) : HumanMySpeed ;
        }
    }

    public void ApplySetting(LevelSaveSetting ls)
    {
        var humans = GameManager.Instance.humanBases;
        for (int i = 0; i < humans.Count; i++)
        {
            if (humans[i].IsMe)
                continue;
            humans[i].TryGetComponent(out ModelsGroup group);
            if (humans[i] == ZombieShowTimer.ZombiePlayer)
            {
                humans[i].p_Para.BaseAttackAngle = ls.BaseAttackAngle;
                humans[i].p_Para.BaseAttackDeltaTime = ls.BaseAttackDeltaTime;
                humans[i].p_Para.BaseAttackDistance = ls.BaseAttackDistance;
                group.index = ls.ZombieIndex;
            }
            humans[i].ApplyParameter();
        }

        if (PlayerControl.Instance.IsHuman)
            ZombieShowTimer.Instance.SetFieldOfViewData(
                ls.BaseViewLength,
                ls.BaseMinViewLength,
                ls.BaseViewAngle
            );
        else
        {
            var p_para = PlayerControl.Instance.p_Para;
            ZombieShowTimer.Instance.SetFieldOfViewData(
                p_para.BaseMaxViewRadius,
                p_para.BaseMinViewRadius,
                p_para.BaseViewAngle
            );
        }
    }

}
