
/****************************************************
 * FileName:		NewCharacterUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-13-22:58:54
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class NewCharacterUI : BasePanel
{
	//Trycharacter_SWATÌØ¾¯
	//Trycharacter_Chainsawµç¾â
	//Trycharacter_CowboyÅ£×Ð
	[System.Serializable]
	public class CharacterData
	{
		public string fbMsg;
		public GameObject model;
		public bool ishuman;
		public int index;
	}

	public Transform ccPos;

    private int lastLoadLevel = -1;
	private int index = -1;
	public List<CharacterData> characterDatas;

	private void CloseAllModel()
	{
		foreach (var cd in characterDatas)
			cd.model.gameObject.SetActive(false);
	}

	private void ShowCharacterData(int _index)
	{
		index = _index;
		var model = characterDatas[_index].model;
		if (!model.activeSelf)
		{
			CloseAllModel();
			model.gameObject.SetActive(true);
			model.transform.position = ccPos.position;
			model.transform.rotation = ccPos.rotation;
		}
	}

	private void Update()
	{
		ShowCharacterData(index);
        button_TryItBtn.interactable = SDKInit.Instance.RewardedAdsIsReady();
    }

	private bool IsBuy(int level)
	{
		if (characterDatas[level].ishuman)
			return PlayerPrefs.GetInt(UpgradeUI.BuyHuman + characterDatas[level].index.ToString(), 0) == 1;
		return PlayerPrefs.GetInt(UpgradeUI.BuyKiller + characterDatas[level].index.ToString(), 0) == 1;
	}


	#region UI Variable Statement 
	private Button button_closeBtn;
	private Button button_TryItBtn;
	#endregion

	#region UI Variable Assignment 
	private void InitUI()
	{
		button_closeBtn = transform.Find("closeBtn").GetComponent<Button>();
		button_TryItBtn = transform.Find("TryItBtn").GetComponent<Button>();
	}
	#endregion

	#region UI Event Register 
	private void AddEvent()
	{
		button_closeBtn.onClick.AddListener(OncloseBtnClicked);
		button_TryItBtn.onClick.AddListener(OnTryItBtnClicked);
	}

	private bool isinit = false;
	private void Awake()
	{
		if (isinit)
			return;
		isinit = true;
		InitUI();
		AddEvent();
		Messenger.AddListener(ConstValue.CallBackFun.OpenNewCharacterUI, OnEnter);
		gameObject.SetActive(false);
		ccPos.gameObject.SetActive(false);
        button_TryItBtn.transform.DOScale(1.2f, .5f).SetLoops(-1, LoopType.Yoyo);

    }


	private void OncloseBtnClicked()
	{
		OnExit();
	}

	private void OnTryItBtnClicked()
	{
		SDKInit.rewardCallback = () => {
			var ishuman = characterDatas[index].ishuman;
			var zst = ZombieShowTimer.Instance;
			zst.SetZombieIsNotPlayer = ishuman;
			zst.SetZombieIsPlayer = !zst.SetZombieIsNotPlayer ;
			if (ishuman)
				PlayerControl.Instance.modelsGroup.SetModel(0, characterDatas[index].index);
			else
				PlayerPrefs.SetInt(ConstValue.SaveDataStr.TryGetCharacter, index );
			OnExit();
		};
		SDKInit.rewardType = RewardType.Function;
		SDKInit.Instance.ShowRewardedAds(characterDatas[index].fbMsg);
        

    }


	public override void OnEnter()
	{
		index = -1;
		PlayerPrefs.SetInt(ConstValue.SaveDataStr.TryGetCharacter, -1);

        if (lastLoadLevel == LevelSetting.Value)
            return;
        lastLoadLevel = LevelSetting.Value;
        var level = LevelSetting.Value;
		if (level <= 4)
			return;
		level -= 5;
		if (level % 4 != 0)
			return;
		level = ( level / 4 ) % 3;
        if (IsBuy(level))
			return;
		gameObject.SetActive(true);
		ShowCharacterData(level);
		Model_Upgrade.Instance.gameObject.SetActive(true);
		Model_Upgrade.Instance.CloseAllModels();
		GameManager.Instance.GameStop();
        button_closeBtn.gameObject.SetActive(false);
        
        this.AttachTimer(2.5f, () => {
            button_closeBtn.gameObject.SetActive(true);
            button_closeBtn.transform.DOKill();
            button_closeBtn.transform.localScale = Vector3.zero;
            button_closeBtn.transform.DOScale(1, .5f);
        } );
		ccPos.gameObject.SetActive(true);
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
		ccPos.gameObject.SetActive(false);
		GameManager.Instance.GameContinue();
		Model_Upgrade.Instance.gameObject.SetActive(false);
	}
	#endregion

}
