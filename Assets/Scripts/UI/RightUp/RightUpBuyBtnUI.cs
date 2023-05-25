
/****************************************************
 * FileName:		RightUpBuyBtnUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-24-10:43:44
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
using UniRx;

public class RightUpBuyBtnUI : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    private Button btn;

    #endregion



    void Start()
    {
        TryGetComponent(out btn);
        btn.OnClickAsObservable().Subscribe(
            _ =>{
                SceneMainUI.Instance.index.Value = 0;
            }
        );

        if (EnterScene.IsChinese.Value)
        {
            gameObject.SetActive(false);
            return;
        }

        SceneMainUI.Instance.index.Subscribe( a=>
        {
            if (a <= -1 || LevelSetting.Level.Value <= 3)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        });


    }

    


}
