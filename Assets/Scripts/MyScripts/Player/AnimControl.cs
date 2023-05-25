
/****************************************************
 * FileName:		AnimControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-11-16:20:04
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimControl : MonoBehaviour, IHideable
{

    #region --- Public Variable ---

    #endregion


    #region --- Private Variable ---
    private HumanBase humanBase;
    private ModelsGroup modelsGroup;
    #endregion



    void Awake()
    {
        TryGetComponent(out humanBase);
        TryGetComponent(out modelsGroup);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (PlayerControl.Instance.IsZombie && ! humanBase.IsMe )
        {
            if (!humanBase.BeDiscovered)
                delaySetM.OnUpdate(Time.deltaTime);
            delaySigned.OnUpdate(delaySetM.IsFinish);
            if (delaySigned.IsPressDown)
                SetMaterial(0);
        }
        if (GameManager.isWin || GameManager.isDead || humanBase.IsDead)
            SetMaterial(1);
    }

    private SignedTimer disSigned = new SignedTimer();

    #region 设置速度

    private float lastVelocity;
    private float lastState;
    public void SetAnimVelocity(float velo, float state, float deltaTime)
    {
        if (!modelsGroup)
            return;
        float t1 = Mathf.Lerp(lastVelocity, velo, deltaTime * 10);
        float t2 = Mathf.Lerp(lastState, state, deltaTime * 10);
        SetValue(ConstValue.AnimatorStr.velocity, t1);
        SetValue(ConstValue.AnimatorStr.State, t2);
        lastVelocity = t1;
        lastState = t2;
    }

    #endregion


    #region Set Animation Value

    public void SetValue(string str, float value)
    {
        modelsGroup?.anim?.SetFloat(str, value);
    }

    public void SetValue(string str, int value)
    {
        modelsGroup?.anim?.SetInteger(str, value);
    }

    public void SetValue(string str)
    {
        modelsGroup?.anim?.SetTrigger(str);
    }

    public void SetValue(string str, bool value)
    {
        modelsGroup?.anim?.SetBool(str, value);
    }

    public bool GetValue(string str)
    {
        return modelsGroup.anim.GetBool(str);
    }


    #endregion

    private MyTimer delaySetM = new MyTimer();
    private SignedTimer delaySigned = new SignedTimer();

    public void SetMaterial(float alpha)
    {
        if (modelsGroup.mat == null)
            return;

        var c = modelsGroup.mat[0].GetColor(ConstValue._Color) ;
        c.a = alpha;
        foreach (var mt in modelsGroup.mat)
            mt.SetColor(ConstValue._Color, c);
    }

    public void SetMaterialOpaque()
    {
        if (modelsGroup.mat == null)
            return;
        
        foreach (var mt in modelsGroup.mat)
        {
            mt.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mt.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mt.SetInt("_ZWrite", 1);
            mt.DisableKeyword("_ALPHATEST_ON");
            mt.DisableKeyword("_ALPHABLEND_ON");
            mt.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mt.renderQueue = -1;
        }
    }

    public void SetMaterialTransparent()
    {
        if (modelsGroup.mat == null)
            return;

        foreach (var mt in modelsGroup.mat)
        {
            mt.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mt.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mt.SetInt("_ZWrite", 0);
            mt.DisableKeyword("_ALPHATEST_ON");
            mt.DisableKeyword("_ALPHABLEND_ON");
            mt.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mt.renderQueue = 3000;
        }
    }


    public void OnFOVEnter()
    {
        if (modelsGroup.mat.Count == 0)
            return ;
        if (!ZombieShowTimer.HasZombie)
            return;
        
        //如果玩家是丧尸
        if (PlayerControl.Instance.IsZombie)
        {
            delaySetM.ReStart();
            SetMaterial(1);
        }

        var targets = ZombieShowTimer.ZombiePlayer.targets;
        if (!targets.Contains(humanBase))
            targets.Add(humanBase);
        
        humanBase.BeDiscovered = true;
        humanBase.State = -1;
        disSigned.OnUpdate(humanBase.BeDiscovered);
        if (humanBase.IsMe && disSigned.IsPressDown)
        {
            AudioManager.Instance.PlayMusic(102,true);
        }
    }

    public void OnFOVLeave()
    {
        if (modelsGroup.mat.Count == 0)
            return;

        if (!ZombieShowTimer.HasZombie)
            return;
        if (humanBase.IsZombie)
            return;
        //如果玩家是丧尸
        if (PlayerControl.Instance.IsZombie)
        {
            
        }

        var targets = ZombieShowTimer.ZombiePlayer.targets;
        if (targets.Contains(humanBase))
            targets.Remove(humanBase);
        
        humanBase.BeDiscovered = false;
        humanBase.State = 0;
        disSigned.OnUpdate(humanBase.BeDiscovered);
        if (humanBase.IsMe && disSigned.IsPressUp)
        {
            AudioManager.Instance.PlayMusic(101, true);
        }
    }
}
