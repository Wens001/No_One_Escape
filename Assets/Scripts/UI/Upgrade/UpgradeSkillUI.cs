
/****************************************************
 * FileName:		UpgradeSkillUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-03-10:45:07
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
public class PlayerSkillData
{
    public Sprite sprite;
    public string playerName;
    public string skillName;
    public string skillLabel;
}
public class UpgradeSkillUI : MonoBehaviour
{

	#region UI Variable Statement 
	public Image image_skillImage { get; private set; }
    public Text text_playerName { get; private set; }
    public Text text_skillName { get; private set; }
    public Text text_SkillLabel { get; private set; }

    public GameObject image_bg1 { get; private set; }
    public GameObject image_bg2 { get; private set; }

    #endregion

    #region UI Variable Assignment 
    private bool isinit = false;
	public void InitUI() 
	{
        if (isinit)
            return;
		image_skillImage = transform.Find("skillImage").GetComponent<Image>(); 
		text_playerName = transform.Find("playerName").GetComponent<Text>(); 
		text_skillName = transform.Find("skillName").GetComponent<Text>(); 
		text_SkillLabel = transform.Find("SkillLabel").GetComponent<Text>();
        image_bg1 = transform.Find("skillBG1").gameObject;
        image_bg2 = transform.Find("skillBG2").gameObject;
        data = new ReactiveProperty<PlayerSkillData>();
        data.Subscribe(ChangeData);
        isinit = true;
    }
    #endregion

    public void SetSkillData(PlayerSkillData psd)
    {
        InitUI();
        data.Value = psd;
    }

    private ReactiveProperty<PlayerSkillData> data;

    private void Awake()
    {
        InitUI();
    }

    private void ChangeData(PlayerSkillData value)
    {
        if (value == null)
            return;
        image_skillImage.sprite = value.sprite;
        text_playerName.text = value.playerName;
        text_skillName.text = value.skillName;
        text_SkillLabel.text = value.skillLabel;
    }

}
