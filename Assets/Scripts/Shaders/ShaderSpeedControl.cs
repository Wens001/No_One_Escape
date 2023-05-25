
/****************************************************
 * FileName:		ShaderSpeedControl.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-15-14:17:44
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSpeedControl : MonoBehaviour
{

    #region --- Private Variable ---

    private List<Material> allMats;

    private Vector3 lastPos;
    private float range = 0;
    private Vector3 dir;
    #endregion

    void Awake()
    {
        allMats = new List<Material>();
        var rends = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var rd in rends)
            foreach (var sm in rd.materials)
                allMats.Add(sm);
        lastPos = transform.position;
        dir = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(lastPos,transform.position) != 0)
        {
            var tdir = transform.position - lastPos;
            range += tdir.magnitude;
            dir = Vector3.Lerp(dir, tdir.normalized , Time.deltaTime * 15);
            
        }
        else
            range -= Time.deltaTime * 3;
        range = Mathf.Clamp(range, 0, .3f);
        WriteDataToMaterials();
        lastPos = transform.position;
    }

    private void WriteDataToMaterials()
    {
        foreach (var mat in allMats)
        {
            mat.SetFloat("_Range", range);
            mat.SetVector("forwaddDir", dir);
        }
    }

}
