using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class GDPRView : MonoBehaviour
{
    public static GDPRView Instance { get; private set; }

    public GameObject View1;
	public GameObject View2;
	public GameObject View3;
	public GameObject View4;

	public GameObject BtnSdkObj;
	public GameObject BtnSdkUnObj;
	
	public GameObject BtnAdsObj;
	public GameObject BtnAdsUnObj;

	public TMP_TextEventHandler TextEventHandler;
	
	public IntProperty Status { get; private set; }     //	00：没有选择过；	01：同意sdk，不同意ads； 10：不同意sdk，同意ads；  11：都同意

    private void Awake()
    {
        Instance = this;
        Status = new IntProperty("GDPRStatus", -1);
        Init();

        Status.Property.Subscribe(a =>
        {
            BtnSdkObj.SetActive(a == -1 || (a & 1) != 0);
            BtnSdkUnObj.SetActive(!BtnSdkObj.activeSelf);
            BtnAdsObj.SetActive(a == -1 ||(a & 2) != 0);
            BtnAdsUnObj.SetActive(!BtnAdsObj.activeSelf);
        }
);
    }

    public void Init()
	{
        Content = transform.Find("Content").gameObject;
        transform.Find("GDPRBtn").TryGetComponent(out GDPRBtn);
        GDPRBtn.onClick.AddListener(() => { Show(3);  });
        GDPRBtn.gameObject.SetActive(false);
		TextEventHandler.onLinkSelection.AddListener(OnClickLink);
        Hide();
        SDKInit.isGDPR.Subscribe( a =>
        {
            GDPRBtn.gameObject.SetActive( a && Status.Value != 3);
            if ( a && Status.Value != 3)
            {
                this.AttachTimer(0.05f, () =>Show(1));
            }
        }  );
    }

	public void OnClickSdk(bool isCan)
	{
        if (Status.Value == -1)
            Status.Value = 2;
        if (isCan)
            Status.Value |= 1;
        else
            Status.Value &= (~1);
        BtnSdkObj.SetActive(isCan);
		BtnSdkUnObj.SetActive(!isCan);
	}
	
	public void OnClickAds(bool isCan)
	{
        if (Status.Value == -1)
            Status.Value = 1;
        if (isCan)
            Status.Value |= 2;
        else
            Status.Value &= (~2);
        BtnAdsObj.SetActive(isCan);
		BtnAdsUnObj.SetActive(!isCan);
	}

	public void OnClickAgree3()
	{
        if (Status.Value == -1 || Status.Value == 3)
            OnClickAgree();
        else
            OnClickNext(4);
	}

	public void OnClickAgree()
	{
        Status.Value = 3;
        MaxSdk.SetHasUserConsent(true);
		Hide();
    }

	public void OnClickNext(int step)
	{

        View1.SetActive(step == 1);
        View2.SetActive(step == 2);
        View3.SetActive(step == 3);
        View4.SetActive(step == 4);
	}

	public void OnClickLink(string idStr, string url, int id)
	{
		Application.OpenURL(url);
	}

    private GameObject Content;
    private Button GDPRBtn;


    public void Show( int index = 1, bool isHandleAds = true)
	{
        Time.timeScale = 0;
        if (AppLovinCrossPromo.Instance() == null)
            AppLovinCrossPromo.Init();
        AppLovinCrossPromo.Instance().HideMRec();
        GDPRBtn.gameObject.SetActive(false);
        Content.SetActive(true);

		OnClickNext(index);
	}

	public void Hide()
	{
        Time.timeScale = 1;
        GDPRBtn.gameObject.SetActive((Status.Value != 3) && SDKInit.isGDPR.Value);
        Content.SetActive(false);
    }
}
