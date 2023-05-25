
/****************************************************
 * FileName:		ModelsGroup.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-09-14:36:04
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ModelsGroup : MonoBehaviour
{

    #region --- Public Variable ---

    public ModelObjects modelObjects;

    /// <summary>
    /// 武器
    /// </summary>
    public CasqueControl weaponControl { get {
            if (!Parameter) return null;
            return Parameter.weaponControl;
        } }

    /// <summary>
    /// 头盔
    /// </summary>
    public CasqueControl casqueControl
    {
        get
        {
            if (!Parameter) return null;
            return Parameter.casqueControl;
        }
    }

    public Animator anim
    {
        get
        {
            if (!Parameter) return null;
            return Parameter.anim;
        }
    }

    public List<Material> mat
    {
        get
        {
            return Parameter?.mat;
        }
    }

    #endregion



    public GameObject Model { get; private set; }
    public ModelParameter Parameter { get; private set; }

    public int index = -1;
    private int lastIndex = -1;
    private HumanBase player;

    private bool isinit = false;
    public void Start()
    {
        if (isinit)
            return;
        isinit = true;
        
        TryGetComponent(out player);

        if (player.name != "Player")
        {
            if (index == -1)
                SetRandomModel(0);
            else
                SetModel(0,index);
        }
        else
        {
            index = PlayerPrefs.GetInt(ConstValue.SaveDataStr.HumanIndex);
            SetModel(0, index);
        }
        
    }

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, KillerShow);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, KillerShow);
    }

    /// <summary>
    /// 变成杀手后替换模型
    /// </summary>
    /// <param name="human"></param>
    private void KillerShow(HumanBase human)
    {
        if (human != player)
            return;
        if (player.IsMe == false)
        {
            if (lastIndex == 0)
                return;
            if (index != -1)
                SetModel(1, index);
            else
                SetRandomModel(1);
            if (index == 1)
                this.AttachTimer(.1f,()=> 
                Messenger.Broadcast<float>(ConstValue.CallBackFun.KillerOtherAudio, 
                PlayerPrefs.GetFloat("SoundVolume", 1)));
            return;
        }

        var newchara = PlayerPrefs.GetInt(ConstValue.SaveDataStr.TryGetCharacter, -1);
        if (newchara != -1)
        {
            index = newchara;
        }
        else
        {
            //更改Player模型
            index = PlayerPrefs.GetInt(ConstValue.SaveDataStr.KillerIndex);
        }
        SetModel(1, index);
        if (index == 1)
            this.AttachTimer(.1f, () =>
             Messenger.Broadcast<float>(ConstValue.CallBackFun.KillerOtherAudio,
             PlayerPrefs.GetFloat("SoundVolume", 1)));
    }


    public void SetRandomModel(int team)
    {
        if (Model)
            Destroy(Model);
        Model = modelObjects.ShowRandomModel(team, transform, player,out index);
        Parameter = Model.GetComponent<ModelParameter>();
        lastIndex = index;
    }
    public void SetModel(int team,int index)
    {
        if (Model)
            Destroy(Model);
        Model = modelObjects.ShowModel(team, index, transform,player);
        Parameter = Model.GetComponent<ModelParameter>();
    }

}
