
/****************************************************
 * FileName:		insufficientUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-23-14:06:33
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

public class insufficientUI : Singleton<insufficientUI>
{

    #region --- Private Variable ---

    private Image bgImage;
    private Transform Root;
    private Button closeBtn;
    private Button buyBtn;


    #endregion

    private void Awake()
    {
        TryGetComponent(out bgImage);
        Root = transform.GetChild(0);
        Root.Find("closeBtn").TryGetComponent(out closeBtn);
        Root.Find("buyBtn").TryGetComponent(out buyBtn);

        closeBtn.onClick.AddListener(() => Hide());
        buyBtn.onClick.AddListener( ()=> {
            SDKInit.Instance.BuyProduct(ShopProductNames.gems1);
            UIAnim(buyBtn.transform);
        } );

        Hide();
    }

    private void UIAnim(Transform trans)
    {
        trans.DOKill();
        trans.localScale = Vector3.one;
        trans.DOPunchScale(new Vector3(0.2f, 0.2f, 1), 0.4f, 12, 0.5f);
    }

    public void Show()
    {
        bgImage.enabled = true;
        Root.gameObject.SetActive(true);
        Root.DOKill();
        Root.localScale = Vector3.zero;
        Root.DOScale(1, .3f);
    }


    private void Hide()
    {
        Root.gameObject.SetActive(false);
        bgImage.enabled = false;
    }

}
