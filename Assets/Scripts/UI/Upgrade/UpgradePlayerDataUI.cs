
/****************************************************
 * FileName:		UpgradePlayerDataUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-03-14:27:37
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		显示数据
 * 
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using Facebook.Unity;
using DG.Tweening;

public class UpgradePlayerDataUI : MonoBehaviour
{

    public int addText; //增幅
    public int baseMoney ;//基础金币
    private ReactiveProperty<int> level;
    public ReactiveProperty<Sprite> sprite;
    public ReactiveProperty<string> _name;
    public ReactiveProperty<string> _SaveName;

    public Sprite[] rays;


    #region UI Variable Statement 
    private Text text_dataName; 
	private Image image_sprite; 
	private Image image_ray0; 
	private Image image_ray1; 
	private Image image_ray2; 
	private Button button_addLevel; 
	private Text text_value; 
	private Text text_addText; 
	private Image image_levelMax;

    private Button adsBtn;
    private GameObject adsFreeText;
    private GameObject adsUpgradeText;

    private Button CoinBtn;
    private Text CoinBtnValue;

    #endregion

    #region UI Variable Assignment 
    private bool isinit = false;
	public void InitUI() 
	{
        if (isinit)
            return;
        transform.Find("dataName").TryGetComponent(out text_dataName);
        transform.Find("sprite").TryGetComponent(out image_sprite);
        transform.Find("Data/0/ray0").TryGetComponent(out image_ray0);
        transform.Find("Data/1/ray1").TryGetComponent(out image_ray1);
        transform.Find("Data/2/ray2").TryGetComponent(out image_ray2);
        transform.Find("addLevel").TryGetComponent(out button_addLevel);
        transform.Find("addLevel/value").TryGetComponent(out text_value);
        transform.Find("addText").TryGetComponent(out text_addText);
        transform.Find("levelMax").TryGetComponent(out image_levelMax);
        transform.Find("adsBtn").TryGetComponent(out adsBtn);
        adsFreeText = transform.Find("adsBtn/AdsText").gameObject;
        adsUpgradeText = transform.Find("adsBtn/UpgradeText").gameObject;
        transform.Find("CoinBtn").TryGetComponent(out CoinBtn);
        CoinBtn.OnClickAsObservable().Subscribe(
            _ =>
            {
                UIAnim(CoinBtn.transform);
                if (GameSetting.Coin.Value >= LevelGetCoin(level.Value))
                {
                    GameSetting.Coin.Value -= LevelGetCoin(level.Value);
                    SetUpgrade(1);
                    ChangeData(level.Value);
                }
            }
            
            );
        CoinBtn.transform.Find("value").TryGetComponent(out CoinBtnValue);

        level = new ReactiveProperty<int>();
        sprite = new ReactiveProperty<Sprite>();
        _name = new ReactiveProperty<string>();
        _SaveName = new ReactiveProperty<string>();

        level.Subscribe(ChangeData);
        level.Subscribe( a=> { Messenger.Broadcast(ConstValue.CallBackFun.FBSendAllHumansData); });
        sprite.Subscribe(_ => { if (_) image_sprite.sprite = _; });
        _name.Subscribe(_ => { if (_!="") text_dataName.text = _; });
        _SaveName.Subscribe(_ => { if (_ != "") level.Value = PlayerPrefs.GetInt(_); });


        button_addLevel.OnClickAsObservable().Subscribe(_ => { UIAnim(button_addLevel.transform); TryAddLevel(); });

        SceneMainUI.Instance.InitUI();
        GameSetting.Money.Property.Subscribe(_ => CheckButton(level.Value));

        adsBtn.OnClickAsObservable().Subscribe(
            _ => {
                UIAnim(adsBtn.transform);
                if (SDKInit.Instance.RewardedAdsIsReady())
                {
                    SDKInit.rewardType = RewardType.Function;
                    SDKInit.rewardCallback = () => {
                        PlayUI.IsTriggerPlayBtn = false;
                        if (level.Value != 0 && level.Value % 3 == 0 && GetUpgrade() == 0)
                        {
                            SetUpgrade(1);
                            Debug.Log("突破升级");
                        }

                        else
                        {
                            level.Value = Mathf.Min(level.Value + 1, 9);
                            Messenger.Broadcast(ConstValue.CallBackFun.CharcterUpgrade, level.Value);
                        }
                        ChangeData(level.Value);
                    };
                    SDKInit.Instance.ShowRewardedAds(level.Value % 3 != 0 ? "levelup" : "Upgrade");
                }
            }
            );

        isinit = true;
    }

    private void UIAnim(Transform trans)
    {
        trans.DOKill();
        trans.localScale = Vector3.one;
        trans.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
    }

    private int LevelGetMoney(int level)
    {
        return Mathf.RoundToInt(baseMoney * Mathf.Pow(1.4f, level )) + 20 * level;
    }
    private int LevelGetCoin(int level)
    {
        return level / 3 * 5;
    }
    private int LevelGetaddText(int level)
    {
        var yu = addText / 3 % 3;
        return addText / 9 * level + level / 3 * yu ;
    }

    private void TryAddLevel()
    {
        var needMoney = LevelGetMoney(level.Value);
        if (GameSetting.Money.Value >= needMoney)
        {
            level.Value = Mathf.Min(level.Value + 1, 9);
            Debug.Log("技能等级升级，当前等级：" + level.Value);
            Messenger.Broadcast(ConstValue.CallBackFun.CharcterUpgrade, level.Value);
            GameSetting.Money.Value -= needMoney;

            var str = "levelup_gold";
            if (PlayerPrefs.GetInt(str, 0) == 0)
            {
                FB.LogAppEvent(str, null, null);
                PlayerPrefs.SetInt(str, 1);
            }
        }
        //钱不够，跳转
        else
        {
            //SceneMainUI.Instance.index.Value = 0;
        }
    }

    private void CheckButton(int _level)
    {
        if (_level >= 9)
        {
            image_levelMax.gameObject.SetActive(true);
            adsBtn.gameObject.SetActive(false);
            button_addLevel.gameObject.SetActive(false);
            CoinBtn.gameObject.SetActive(false);
        }
        else
        {
            var needMoney = LevelGetMoney(_level);
            //进阶
            if (_level>=1 && _level %3 == 0 && GetUpgrade() == 0)
            {
                if (true)
                {
                    adsBtn.gameObject.SetActive(true);
                    image_levelMax.gameObject.SetActive(false);
                    button_addLevel.gameObject.SetActive(false);
                    adsFreeText.gameObject.SetActive(false);
                    adsUpgradeText.gameObject.SetActive(true);
                    CoinBtn.gameObject.SetActive(false);
                }
                else
                {
                    adsBtn.gameObject.SetActive(false);
                    image_levelMax.gameObject.SetActive(false);
                    button_addLevel.gameObject.SetActive(false);
                    CoinBtn.gameObject.SetActive(true);
                    CoinBtnValue.text = LevelGetCoin(_level).ToString();
                }
            }
            //Free广告
            else if (PlayUI.IsTriggerPlayBtn && GameSetting.Money.Value < needMoney)
            {
                image_levelMax.gameObject.SetActive(false);
                adsBtn.gameObject.SetActive(true);
                adsFreeText.gameObject.SetActive(true);
                adsUpgradeText.gameObject.SetActive(false);
                button_addLevel.gameObject.SetActive(false);
                CoinBtn.gameObject.SetActive(false);
            }
            //金币
            else
            {
                image_levelMax.gameObject.SetActive(false);
                adsBtn.gameObject.SetActive(false);
                button_addLevel.gameObject.SetActive(true);
                CoinBtn.gameObject.SetActive(false);
            }
        }
    }

    private bool IsUpgrade()
    {
        return GetUpgrade() == 1;
    }
    private int GetUpgrade()
    {
        return PlayerPrefs.GetInt(_SaveName.Value + "Upgrade");
    }
    private void SetUpgrade(int value)
    {
        PlayerPrefs.SetInt(_SaveName.Value + "Upgrade", value);
    }

    private void ChangeData(int _level)
    {
        if (_SaveName.Value != "")
        {
            PlayerPrefs.SetInt(_SaveName.Value, _level);
            if (_level % 3 != 0)
                SetUpgrade(0);
        }


        text_addText.text = "+" + LevelGetaddText(_level).ToString() + "%";

        text_value.text = LevelGetMoney(_level).ToString();

        var index = 0;
        switch (_level)
        {
            case 0:case 1:case 2:
                index = 0;
                break;
            case 3:
                index = IsUpgrade() ? 1 : 0;
                break;
            case 4:case 5:
                index = 1;
                break;
            case 6:
                index = IsUpgrade() ? 2 : 1;
                break;
            default:
                index = 2;
                break;
        }

        image_ray0.sprite = rays[index];
        image_ray1.sprite = rays[index];
        image_ray2.sprite = rays[index];

        if (!IsUpgrade())
        {
            image_ray0.gameObject.SetActive(_level != 0 && _level % 3 >= 0);
            image_ray1.gameObject.SetActive(_level != 0 && _level % 3 != 1);
            image_ray2.gameObject.SetActive(_level != 0 && _level % 3 == 0);
        }
        else
        {
            image_ray0.gameObject.SetActive(_level % 3 != 0);
            image_ray1.gameObject.SetActive(_level % 3 == 2);
            image_ray2.gameObject.SetActive(false);
        }
        
        CheckButton(_level);
    }

    #endregion

    private void Awake()
    {
        InitUI();
    }

}
