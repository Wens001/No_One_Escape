
/****************************************************
 * FileName:		CameraFllow.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-01-17:54:32
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
public class CameraFllow : Singleton<CameraFllow>
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---

    public Transform target { get; private set; }
    public bool UseRotation = false;

    public bool IsAtTarget { get {
            if (target == null)
                return false;
            return Vector3.Distance(transform.position , target.position + offset) < 1f;
        } }

    public Vector3 offset;
    private float speed = 5;
    public static Vector3 baseOffset { get; private set; }

    #endregion
    private void Awake()
    {
        baseOffset = offset;
    }

    void Start()
    {
        SetPlayer();
    }

    #region Debug_Test

    private bool UseThirdPerson = false;
    private bool UseTopDown = false;

    private System.IDisposable disposable1;
    private System.IDisposable disposable2;
    private void OnEnable()
    {
        disposable1 = SceneSettingUI.View.Subscribe( 
            value=> {
                UseThirdPerson = value;
                transform.SetParent(value ? PlayerControl.Instance.transform : null);
                if (value)
                {
                    transform.localPosition = new Vector3(0, 2.5f, -2.5f);
                    transform.localRotation = Quaternion.Euler(30, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(50, 270, 0);
                }
            } );
        disposable2 = SceneSettingUI.TopDown.Subscribe(
          value => {
              UseTopDown = value;
              transform.SetParent(null);
              if (value)
              {
                  offset = new Vector3(3, 8);
                  transform.rotation = Quaternion.Euler(65, 270, 0);
              }
              else
              {
                  offset = new Vector3(4, 7);
                  transform.rotation = Quaternion.Euler(50, 270, 0);
              }
          }
            );
    }
    private void OnDisable()
    {
        if (disposable1 != null)
            disposable1.Dispose();
        if (disposable2 != null)
            disposable2.Dispose();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (UseThirdPerson)
        {

            return;
        }

        if (UseTopDown)
        {
            offset = new Vector3(3, 8);
            transform.position = Vector3.Lerp(transform.position, offset + target.position, speed * Time.deltaTime);
            return;
        }

        if (target)
        {
            if (!UseRotation)
            {
                transform.position = 
                    Vector3.Lerp(transform.position, offset + target.position, speed * Time.deltaTime);
            }
            else
            {
                transform.position =
                    Vector3.Lerp(transform.position, target.TransformPoint(offset), speed * Time.deltaTime);
            }
        }

        if (UseRotation && target)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(target.position - transform.position + Vector3.up),8 * Time.deltaTime);
        }
    }


    public void SetOtherShow(Transform _target , Vector3 _offset )
    {
        target = _target; 
        speed = 3;
        offset = _offset;
    }

    public void LookAtTarget(Transform door, Vector3 _offset)
    {
        target = door;
        speed = 3;
        UseRotation = true;
        offset = _offset;
    }

    public void SetPlayer()
    {
        target = PlayerControl.Instance.transform;
        speed = 5;
        if (PlayerControl.Instance.IsHuman || ZombieShowTimer.Instance.IsNotZomble)
            offset = baseOffset;
        else
            offset = new Vector3(5, 8, 0);
    }

}
