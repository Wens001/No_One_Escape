
/****************************************************
 * FileName:		BloodEffectPanelUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-26-13:41:38
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BloodEffectPanelUI : BasePanel
{
    public float timer = .5f;
    public AnimationCurve AlphaCurve = new AnimationCurve();

	#region UI Variable Statement 
	private Image image_Image;
    private Color _color;
    private bool beDiscovered ;
    private MyTimer myTimer ;
    #endregion

    #region UI Variable Assignment 
    private void InitUI() 
	{
		image_Image = transform.Find("Image").GetComponent<Image>(); 

	}
	#endregion 

	#region UI Event Register 

    private void Awake()
    {
        InitUI();
        _color = Color.white;
        _color.a = 0;
        image_Image.color = _color;
        myTimer = new MyTimer(timer);
    }

    private void Update()
    {
        if (!ZombieShowTimer.HasZombie|| GameManager.isWin || GameManager.isDead || GameManager.Speed <0.1f)
        {
            _color.a = 0;
            image_Image.color = _color;
            return;
        }
        if (PlayerControl.Instance.IsZombie)
        {
            _color.a = 0;
            image_Image.color = _color;
            return;
        }
        if (PlayerControl.Instance.BeDiscovered)
        {
            myTimer.OnUpdate(GameManager.DeltaTime);
            if (myTimer.IsFinish)
                myTimer.ReStart();
            _color.a = Mathf.Lerp(_color.a,AlphaCurve.Evaluate(myTimer.GetRatioComplete),GameManager.DeltaTime * 5) ;
        }
        else
        {
            _color.a = Mathf.Lerp(_color.a,0, Time.deltaTime * 5);
        }
        image_Image.color = _color;
    }

    public override void OnEnter()
    {
        gameObject.SetActive(true);
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
    }
    #endregion

}
