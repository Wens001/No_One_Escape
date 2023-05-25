
/****************************************************
 * FileName:		ModelObjects.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-06-09-17:34:05
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "GameDataAsset", menuName = "Creat ModelObjects Asset")]
public class ModelObjects : SerializedScriptableObject
{
    [System.Serializable]
    public class ModelMessage
    {
        [LabelText("模型")] public GameObject model;
        [LabelText("攻击技能")] public string attackMethod;
    }

    [LabelText("基础动物")] public List<ModelMessage> Animal = new List<ModelMessage>();

    [LabelText("幸存者")] public List<ModelMessage> general = new List<ModelMessage>();

    [LabelText("杀手")] public List<ModelMessage> killerModel = new List<ModelMessage>();


    public GameObject ShowModel(int team,int index, Transform root,HumanBase player)
    {
        var message = GetTeam(team);
        index = Mathf.Clamp(index, 0, message.Count - 1);

        if(team == 0 && index == 0 && !player.IsMe)
        {
            index = Random.Range(0, Animal.Count);
            var goo = InstantiateModel(Animal[index].model, root, Animal[index].model.transform.localScale);
            player.ChangeAttackJudge(Animal[index].attackMethod);
            return goo;
        }

        var go = InstantiateModel(message[index].model,root , message[index].model.transform.localScale) ;
        player.ChangeAttackJudge(message[index].attackMethod);
        return go;
    }

    private GameObject InstantiateModel(GameObject model,Transform root,Vector3 scale)
    {
        var go = Instantiate(model);
        go.transform.SetParent(root);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;
        return go;
    }

    public GameObject ShowRandomModel(int team, Transform root,HumanBase player,out int index)
    {
        var message = GetTeam(team);
        index = Random.Range(0, message.Count);

        if(team == 0 && index == 0)
        {
            index = Random.Range(0, Animal.Count);
            var goo = InstantiateModel(Animal[index].model, root, Animal[index].model.transform.localScale);
            player.ChangeAttackJudge(Animal[index].attackMethod);
            return goo;
        }
        var go = InstantiateModel(message[index].model,root , message[index].model.transform.localScale) ;
        player.ChangeAttackJudge(message[index].attackMethod);
        return go;
    }

    private List<ModelMessage> GetTeam(int team)
    {
        switch (team)
        {
            case 0:
                return general;
            case 1:
                return killerModel;
        }
        return general;
    }

}
