
/****************************************************
 * FileName:		ModelParameter.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-09-14:52:01
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
using Sirenix.OdinInspector;

public class ModelParameter : MonoBehaviour
{

    #region --- Public Variable ---

    public Animator anim { get; private set; }
    public CasqueControl casqueControl;
    public CasqueControl weaponControl;
    public List<Material> mat { get; private set; } = new List<Material>();

    #endregion


    #region --- Private Variable ---



    #endregion


    private void Awake()
    {
        anim = GetComponent<Animator>();
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rd in renderers)
            foreach (var mr in rd.materials)
                mat.Add(mr);
    }

    void Start()
    {

    }

    #region OtherColor
    private static int index;
    private static Color []allcolors = new Color[]{ Color.red ,Color.green , Color.yellow , Color.blue ,Color.white, Color.cyan };
    private System.IDisposable disposable;
    private void OnEnable()
    {
        disposable = SceneSettingUI.OtherColor.Subscribe( value=>
        {
            var alpha = mat[0].GetColor("_BaseColor").a;
            foreach (var ma in mat)
            {
                var color = value ? allcolors[index % allcolors.Length] : Color.white;
                color.a = alpha;
                ma.SetColor("_BaseColor", color );
            }
            index++;
        }
        );
    }

    private void OnDisable()
    {
        if (disposable != null)
            disposable.Dispose();
    }

    #endregion

}
