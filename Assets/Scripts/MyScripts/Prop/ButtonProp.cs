
/****************************************************
 * FileName:		ButtonProp.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-12-17:32:50
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonProp : PropBase
{

    #region --- Public Variable ---
    public float targetY = 0;

    #endregion


    #region --- Private Variable ---
    private Material mat;

    override public void PlayAction(HumanBase human)
    {
        if (coll.enabled == false)
            return;
        if (human == PlayerControl.Instance)
            GameManager.Instance.gameDataCount.clickTrigger++;
        Messenger.Broadcast<HumanBase,ButtonProp>(ConstValue.CallBackFun.ButtonDown, human, this );
        StartCoroutine(ButtonToDown());
    }

    private IEnumerator ButtonToDown()
    {
        float timer = 0;
        float maxTimer = .3f;
        float startY = model.position.y;
        targetY = transform.position.y - targetY;
        for (; timer < maxTimer ; )
        {
            float curve = timer / maxTimer;
            model.SetY( Mathf.Lerp(startY , targetY , curve) );
            mat.SetColor(ConstValue._Color, Color.Lerp( Color.red , Color.green , curve));
            timer += Time.deltaTime;
            yield return null;
        }
        model.SetY( targetY);
    }

    #endregion

    override protected void Awake()
    {
        base.Awake();
        mat = model.GetComponent<MeshRenderer>().material;
    }

    public void SetColl(bool falg)
    {
        if (!coll)
            TryGetComponent(out coll);
        coll.enabled = falg;
    }

}
