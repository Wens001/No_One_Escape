
/****************************************************
 * FileName:		ConnectUsUI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-08-13:14:48
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConnectUsUI : MonoBehaviour
{
    #region --- Private Variable ---

    private Text text;

    private Transform group;

    private Button facebookBtn;
    private Button instagramBtn;
    private Button twitterBtn;
    private Button websiteBtn;
    private Button youtubeBtn;

    #endregion

    private void Awake()
    {
        transform.Find("Text").TryGetComponent(out text);

        switch (Application.systemLanguage)
        {
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.Chinese:
                text.text = "-----------请联系我们！-----------";
                break;
            case SystemLanguage.ChineseTraditional:
                text.text = "-----------與我們連結！-----------";
                break;
            case SystemLanguage.Japanese:
                text.text = "------私達と繋がりましょう！------";
                break;
            case SystemLanguage.Korean:
                text.text = "--------우리와 연결하세요!--------";
                break;
            case SystemLanguage.Spanish:    //西班牙语
                text.text = "-----¡Síguenos en las redes!-----";
                break;
            case SystemLanguage.German:    //德语
                text.text = "-Setze dich mit uns in Verbindung!-";
                break;
            case SystemLanguage.Russian:    //俄语
                text.text = "--Свяжитесь с нами!--";
                break;
            default:
                text.text = "---------Connect with us!---------";
                break;
        }




        group = transform.Find("groups");
        group.Find("facebook").TryGetComponent(out facebookBtn);
        facebookBtn.onClick.AddListener(() => { Application.OpenURL("https://www.facebook.com/lionstudios.cc/"); }); 

        group.Find("instagram").TryGetComponent(out instagramBtn);
        instagramBtn.onClick.AddListener(() => { Application.OpenURL("https://www.instagram.com/lionstudioscc/"); });

        group.Find("twitter").TryGetComponent(out twitterBtn);
        twitterBtn.onClick.AddListener(() => { Application.OpenURL("https://twitter.com/LionStudiosCC"); });

        group.Find("website").TryGetComponent(out websiteBtn);
        websiteBtn.onClick.AddListener(() => { Application.OpenURL("https://lionstudios.cc"); });

        group.Find("youtube").TryGetComponent(out youtubeBtn);
        youtubeBtn.onClick.AddListener(() => { Application.OpenURL("https://www.youtube.com/lionstudioscc"); });



    }

}
