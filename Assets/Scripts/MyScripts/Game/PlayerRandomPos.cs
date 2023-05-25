
/****************************************************
 * FileName:		PlayerRandomPos.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-05-16:43:45
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRandomPos : MonoBehaviour
{

    #region --- Public Variable ---

    public Transform ais;

    #endregion


    #region --- Private Variable ---

    private List<Vector3> posList;

    private List<int> indexList;
    #endregion

    private void Start()
    {
        posList = new List<Vector3>();
        indexList = new List<int>();
        foreach (Transform child in transform)
            posList.Add(child.position);
        indexList.Add(Random.Range(0, posList.Count));
        PlayerControl.Instance.transform.position = posList[indexList[indexList.Count -1]] ;

        var others = ais.GetComponentsInChildren<NavMeshAgent>();
        foreach (var agent in others)
        {
            var value = -1;
            for (; ; )
            {
                value = Random.Range(0, posList.Count);
                if (indexList.Contains(value))
                    continue;
                indexList.Add(value);
                agent.transform.position = posList[value];
                break;
            }
        }
        Destroy(gameObject);
    }

    


}
