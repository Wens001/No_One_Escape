
/****************************************************
 * FileName:		ShoppingUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-06-11:38:10
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;
public class ShoppingUI : BasePanel
{


    private Transform Gens;
    private Transform Coins;

    private Button[] moneyButtons;
    private Button[] coinButtons;

    private bool isinit = false;
    private void Awake()
    {
        if (isinit)
            return;
        isinit = true;
        Gens = transform.Find("Gems");
        Coins = transform.Find("Coins");

        moneyButtons = new Button[6];
        for (int i = 0; i < moneyButtons.Length; i++)
            Gens.Find("btns/btn"+i.ToString()).TryGetComponent(out moneyButtons[i]);

        for (int i = 0; i < 5; i++)
        {
            var t = i;
            moneyButtons[i].OnClickAsObservable().Subscribe( _=> {
                BuyMoney(moneyButtons[t].transform,t);
            } );
        }
        moneyButtons[0].OnClickAsObservable().Subscribe(_ => { SDKInit.Instance.BuyProduct(ShopProductNames.gems0); });
        moneyButtons[1].OnClickAsObservable().Subscribe(_ => { SDKInit.Instance.BuyProduct(ShopProductNames.gems1); });
        moneyButtons[2].OnClickAsObservable().Subscribe(_ => { SDKInit.Instance.BuyProduct(ShopProductNames.gems2); });
        moneyButtons[3].OnClickAsObservable().Subscribe(_ => { SDKInit.Instance.BuyProduct(ShopProductNames.gems3); });
        moneyButtons[4].OnClickAsObservable().Subscribe(_ => { SDKInit.Instance.BuyProduct(ShopProductNames.gems4); });


        moneyButtons[5].OnClickAsObservable().Subscribe(
            _ => {
                moneyButtons[5].TryGetComponent(out CountdownUI cdui);
                UIAnim(moneyButtons[5].transform);
                SDKInit.rewardCallback = () => { GameSetting.Coin.Value += 5; cdui.OnClick(); };
                SDKInit.rewardType = RewardType.Function;
                SDKInit.Instance.ShowRewardedAds("freegems_shop");
        });


        coinButtons = new Button[3];
        Coins.Find("btns/btn0").TryGetComponent(out coinButtons[0]);
        Coins.Find("btns/btn1").TryGetComponent(out coinButtons[1]);
        Coins.Find("btns/btn2").TryGetComponent(out coinButtons[2]);
        coinButtons[0].OnClickAsObservable().Subscribe( _=> TryBuyMoney(coinButtons[0].transform,20,300));
        coinButtons[1].OnClickAsObservable().Subscribe( _=> TryBuyMoney(coinButtons[1].transform,50,800));
        coinButtons[2].OnClickAsObservable().Subscribe(
            _ => {
                coinButtons[2].TryGetComponent(out CountdownUI cdui);
                UIAnim(coinButtons[2].transform);
                SDKInit.rewardCallback = ()=> { GameSetting.Money.Value += 200; cdui.OnClick(); };
                SDKInit.rewardType = RewardType.Function;
                SDKInit.Instance.ShowRewardedAds("freegold_shop");
            }
        );
    }

    private void BuyMoney(Transform trans,int index)
    {
        UIAnim(trans);
    }

    private void UIAnim(Transform trans)
    {
        trans.DOKill();
        trans.localScale = Vector3.one;
        trans.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
    }

    private void TryBuyMoney(Transform trans,int coin,int money)
    {
        UIAnim(trans);
        if (GameSetting.Coin.Value >= coin)
        {
            GameSetting.Coin.Value -= coin;
            GameSetting.Money.Value += money;
        }
        else
        {
            //insufficientUI.Instance.Show();
        }
    }

    public override void OnEnter()
    {
        Awake();
        Model_Upgrade.Instance.gameObject.SetActive(false);
        gameObject.SetActive(true);

    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

}
