
/****************************************************
 * FileName:		CountdownUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-23-17:35:17
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CountdownUI : MonoBehaviour
{

    public string _Str;

    private CanvasGroup canvasGroup;
    private GameObject adsGo;
    private Button button;
    private DateTime nextDateTime;
    private Text money;
    private ReactiveProperty<int> needSeconds;

    private Color _baseColor;
    private Vector3 _lastPos;
    private void Awake()
    {
        needSeconds = new ReactiveProperty<int>();
        transform.Find("bgs").TryGetComponent(out canvasGroup);
        TryGetComponent(out button);
        transform.Find("money").TryGetComponent(out money);
        _baseColor = money.color;
        _lastPos = money.rectTransform.anchoredPosition3D;
        adsGo = transform.Find("bgs/ads").gameObject;

        if (PlayerPrefs.HasKey(_Str))
            nextDateTime = PlayerPrefsTime.GetDateTime(_Str);

        needSeconds.Subscribe(UpdateText);
    }

    private void Update()
    {
        if (nextDateTime == null)
        {
            button.interactable = true;
            needSeconds.Value = 0;
        }
        else
        {
            if (nextDateTime < DateTime.Now)
            {
                button.interactable = true;
                needSeconds.Value = 0;
            }
            else
            {
                button.interactable = false;
                needSeconds.Value = (int)(nextDateTime - DateTime.Now).TotalSeconds;
            }
        }
    }


    private void UpdateText(int a)
    {
        if (a <= 0)
        {
            canvasGroup.alpha = 1;
            money.text = "FREE";
            money.rectTransform.anchoredPosition3D = _lastPos;
            money.color = _baseColor;
            adsGo.SetActive(true);
        }
        else
        {
            canvasGroup.alpha = .5f;
            money.text = (a / 60).ToString("00") + ":" + (a % 60).ToString("00");
            var t = _lastPos;
            t.x = 0;
            money.rectTransform.anchoredPosition3D = t;
            money.color = Color.white;
            adsGo.SetActive(false);
        }
    }

    public float AddMinutes = 30f;
    public void OnClick()
    {
        nextDateTime = DateTime.Now.AddMinutes(AddMinutes);
        PlayerPrefsTime.SetDateTime(_Str, nextDateTime);
    }

}
