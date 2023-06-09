
/****************************************************
 * FileName:		NativeShareScript.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-11-16:20:09
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;
using System.Collections;
using System.IO;
public class NativeShareScript : MonoBehaviour
{
    public GameObject CanvasShareObj;
    private bool isProcessing = false;
    private bool isFocus = false;
    public void ShareBtnPress()
    {
        if (!isProcessing)
        {
            CanvasShareObj.SetActive(true);
            StartCoroutine(ShareScreenshot());
        }
    }
    IEnumerator ShareScreenshot()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot("screenshot.png", 2);
        string destination = Path.Combine(Application.persistentDataPath, "screenshot.png");
        yield return new WaitForSeconds(0.3f); //WaitForSecondsRealtime(0.3f);  
        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),uriObject);
            intentObject.Call("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),"Can you beat my score?");
            intentObject.Call("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",intentObject, "Share your new score");
            currentActivity.Call("startActivity", chooser);
            yield return new WaitForSeconds(1f); //WaitForSecondsRealtime(1f);  
        }
        yield return new WaitUntil(() => isFocus);
        CanvasShareObj.SetActive(false);
        isProcessing = false;
    }
    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }
}