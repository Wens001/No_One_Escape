
/****************************************************
 * FileName:		JoyStickTest.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-11-10:54:14
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
public class JoyStickTest : Singleton<JoyStickTest>
{
    #region --- Private Variable ---

    private Image Btn;
    private static float widthScale7;

    private static Image BG;
    private static Image circle;
    private static float widthScale;
    private static bool isinit = false;
    #endregion


    void Start()
    {

        transform.Find("Btn").TryGetComponent(out Btn);
        widthScale7 = Screen.width / 7;
        Btn.rectTransform.sizeDelta = new Vector2(widthScale7, widthScale7);
        Btn.gameObject.SetActive(false);
        _camera = Camera.main;

        transform.Find("BG").TryGetComponent(out BG);
        BG.transform.Find("circle").TryGetComponent(out circle);
        widthScale = Screen.width / 5;
        BG.rectTransform.sizeDelta = new Vector2(widthScale, widthScale);
        circle.rectTransform.sizeDelta = new Vector2(widthScale/2, widthScale/2);
        Hide();

        SceneSettingUI.JoyStick.Subscribe( a=> 
            {
                BG.enabled = a;
                circle.enabled = a;
            }  
        );
        isinit = true;

    }

    #region 按钮提示

    private void OnEnable()
    {
        Messenger.AddListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
        buttonTimer.SetFinish();
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase, ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
    }

    private void ListenerDoor(HumanBase human, ButtonProp bp)
    {
        if (PlayerControl.Instance.IsZombie)
            UpdateButtonToolTip(bp.transform);
    }

    private Camera _camera;
    private Transform buttonTrans;
    private MyTimer buttonTimer = new MyTimer(3f);
    private void UpdateButtonToolTip(Transform target)
    {
        buttonTrans = target;
        buttonTimer.ReStart();
    }

    private void Update()
    {
        Btn.gameObject.SetActive(!buttonTimer.IsFinish);
        if (buttonTimer.IsFinish || buttonTrans == null )
            return;
        buttonTimer.OnUpdate(Time.deltaTime);
        var spos = _camera.WorldToScreenPoint(buttonTrans.position);
        spos.x = Mathf.Clamp(spos.x, 0, Screen.width - widthScale7);
        spos.y = Mathf.Clamp(spos.y, 0, Screen.height - widthScale7);
        spos.z = 0;

        if (spos.x > Screen.width / 2)
        {
            var scale = Btn.rectTransform.localScale;
            scale.x = -1;
            Btn.rectTransform.localScale = scale;
            spos.x += widthScale7;
        }
        Btn.rectTransform.anchoredPosition = spos;
    }

    #endregion

    #region 遥感控制

    public static void Show()
    {
        if (!isinit)
            return;
        BG.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (!isinit)
            return;
        BG.gameObject.SetActive(false);
    }

    public static void SetPosition(Vector3 mousePos)
    {

        var bgPos = mousePos;

        bgPos.x -= widthScale / 2;
        bgPos.y -= widthScale / 2;
        BG.rectTransform.anchoredPosition = bgPos;

        var tlength = (Input.mousePosition - mousePos).magnitude;
        tlength = Mathf.Clamp(tlength, 0, widthScale / 4);
        var tpos = (Input.mousePosition - mousePos).normalized * tlength;
        tpos.x -= circle.rectTransform.sizeDelta.x / 2;
        tpos.y -= circle.rectTransform.sizeDelta.y / 2;

        circle.rectTransform.localPosition = tpos +
            new Vector3(BG.rectTransform.sizeDelta.x / 2, BG.rectTransform.sizeDelta.y / 2, 0);
    }

    #endregion


}
