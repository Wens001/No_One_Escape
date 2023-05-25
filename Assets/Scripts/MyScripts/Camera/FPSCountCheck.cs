
/****************************************************
 * FileName:		FPSCountCheck.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-07-14:03:48
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using UnityEngine;

public class FPSCountCheck : MonoBehaviour
{

    private Light _light;

    public float FpsValue;
    private float time;
    private int frameCount;

    private MyTimer myTimer = new MyTimer(2f);

    private void Awake()
    {
        TryGetComponent(out _light);
    }

    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            FpsValue = frameCount / time;
            time = 0;
            frameCount = 0;
        }

        if (FpsValue <= 25)
        {
            myTimer.OnUpdate(Time.deltaTime);
            if (myTimer.IsFinish)
                _light.shadows = LightShadows.None;
        }
        else
            myTimer.ReStart();
    }
}
