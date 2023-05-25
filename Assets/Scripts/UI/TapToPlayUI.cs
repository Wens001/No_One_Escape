
/****************************************************
 * FileName:		TapToPlayUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-26-10:57:10
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class TapToPlayUI : BasePanel
{

	#region UI Variable Statement 
	private Image image_TapToPlay; 
	private Button button_TapToPlay; 
	private Image image_Image; 
	#endregion 

	#region UI Variable Assignment
	private void InitUI() 
	{
		image_TapToPlay = transform.GetComponent<Image>(); 
		button_TapToPlay = transform.GetComponent<Button>(); 
		image_Image = transform.Find("Image").GetComponent<Image>(); 
	}
	#endregion 

	#region UI Event Register 
	private void AddEvent() 
	{
		button_TapToPlay.onClick.AddListener(OnTapToPlayClicked);
	}
 

	private void OnTapToPlayClicked()
	{

    }

    private void Awake()
    {
        InitUI();
        AddEvent();
    }

    private bool isListener = false;
    private void Update()
    {
        if (isListener && Input.GetMouseButtonDown(0) &&! SceneMainUI.IsShow)
        {
            GameManager.Instance.GameContinue();
            UIPanelManager.Instance.PopPanel();
            isListener = false;
        }
    }

    public override void OnEnter()
    {
        gameObject.SetActive(true);
        isListener = true;
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
        isListener = false;
    }
    #endregion

}
