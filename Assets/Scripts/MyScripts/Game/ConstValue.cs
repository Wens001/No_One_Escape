
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
        public const string KillerOtherAudio = "KillerOtherAudio";    //ɱ��������Ч
        public const string OpenNewCharacterUI = "OpenNewCharacterUI";//���½���
        public const string Damage = "Damage";    //ɱ�ֳ���
        public const string KillerShow = "KillerShow";    //ɱ�ֳ���
        public const string HumanBeDiscovered = "HumanBeDiscovered";      //�����౻����
        public const string ZombieLossVision = "ZombieLossVision";      //ɥʬ��ʧ��Ұ
        public const string ZombieShow = "ZombieShow";      //ɥʬ����
        public const string ButtonDown = "ButtonDown";
        public const string PlayerDead = "PlayerDead";
        public const string CharcterUpgrade = "CharcterUpgrade"; //��ɫ�ȼ�����

        public const string WillKillPlayer = "WillKillPlayer";  //��Ҫɱ�Ҵ���
        public const string PlayerRebone = "PlayerRebone";
        public const string PlayerGoOut = "PlayerGoOut";    //������߳�����
        public const string GameOver = "GameOver";    //��Ϸ����
        public const string PlayEffect = "PlayEffect";    //������Ч

        public const string AddMoney = "AddMoney";    //���ӽ��
        public const string AddCoin = "AddCoin";    //������ʯ
        public const string FBSendAllHumansData = "FBSendAllHumansData";    //fb��������������Ϣ

        public const string BuyCoin = "BuyCoin";    //������ʯ
        public const string NotADS = "NotADS";    //�ڹ�

        public const string PropNumAdd = "PropNumAdd";        //������Ŀ����

        public const string FirstLanding = "FirstLanding";    //�����״ε�½
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
