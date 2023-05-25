
/****************************************************
 * FileName:		UpgradeTeamUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-03-10:22:23
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------------------------
/**
* @author :
* date    :
* purpose :
*/
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using System.IO;
public class UpgradeTeamUI : Singleton<UpgradeTeamUI>
{
    
    #region UI Variable Statement 
    private Button button_RightFalse; 
	private Button button_leftTrue; 
	private Button button_leftFalse; 
	private Button button_RightTrue; 
	private Image killer_Image; 
	private Text killer_Text; 
	private Image human_Image; 
	private Text human_Text;
    #endregion

    #region UI Variable Assignment 
    private bool isinit = false;
    private Transform killerImage;
    private Transform humanImage;


    public void InitUI() 
	{
        if (isinit)
            return;
        isinit = true;

        button_RightFalse = transform.Find("RightFalse").GetComponent<Button>(); 
		button_leftTrue = transform.Find("leftTrue").GetComponent<Button>(); 
		button_leftFalse = transform.Find("leftFalse").GetComponent<Button>(); 
		button_RightTrue = transform.Find("RightTrue").GetComponent<Button>();

        killerImage = transform.Find("killerImage");
        humanImage = transform.Find("humanImage");

        killer_Image = transform.Find("killerImage/Image").GetComponent<Image>();
        killer_Text = transform.Find("killerImage/Text").GetComponent<Text>();
        human_Image = transform.Find("humanImage/Image").GetComponent<Image>();
        human_Text = transform.Find("humanImage/Text").GetComponent<Text>();

        button_RightFalse.OnClickAsObservable().Subscribe(_=> { leftTeam.Value = false; });
        button_leftFalse.OnClickAsObservable().Subscribe (_=> { leftTeam.Value = true;  });

        leftTeam = new BoolReactiveProperty
        {
            Value = false
        };
        leftTeam.Subscribe(ChangeTeamData);
        

    }
    #endregion

    public BoolReactiveProperty leftTeam;
    private void Awake()
    {
        InitUI();
    }

    public Color trueColor;
    public Color falseColor;
    private void ChangeTeamData(bool isLeft)
    {
        killer_Image.color = isLeft ? trueColor : falseColor;
        killer_Text.color = killer_Image.color;
        human_Image.color = !isLeft ? trueColor : falseColor;
        human_Text.color = human_Image.color;

        button_leftTrue.gameObject.SetActive(isLeft);
        button_leftFalse.gameObject.SetActive(!isLeft);
        button_RightFalse.gameObject.SetActive(isLeft);
        button_RightTrue.gameObject.SetActive(!isLeft);

        if (button_leftTrue.gameObject.activeSelf)
            killerImage.parent = button_leftTrue.transform;
        if (button_leftFalse.gameObject.activeSelf)
            killerImage.parent = button_leftFalse.transform;

        if (button_RightFalse.gameObject.activeSelf)
            humanImage.parent = button_RightFalse.transform;
        if (button_RightTrue.gameObject.activeSelf)
            humanImage.parent = button_RightTrue.transform;
    }

}
