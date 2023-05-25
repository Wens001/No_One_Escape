
/****************************************************
 * FileName:		Model_Upgrade.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-03-15:32:48
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class Model_Upgrade : Singleton<Model_Upgrade>
{
    [System.Serializable]
    public class ModelMessage
    {
        public GameObject model;
        public string name ;
        public string nameChinese;
        public string skillName;
        public string skillNameChinese;
        public Sprite sprite;
        [Multiline]
        public string describe;
        [Multiline]
        public string describeChinese;
        public int needMoney;
    }

    #region --- Public Variable ---

    public UpgradeTeamUI teamUI;
    public UpgradeSkillUI skillUI;
    public UpgradeUI upgradeUI;


    public ModelMessage[] humanModels;
    public ModelMessage[] killerModels;

    public ReactiveProperty<int> humanIndex;
    public ReactiveProperty<int> killerIndex;

    #endregion

    private Transform UpgradeTrans;
    private Transform leftTrans;
    private Transform rightTrans;


    #region --- Private Variable ---
    private bool isinit;
    public void Init()
    {
        if (isinit)
            return;
        isinit = true;

        UpgradeTrans = transform.Find("UpgradePos");
        leftTrans = transform.Find("LeftPos");
        rightTrans = transform.Find("RightPos");



        humanIndex = new ReactiveProperty<int>
        { Value = PlayerPrefs.GetInt(ConstValue.SaveDataStr.HumanIndex,0) };
        killerIndex = new ReactiveProperty<int>
        { Value = PlayerPrefs.GetInt(ConstValue.SaveDataStr.KillerIndex, 0) };

        humanIndex.Subscribe(_ => ShowModel());
        killerIndex.Subscribe(_ => ShowModel());
        teamUI.InitUI();
        teamUI.leftTeam.Subscribe(_ => ShowModel());

        ShowPlayModel();
    }

    #endregion

    public void ShowPlayModel()
    {
        Init();

        CloseAllModels();
        {
            var killerx = PlayerPrefs.GetInt(ConstValue.SaveDataStr.KillerIndex, 0);
            killerModels[killerx].model.SetActive(true);
            killerModels[killerx].model.transform.position = leftTrans.position;
            killerModels[killerx].model.transform.rotation = leftTrans.rotation;

        }
        {
            var humanx = PlayerPrefs.GetInt(ConstValue.SaveDataStr.HumanIndex, 0);
            humanModels[humanx].model.SetActive(true);
            humanModels[humanx].model.transform.position = rightTrans.position;
            humanModels[humanx].model.transform.rotation = rightTrans.rotation;
        }
        
    }

    public void CloseAllModels()
    {
        foreach (var md in humanModels)
            md.model.SetActive(false);
        foreach (var md in killerModels)
            md.model.SetActive(false);
    }

    public void ShowModel()
    {
        Init();
        CloseAllModels();
        if (teamUI.leftTeam.Value)
        {
            killerModels[killerIndex.Value].model.SetActive(true);
            killerModels[killerIndex.Value].model.transform.position = UpgradeTrans.position;
            killerModels[killerIndex.Value].model.transform.rotation = UpgradeTrans.rotation;

            SetSkillStringData(killerModels[killerIndex.Value]);
            upgradeUI.ChangeTeamData(teamUI.leftTeam.Value, killerIndex.Value);
        }
        else
        {
            humanModels[humanIndex.Value].model.SetActive(true);
            humanModels[humanIndex.Value].model.transform.position = UpgradeTrans.position;
            humanModels[humanIndex.Value].model.transform.rotation = UpgradeTrans.rotation;

            SetSkillStringData(humanModels[humanIndex.Value]);
            upgradeUI.ChangeTeamData(teamUI.leftTeam.Value, humanIndex.Value);
        }
    }

    private void SetSkillStringData(ModelMessage msg)
    {
        skillUI.InitUI();
        skillUI.image_skillImage.gameObject.SetActive(msg.sprite);
        if (msg.sprite)
            skillUI.image_skillImage.sprite = msg.sprite;
        skillUI.image_skillImage.gameObject.SetActive(msg.sprite);
        skillUI.image_bg1.SetActive(msg.sprite);
        skillUI.image_bg2.SetActive(!msg.sprite);
        skillUI.text_playerName.text = EnterScene.IsChinese.Value ? msg.nameChinese : msg.name;
        skillUI.text_skillName.text = EnterScene.IsChinese.Value ? msg.skillNameChinese : msg.skillName;
        skillUI.text_SkillLabel.text = EnterScene.IsChinese.Value ? msg.describeChinese : msg.describe;
    }

    private void ChangeLeftArrow(ReactiveProperty<int> rp,int max)
    {
        var t = rp.Value;
        t = (t - 1) < 0 ? max : t - 1;
        rp.Value = t;
    }
    private void ChangeRightArrow(ReactiveProperty<int> rp, int max)
    {
        var t = rp.Value;
        t = (t + 1) > max ? 0 : t + 1;
        rp.Value = t;
    }

    public void LeftArrorButton()
    {
        Init();
        if (teamUI.leftTeam.Value)
            ChangeLeftArrow(killerIndex, killerModels.Length - 1);
        else
            ChangeLeftArrow(humanIndex, humanModels.Length - 1);
    }
    
    public void RightArrorButton()
    {
        Init();
        if (teamUI.leftTeam.Value)
            ChangeRightArrow(killerIndex, killerModels.Length - 1);
        else
            ChangeRightArrow(humanIndex, humanModels.Length - 1);
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {

        SceneMainUI.Instance.InitUI();
        SceneMainUI.Instance.index.Subscribe(
            a => {
                leftTrans.gameObject.SetActive(a == 2);
                rightTrans.gameObject.SetActive(a == 2);
                UpgradeTrans.gameObject.SetActive(a == 3);
            }
            );
    }

}
