
/****************************************************
 * FileName:		MoreButtonUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-08-15:06:37
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
using DG.Tweening;
public class MoreButtonUI : MonoBehaviour
{

    #region --- Public Variable ---

    public Transform[] humans;
    public Transform[] killers;
    

    #endregion


    #region --- Private Variable ---

    private Button[] buttons;
    private UpgradeTeamUI teamUI;

    #endregion

    private void Awake()
    {
        teamUI = UpgradeTeamUI.Instance;
        teamUI.InitUI();
        teamUI.leftTeam.Subscribe(ChangeData);

        const int size = 4;
        buttons = new Button[size];
        for (int i = 0; i < size; i++)
        {
            transform.Find("h" + i).TryGetComponent(out buttons[i]);
            var t = i;
            buttons[i].OnClickAsObservable().Subscribe(_=>ClickPicture(buttons[t],t));
        }

        for (int i = 0; i < killers.Length; i++)
        {
            killers[i].parent = buttons[i].transform;
            killers[i].localPosition = Vector3.zero + Vector3.up * 14 * (i!=1 ? 1 : 0) ;
        }
        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].parent = buttons[i].transform;
            humans[i].localPosition = Vector3.zero + Vector3.up * 14;
        }

    }

    private void ClickPicture(Button btn,int index)
    {
        if (teamUI.leftTeam.Value)
        {
            if (index < killers.Length)
                Model_Upgrade.Instance.killerIndex.Value = index;
        }
        else
        {
            if (index < humans.Length)
                Model_Upgrade.Instance.humanIndex.Value = index;
        }
    }

    private void ChangeData(bool isLeft)
    {
        for (int i = 0; i < killers.Length; i++)
            killers[i].gameObject.SetActive(isLeft);
        for (int i = 0; i < humans.Length; i++)
            humans[i].gameObject.SetActive(!isLeft);



    }


}
