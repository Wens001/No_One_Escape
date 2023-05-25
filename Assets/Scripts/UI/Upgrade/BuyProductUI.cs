
/****************************************************
 * FileName:		BuyProductUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-22-14:49:07
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class BuyProductUI : MonoBehaviour
{
    #region --- Private Variable ---

    public int IsBuyIndex;
    public Sprite IsBuySprite;


    private const string IsBuyKey = "IsBuyKey";
    private Text coinValue;
    private Text money;
    private GameObject noads;
    private Image bg2;
    private GameObject bg2Text;

    #endregion

    private void Awake()
    {
        transform.Find("bgs/bg2").TryGetComponent(out bg2);
        bg2Text = bg2.transform.GetChild(0).gameObject;
        noads = transform.Find("bgs/noads").gameObject;

        if (PlayerPrefs.GetInt(IsBuyKey + IsBuyIndex.ToString(),0) == 1)
            SetIsBuy(IsBuyIndex);
        Messenger.AddListener<int>(ConstValue.CallBackFun.BuyCoin,SetIsBuy);
        if (SDKInit.NOADS.Value == 1)
            ClearADSImage();
        SDKInit.NOADS.Property.Subscribe(_ => { if (_ == 1) ClearADSImage(); });
    }

    public void ClearADSImage()
    {
        noads.gameObject.SetActive(false);
    }

    private bool SetIsBuyBool = false;
    public void SetIsBuy(int index)
    {
        if (IsBuyIndex != index)
            return;
        if (SetIsBuyBool)
            return;
        SetIsBuyBool = true;
        bg2.sprite = IsBuySprite;
        bg2Text.gameObject.SetActive(false);
        PlayerPrefs.SetInt(IsBuyKey + IsBuyIndex.ToString(), 1);
    }


}
