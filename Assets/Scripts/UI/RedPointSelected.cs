
/****************************************************
 * FileName:		RedPointSelected.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-03-14:49:36
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class RedPointSelected : MonoBehaviour
{

    #region --- Public Variable ---

    public GameObject redP;

    public bool isPrefs = false;
    public string PrefsStr = "";

    public bool isUpgrade = false;

    #endregion


    #region --- Private Variable ---

    private Button button;
    private int prefsInt = 0;

    #endregion

    private void Awake()
    {
        if (!redP)
            return;
        TryGetComponent(out button);
        if (isPrefs)
        {
            prefsInt = PlayerPrefs.GetInt(PrefsStr, 0);
            redP.SetActive(LevelSetting.Value >= 4 && prefsInt == 0);
            button.onClick.AddListener(() => { redP.SetActive(false); PlayerPrefs.SetInt(PrefsStr, 1); });
        }
        else if (isUpgrade)
        {
            SceneMainUI.Instance.index.Subscribe(v => {
                redP.SetActive(v != 3 && GameSetting.Money.Value >= 200 && LevelSetting.Value >= 4 ); } );
            button.onClick.AddListener(() =>redP.SetActive(false));
        }
        else
        {
            redP.SetActive(LevelSetting.Value >= 4);
            button.onClick.AddListener(() => { redP.SetActive(false); });
        }
        
    }


}
