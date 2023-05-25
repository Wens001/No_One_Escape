
/****************************************************
 * FileName:		OtherAudioSourcePlay.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-07-10:43:09
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherAudioSourcePlay : MonoBehaviour
{

    private AudioSource aSource;

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---



    #endregion

    private void Start()
    {
        TryGetComponent(out aSource);
    }

    private void OnEnable()
    {
        Messenger.AddListener<float>(ConstValue.CallBackFun.KillerOtherAudio, PlayAudio);
        Messenger.AddListener(ConstValue.CallBackFun.GameOver, GameOver);
        Messenger.AddListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, PlayerDeadListener);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<float>(ConstValue.CallBackFun.KillerOtherAudio, PlayAudio);
        Messenger.AddListener(ConstValue.CallBackFun.GameOver, GameOver);
        Messenger.RemoveListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, PlayerDeadListener);
    }

    private void GameOver()
    {
        PlayAudio(0);
    }

    private void PlayerDeadListener(HumanBase killer, HumanBase target)
    {
        if (killer.modelsGroup.index == 1)
        {
            PlayAudio(0);
            this.AttachTimer(1.1f,
                () => {
                    if (!GameManager.isWin && !GameManager.isDead)
                        PlayAudio(PlayerPrefs.GetFloat("SoundVolume", 1));
                }
            );
        }
    }

    public void PlayAudio(float value)
    {
        if (!aSource)
            return;
        aSource.volume = Mathf.Clamp01(value);
        if (value > .1f)
            aSource.Play();
        else if (value < .1f)
            aSource.Stop();
        aSource.time = 0;
    }



    
}
