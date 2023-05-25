
/****************************************************
 * FileName:		PropAI.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-08-11-14:27:24
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropAI : MonoBehaviour
{

    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---
    private int curLevel;

    private const int epinephrineLevel = 7;
    private const int setTrapLevel = 9;
    private const int shieldLevel = 12;
    private const int radarLevel = 15;

    private List<AIControl> humanAIs;
    private AIControl zombieAI = null;
    private List<AIControl> surviverAIs;


    #endregion



    void Start()
    {
        curLevel = LevelSetting.Value;
        humanAIs = FindObjectsOfType<AIControl>().ToList();
        foreach (var item in humanAIs)
        {
            if (item.IsZombie)
            {
                zombieAI = item;
            }
            else
            {
                surviverAIs.Add(item);
            }
        }
    }
    
    void Update()
    {
        if (curLevel >= epinephrineLevel)
        {
            EpinephrineAI();
        }
        if (curLevel >= setTrapLevel)
        {
            SetTrapAI();
        }
        if (curLevel >= shieldLevel)
        {
            ShieldAI();
        }
        if (curLevel >= radarLevel)
        {
            RadarAI();
        }
    }



    private float EpinephrineDelay = 10f;
    private float durTimer;
    private float durTime = 15f;
    /// <summary>
    /// 肾上腺素随机
    /// </summary>
    public void EpinephrineAI()
    {
        if (EpinephrineDelay > 0)
        {
            EpinephrineDelay -= Time.deltaTime;
        }
        else
        {
            if (durTimer > 0)
            {
                durTimer -= Time.deltaTime;

            }
            else
            {
                durTimer = durTime;
                //移除获得“护盾”的AI
                var otherList = humanAIs;
                foreach (var item in otherList)
                {
                    if (item.hasProp && item.propName != PropType.Epinephrine)
                    {
                        otherList.Remove(item);
                    }
                }
                //随机两名AI获得“肾上腺素”
                var temList = RandomEpinephrine(otherList, 2);
                foreach (var item in temList)
                {
                    item.hasProp = true;
                    item.propName = PropType.Epinephrine;
                }
            }
        }
    }
    
    private float shieldDelay = 10f;
    private bool shieldOnce = true;
    /// <summary>
    /// 护盾随机
    /// </summary>
    public void ShieldAI()
    {
        if (shieldDelay > 0)
        {
            shieldDelay -= Time.deltaTime;
        }
        else if (shieldOnce == true)
        {
            shieldOnce = false;
            var tem = RandomShield(surviverAIs);
            tem.hasProp = true;
            tem.propName = PropType.Shield;
        }
    }

    private float RadarDelay = 10f;
    private float waitTime;
    /// <summary>
    /// 雷达使用
    /// </summary>
    private void RadarAI()
    {
        if (RadarDelay > 0)
        {
            RadarDelay -= Time.deltaTime;
        }
        else
        {
            if (zombieAI.targets.Count == 0)
            {
                waitTime += Time.deltaTime;
                if (waitTime >= 2f)
                {
                    PropsManager.Instance.UseProp(zombieAI, PropType.Radar);
                }
            }
            else
            {
                waitTime = 0;
            }
        }


    }

    private float setTrapDelay = 10f;
    private float setTrapdurTimer;
    private float setTrapdurTime = 5f;
    private bool setTrapOnce = true;
    private bool setTrapTwice = true;
    private bool setTrapThired = true;
    /// <summary>
    /// 陷阱使用
    /// </summary>
    private void SetTrapAI()
    {
        if (setTrapDelay > 0)
        {
            setTrapDelay -= Time.deltaTime;
        }
        else
        {
            if (setTrapdurTimer > 0)
            {
                setTrapdurTimer -= Time.deltaTime;
            }
            else
            {
                setTrapdurTimer = setTrapdurTime;
                //10s 判定第一次
                if (setTrapOnce)
                {
                    setTrapOnce = false;
                    int num = Random.Range(0,100);
                    if (num <= 35)
                    {
                        PropsManager.Instance.UseProp(zombieAI, PropType.SetTrap);
                    }
                    return;
                }
                //15s 判定第二次
                if (setTrapTwice)
                {
                    setTrapTwice = false;
                    int num = Random.Range(0, 100);
                    if (num <= 25)
                    {
                        PropsManager.Instance.UseProp(zombieAI, PropType.SetTrap);
                    }
                    return;
                }
                //20s 判定第三次
                if (setTrapThired)
                {
                    setTrapThired = false;
                    int num = Random.Range(0, 100);
                    if (num <= 15)
                    {
                        PropsManager.Instance.UseProp(zombieAI, PropType.SetTrap);
                    }
                    return;
                }
            }
        }

    }


    private List<AIControl> RandomEpinephrine(List<AIControl> source, int num)
    {
        if (num > source.Count)
        {
            return source;
        }

        List<AIControl> temList = source;
        List<AIControl> result = new List<AIControl>();

        for (int i = 0; i < num; i++)
        {
            int index = Random.Range(0, temList.Count);

            result.Add(temList[index]);
            temList.RemoveAt(index);
        }

        return result;
    }

    private AIControl RandomShield(List<AIControl> source)
    {
        if (source == null)
        {
            return null;
        }
        int index = Random.Range(0, source.Count);
        var temList = source;
        var tem = temList[index];
        if (tem.hasProp)
        {
            temList.Remove(tem);
            tem = RandomShield(temList);
        }
        return null;
    }


}
