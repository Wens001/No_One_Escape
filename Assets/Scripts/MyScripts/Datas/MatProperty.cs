
/****************************************************
 * FileName:		MatProperty.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-09-17:38:33
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "GameDataAsset", menuName = "Creat MatProperty Asset")]
public class MatProperty : ScriptableObject
{
    [System.Serializable]
    public class Datas
    {
        [ColorUsage(true,true)] public Color _GroundColor;
        [ColorUsage(true, true)] public Color _DownColor;
        [ColorUsage(true, true)] public Color _TopColor;
        public float _TopY;
        [PropertyRange(0,1)]public float Metallic;
        [PropertyRange(0, 1)] public float Smmthness;
    }

    public List<Datas> matList = new List<Datas>();

    public int index;
    [PreviewField] public Material wallMat;
    [PreviewField] public Material groundMat;

    [Button("添加属性")]
    public void AddMatProperty()
    {
        var mp = new Datas
        {
            _DownColor = wallMat.GetColor("_DownColor"),
            _TopColor = wallMat.GetColor("_TopColor"),
            _TopY = wallMat.GetFloat("_TopY"),
            Metallic = wallMat.GetFloat("Metallic"),
            Smmthness = wallMat.GetFloat("Smmthness"),
            _GroundColor = groundMat.GetColor("_BaseColor"),
        };
        matList.Add(mp);
    }

    [Button("更改当前属性")]
    public void UpdateMatProperty()
    {
        index = Mathf.Clamp(index, 0, matList.Count - 1);
        var mp = new Datas
        {
            _DownColor = wallMat.GetColor("_DownColor"),
            _TopColor = wallMat.GetColor("_TopColor"),
            _TopY = wallMat.GetFloat("_TopY"),
            Metallic = wallMat.GetFloat("Metallic"),
            Smmthness = wallMat.GetFloat("Smmthness"),
            _GroundColor = groundMat.GetColor("_BaseColor"),
        };
        matList[index] = mp ;
    }

    [Button("使用当前属性")]
    public void UseMatProperty()
    {
        index = Mathf.Clamp(index, 0, matList.Count - 1);
        wallMat.SetColor("_DownColor", matList[index]._DownColor);
        wallMat.SetColor("_TopColor", matList[index]._TopColor);
        wallMat.SetFloat("_TopY", matList[index]._TopY);
        wallMat.SetFloat("Metallic", matList[index].Metallic);
        wallMat.SetFloat("Smmthness", matList[index].Smmthness);
        groundMat.SetColor("_BaseColor", matList[index]._GroundColor);
    }

    public void UseMatProperty(int index)
    {
        index = index % matList.Count;
        wallMat.SetColor("_DownColor", matList[index]._DownColor);
        wallMat.SetColor("_TopColor", matList[index]._TopColor);
        wallMat.SetFloat("_TopY", matList[index]._TopY);
        wallMat.SetFloat("Metallic", matList[index].Metallic);
        wallMat.SetFloat("Smmthness", matList[index].Smmthness);
        groundMat.SetColor("_BaseColor", matList[index]._GroundColor);
    }
}