using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UniRx;
public class StarsPopup : BasePanel
{

    public Text mainText;
    public Text sendButton;
    public Text notNowButton;
    public Text neverButton;
    public Transform starsHolder;
    public Button send;

    private bool openUrl;


    private Image image;
    private Transform child;


    private void Awake()
    {
        TryGetComponent(out image);
        child = transform.GetChild(0);
        gameObject.SetActive(false);

        LevelSetting.Level.Property.Subscribe(
        level =>
        {
            if (level == 5 && PlayerPrefs.GetInt("hasPopup", 0) == 0 )
            {
                PlayerPrefs.SetInt("hasPopup", 1);
                OnEnter();
            }
        }
        );
    }

    /// <summary>
    /// Set popup texts from Settings Window
    /// </summary>
    private void Start()
    {
        send.interactable = false;
        for (int i = 0; i < starsHolder.childCount; i++)
        {
            starsHolder.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Button event called from Send Button 
    /// </summary>
    public void SendButtonClick()
    {
        ClosePopup();
        RateGame.Instance.NeverShowPopup();
        if (openUrl)
        {
            RateGame.Instance.OpenUrl();
        }
    }


    /// <summary>
    /// Button event called from Later button
    /// </summary>
    public void NotNowButton()
    {
        ClosePopup();
    }


    /// <summary>
    /// Button event called from never button
    /// </summary>
    public void NeverButton()
    {
        ClosePopup();
        RateGame.Instance.NeverShowPopup();
    }


    /// <summary>
    /// Called when a star is clicked, activates the required stars
    /// </summary>
    /// <param name="star"></param>
    public void StarClicked(GameObject star)
    {
        int starNUmber = int.Parse(star.name.Split('_')[1]);
        if (starNUmber + 1 < RateGame.Instance.RateGameSettings.minStarsToSend)
        {
            openUrl = false;
        }
        else
        {
            openUrl = true;
        }
        for (int i = 0; i < starsHolder.childCount; i++)
        {
            if (i <= starNUmber)
            {
                starsHolder.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                starsHolder.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        send.interactable = true;
    }


    /// <summary>
    /// Make close animation then destroy the popup
    /// </summary>
    private void ClosePopup()
    {
        Destroy(gameObject);
    }


    /// <summary>
    /// Trigger close popup event
    /// </summary>
    private void CloseEvent()
    {
        RateGame.Instance.RatePopupWasClosed();
    }

    public override void OnEnter()
    {
#if UNITY_ANDROID
        gameObject.SetActive(true);
        image.enabled = true;
        Start();
        child.gameObject.SetActive(true);
        child.DOKill();
        child.localScale = Vector3.zero;
        child.DOScale(Vector3.one, .3f);
#endif
#if UNITY_IOS
        Device.RequestStoreReview();
        gameObject.SetActive(false);
#endif
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

    public override void OnExit()
    {

    }
}
