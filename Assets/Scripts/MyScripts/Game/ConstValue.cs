
/****************************************************
 * FileName:		ConstValue.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-12-10:07:24
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstValue 
{
    public class LayerName
    {
        public const string Default = "Default";
        public const string Ground = "Ground";
        public const string Wall = "Wall";
        public const string Window = "Window";
    }

    public class TagName
    {
        public const string Exit = "Exit";
        public const string Player = "Player";
    }

    public class AnimatorStr
    {
        public const string velocity = "velocity";
        public const string JumpForWall = "JumpForWall";
        public const string Morph = "Morph";
        public const string State = "State";
        public const string Dead = "Dead";
        public const string DeadIndex = "DeadIndex";
        public const string Attack = "Attack";
        public const string AttackIndex = "AttackIndex";
        public const string Help = "Help";
        public const string IsClean = "IsClean";
        public const string Dance = "Dance";
        public const string DanceIndex = "DanceIndex";
        public const string Mirror = "Mirror";
    }

    public class CallBackFun
    {
        public const string KillerOtherAudio = "KillerOtherAudio";    //杀手其他音效
        public const string OpenNewCharacterUI = "OpenNewCharacterUI";//打开新界面
        public const string Damage = "Damage";    //杀手出现
        public const string KillerShow = "KillerShow";    //杀手出现
        public const string HumanBeDiscovered = "HumanBeDiscovered";      //有人类被发现
        public const string ZombieLossVision = "ZombieLossVision";      //丧尸丢失视野
        public const string ZombieShow = "ZombieShow";      //丧尸出现
        public const string ButtonDown = "ButtonDown";
        public const string PlayerDead = "PlayerDead";
        public const string CharcterUpgrade = "CharcterUpgrade"; //角色等级提升

        public const string WillKillPlayer = "WillKillPlayer";  //将要杀幸存者
        public const string PlayerRebone = "PlayerRebone";
        public const string PlayerGoOut = "PlayerGoOut";    //有玩家走出大门
        public const string GameOver = "GameOver";    //游戏结束
        public const string PlayEffect = "PlayEffect";    //播放特效

        public const string AddMoney = "AddMoney";    //增加金币
        public const string AddCoin = "AddCoin";    //增加钻石
        public const string FBSendAllHumansData = "FBSendAllHumansData";    //fb发送所有人物信息

        public const string BuyCoin = "BuyCoin";    //购买钻石
        public const string NotADS = "NotADS";    //内购

        public const string PropNumAdd = "PropNumAdd";        //道具数目增加

        public const string FirstLanding = "FirstLanding";    //今日首次登陆
    }

    public class AIAnimStr
    {
        public const string Chase = "Chase";
        public const string Escape = "Escape";
        public const string Touch = "Touch"; 
        public const string Open = "Open";
    }

    public class SaveDataStr
    {
        public const string Money = "Money";
        public const string Coin = "Coin";
        public const string KillerIndex = "KillerIndex";
        public const string HumanIndex = "HumanIndex";
        public const string TryGetCharacter = "TryGetCharacter";
    }

    public const float GameTime = 45f;

    public const string _Color = "_BaseColor";

    public const float DeadSpeedMul = .4f;
    public const float DelayDeadTime = .2f;
}
