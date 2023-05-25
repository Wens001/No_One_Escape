
/****************************************************
 * FileName:		GameManager.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-12-10:29:08
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
public class GameDataCount
{
    public int killHuman;
    public int helpHuman;
    public int clickTrigger;
}

public class GameManager : Singleton<GameManager>
{
    public Material pab;
    public static bool UIIsMove ;
    public GameDataCount gameDataCount = new GameDataCount();
    public GameObject canvasPrefab;
    public GameObject helpRebonePrefab;
    public GameObject bloodPool;

    public GameObject bloodSplatCritical;

    

    public void CreatehelpRebone(Vector3 pos,HumanBase _base )
    {
        if (helpRebonePrefab == null)
            return;
        var go = PoolManager.SpawnObject(helpRebonePrefab, pos,Quaternion.identity) ;
        go.transform.TryGetComponent(out HelpRebone helpRebone);
        helpRebone.helpHuman = _base;
    }


    public static float Speed { get; private set; } = 1;
    public static float DeltaTime { get { return Speed * Time.deltaTime; } }
    public List<HumanBase> humanBases = new List<HumanBase>();

    public static bool isWin { get; set; }
    public static bool isDead { get; set; }

    private void Awake()
    {
        AppLovinCrossPromo.Init();
        pab.SetColor("_Color", new Color(1, 1, 1, 0));
        humanBases.Clear();
        foreach (var hb in FindObjectsOfType<HumanBase>())
            humanBases.Add(hb);
        LiveHumanSize = humanBases.Count;
        GoOutHumanSize = 0;
        if (ZombieShowTimer.Instance.IsNotZomble == false)
            LiveHumanSize--;

        upDoor = FindUpDoor();

        if (GameObject.Find("Canvas") == null)
        {
            var canvas = Instantiate(canvasPrefab);
        }

    }

    private void Start()
    {
        AudioManager.Instance.ChangeMusicVolume(0);
        GameStop();

    }

    private void OnEnable()
    {
        isWin = false;
        isDead = false;
        Messenger.AddListener<HumanBase,ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
        Messenger.AddListener<HumanBase, HumanBase>(ConstValue.CallBackFun.PlayerDead, PlayerDeadListener);
        Messenger.AddListener<HumanBase,HumanBase>(ConstValue.CallBackFun.PlayerRebone, PlayerReboneListener);
        Messenger.AddListener<Door,HumanBase>(ConstValue.CallBackFun.PlayerGoOut, PlayerGoOutListener);
        Messenger.AddListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, ZombieShowFunc);

        UIPanelManager panelManager = UIPanelManager.Instance;
        for(int i=0;i<10;i++)
            panelManager.PopPanel();

        panelManager.PushPanel(UIPanelType.GamePanel);
        panelManager.PushPanel(UIPanelType.BloodEffect);
        panelManager.PushPanel(UIPanelType.TapToPlay);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<HumanBase,ButtonProp>(ConstValue.CallBackFun.ButtonDown, ListenerDoor);
        Messenger.RemoveListener<HumanBase,HumanBase>(ConstValue.CallBackFun.PlayerDead, PlayerDeadListener);
        Messenger.RemoveListener<HumanBase,HumanBase>(ConstValue.CallBackFun.PlayerRebone, PlayerReboneListener);
        Messenger.RemoveListener<Door, HumanBase>(ConstValue.CallBackFun.PlayerGoOut, PlayerGoOutListener);
        Messenger.RemoveListener<HumanBase>(ConstValue.CallBackFun.ZombieShow, ZombieShowFunc);
    }

    #region 设置上方的门
    /// <summary>
    /// 所有按钮
    /// </summary>
    public ButtonProp[] ButtonProps {
        get
        {
            if (upDoor == null)
                upDoor = FindUpDoor();
            return upDoor.groups.bounds;
        }
    }

    /// <summary>
    /// 还剩多少个按钮
    /// </summary>
    public int ButtonSize { get {
            if (upDoor == null)
                upDoor = FindUpDoor();
            return upDoor.propSize;
        }
    }

    public Door upDoor { get; private set; }     //剩余门的数量upDoor.propSize
    public Door[] doors { get; private set; }

    private Door FindUpDoor()
    {
        doors = FindObjectsOfType<Door>();
        if (doors == null || doors.Length == 0)
            return null;
        int index = 0;
        float x = doors[0].transform.position.x;
        for (int i = 1; i < doors.Length; i++)
        {
            if (doors[i].transform.position.x < x)
            {
                index = i;
                x = doors[i].transform.position.x;
            }
        }
        return doors[index];
    }


    public void ListenerDoor(HumanBase human,ButtonProp button)
    {
        AudioManager.Instance.PlaySound(1);
        this.AttachTimer(.1f,() => {
            if (GameSetting.VibrationOn.Value == 1)
                MMVibrationManager.Haptic(HapticTypes.Warning);

            if (upDoor.propSize <= 0 )
            {
                AudioManager.Instance.PlaySound(2);
                OpenDoor();
            }
        });
        if (human.IsMe)
            ShowCoinEffect(human.transform, 0);
    }

    #region 开门逻辑

    private bool isOpenDoor = false;
    public void OpenDoor()
    {
        if (isOpenDoor)
            return;
        SceneSettingUI.View.Value = false;
        isOpenDoor = true;

        GameStop();
        CameraFllow.Instance.SetOtherShow(upDoor.transform, new Vector3(5f, 8f, 0));
        Register(upDoor.transform);
    }

    private void Register(Transform target)
    {
        this.AttachTimer(.4f,
            () => {
                if (GameSetting.VibrationOn.Value == 1)
                    Handheld.Vibrate();
                if (CameraFllow.Instance.IsAtTarget)
                {
                    foreach (var door in doors)
                    {
                        door.OpenDoor();
                    }
                    this.AttachTimer(1f,
                        () => {
                            CameraFllow.Instance.SetPlayer();
                            GameContinue();
                            if (!LocalNavMeshBuilder.Instance.IsAsync)
                                LocalNavMeshBuilder.Instance.UpdateNavMesh(true);
                        }
                    );
                }
                else
                {
                    Register(target);
                }
            }
        );
    }

    #endregion

    #endregion

    private void ZombieShowFunc(HumanBase target)
    {
        if (target == PlayerControl.Instance)
        {
            pab.SetColor("_Color", new Color(1, 1, 1, .1f));
            foreach (var hb in humanBases)
            {
                if (hb == null || hb == PlayerControl.Instance)
                    continue;
                hb.TryGetComponent(out AnimControl ac);
                ac?.SetMaterial(0);
            }
        }
    }

    #region 监听玩家信息

    private void PlayerGoOutListener(Door door,HumanBase target)
    {
        if (isWin || isDead || target.IsDead)
            return;
        LiveHumanSize--;
        GoOutHumanSize++;
        
        //玩家走出大门
        if (target.IsHuman && !isDead && !isWin)
        {
            target.IsWin = true;
            target.WinTargetDir = door.dirdir;
            if (target == PlayerControl.Instance)
            {
                CameraFllow.Instance.LookAtTarget(target.transform, new Vector3(0, 1.75f, -2.5f));
                this.AttachTimer(.6f, () => {
                    GameStop();
                    target.animControl.SetValue(ConstValue.AnimatorStr.Dance);
                    target.animControl.SetValue(ConstValue.AnimatorStr.DanceIndex, 1f);
                });

                DelaySetWin(2);
            }
        }

        //AI走出大门
        if ( PlayerControl.Instance.playerType == PlayerType.Zombie && !isDead && !isWin)
        {
            CameraFllow.Instance.LookAtTarget(target.transform, new Vector3(0, 1.75f, -2.5f));
            this.AttachTimer(.6f, () => {
                GameStop();
                target.animControl.SetValue(ConstValue.AnimatorStr.Dance);
                target.animControl.SetValue(ConstValue.AnimatorStr.DanceIndex, 1f);
            });
            DelaySetDefeat(2);
        }
        
    }

    public void SetDefeat()
    {
        if (!isDead && !isWin)
        {
            isDead = true;
            Messenger.Broadcast(ConstValue.CallBackFun.GameOver);
            UIPanelManager.Instance.PushPanel(UIPanelType.DefeatPanel);
        }
    }

    public void DelaySetDefeat(float delayT)
    {
        if (!isDead && !isWin)
        {
            isDead = true;
            this.AttachTimer(delayT, () => {
                Messenger.Broadcast(ConstValue.CallBackFun.GameOver);
                UIPanelManager.Instance.PushPanel(UIPanelType.DefeatPanel);
            });
        }
    }

    public void SetWin()
    {
        if (!isWin && !isDead)
        {
            isWin = true;
            Messenger.Broadcast(ConstValue.CallBackFun.GameOver);
            UIPanelManager.Instance.PushPanel(UIPanelType.WinPanel);
        }
    }

    public void DelaySetWin(float delayT)
    {
        if (!isDead && !isWin)
        {
            isWin = true;
            this.AttachTimer(delayT, () => {
                Messenger.Broadcast(ConstValue.CallBackFun.GameOver);
                UIPanelManager.Instance.PushPanel(UIPanelType.WinPanel);
            });
        }
    }

    public static int LiveHumanSize { get; private set; }   //活着在房间的幸存者
    public static int GoOutHumanSize { get; private set; }   //走出的幸存者

    #region 特效

    public GameObject moneyCoinPrefab;
    /// <summary>
    /// 金币特效
    /// </summary>
    /// <param name="pos"></param>
    public void ShowCoinEffect(Transform trans, int addmoney)
    {
        var prefab = moneyCoinPrefab;
        Messenger.Broadcast<int>(ConstValue.CallBackFun.AddMoney, addmoney);
        var pr = PoolManager.SpawnObject(prefab,
            trans.position + Vector3.up * 1f, prefab.transform.rotation);
        this.AttachTimer(2f, () => { PoolManager.ReleaseObject(pr); });
    }

    public GameObject XuanYunPrefab;

    public void XuanYun(HumanBase target,float time)
    {
        var prefab = XuanYunPrefab;
        var pr = Instantiate(prefab,
            target.Position + Vector3.up * 1.4f, prefab.transform.rotation);
        pr.transform.SetParent(target.transform);
        pr.TryGetComponent(out VertigoBuff vb);
        vb.target = target;
        vb.DurationTime = time;
    }


    #endregion





    private void PlayerDeadListener(HumanBase killer, HumanBase target)
    {
        LiveHumanSize--;
        //如果不存在屠夫，则退出
        if (killer == null || killer == target)
            return;
        //游戏计数
        if (killer.IsMe)
            gameDataCount.killHuman++;
        if (GameSetting.VibrationOn.Value == 1)
            MMVibrationManager.Haptic(HapticTypes.Warning);
        
        //金币特效
        if (killer.IsMe)
        {
            ShowCoinEffect(target.transform,0);
        }

        //血液特效
        var dir = target.Position - killer.Position;
        dir.y = 0;
        dir = dir.normalized;
        var go = PoolManager.SpawnObject(bloodSplatCritical, target.Position, Quaternion.LookRotation(dir, Vector3.up));
        this.AttachTimer(2,() => { PoolManager.ReleaseObject(go); });
        this.AttachTimer(1.25f, () => {
            var go2 = Instantiate(bloodPool, target.Position + Vector3.up * 0.02f, Quaternion.LookRotation(dir, Vector3.up));
            go2.transform.rotation = bloodPool.transform.rotation;
            this.AttachTimer(6,
                ()=> {
                    go2.TryGetComponent(out ParticleSystem ps);
                    var t = ps.main;
                    t.simulationSpeed = 0;
                }
                );
        });

        //死亡是自身，则失败
        if (target.IsMe )
        {
            DefeatPanelUI.KillerKill = true;
            DelaySetDefeat(2);
            return;
        }

        if (LiveHumanSize <= 0 && PlayerControl.Instance.IsZombie && !isWin && !isDead)
        {
            if (GoOutHumanSize <= 0)
            {
                ZombieShowTimer.Instance.KillerWin();
                Instance.DelaySetWin(3.5f);
            }
            else
                Instance.DelaySetDefeat(2);
            return;
        }

    }

    private void PlayerReboneListener(HumanBase rebonePlayer, HumanBase helpPlayer)
    {
        LiveHumanSize ++;
        if (helpPlayer && helpPlayer.IsMe )
        {
            gameDataCount.helpHuman++;
            ShowCoinEffect(rebonePlayer.transform, 0);
        }
    }

    #endregion

    #region 游戏设置
    /// <summary>
    /// 游戏暂停
    /// </summary>
    public void GameStop()
    {
        Speed = 0;
        InputManager.OpenTouch = false;
        for (int i = 0; i < humanBases.Count ; i++)
        {
            humanBases[i].StopMove();
        }
    }

    /// <summary>
    /// 游戏继续
    /// </summary>
    public void GameContinue()
    {
        if (Speed < 0.01f)
            Speed = 1;
        InputManager.OpenTouch = true;
    }

    public static void SetGameSpeed(float value)
    {
        value = Mathf.Clamp(value, 0, 10);
        Speed = value;
    }

    #endregion

    #region 作弊

    [Button("一键踩下所有开关")]
    private void AllButtonTouch()
    {
        var bg = GameObject.FindObjectOfType<ButtonGroup>();
        foreach (var item in bg.bounds)
        {
            if (humanBases[0].IsHuman)
                item.PlayAction(humanBases[0]);
            else
                item.PlayAction(humanBases[1]);
        }
    }

    [Button("一键杀死所有幸存者")]
    private void AllHumanKill()
    {
        foreach (var hb in humanBases)
        {
            if (!hb.IsDead && hb.IsHuman )
                hb.PlayerDead(ZombieShowTimer.ZombiePlayer);
        }
    }

    #endregion

}

public class LevelSetting
{
    public static void LoadNowLevel()
    {
        SDKInit.LogAchievedLevelEvent(Value.ToString("000"));
        SceneManager.LoadScene(GetLevelIndex());
        GC.Collect();
    }

    /// <summary>
    /// 转换到循环关卡
    /// </summary>
    /// <returns></returns>
    public static int GetLevelIndex()
    {
        var level = Value;
        int loopBeginIndex = 4;
        var sceneSize = SceneManager.sceneCountInBuildSettings;
        if (level < sceneSize)
            return level;
        var t = level;
        t = (t - sceneSize) % (sceneSize - loopBeginIndex);
        return t + loopBeginIndex ;
    }

    public static int Value { get { return Level.Value; } set { Level.Value = value; } }


    private static IntProperty _level;
    public static IntProperty Level
    {
        get
        {
            if (_level == null)
                _level = new IntProperty("Level", 1);
            return _level;
        }
    }
}

public class GameSetting
{
    #region 声音

    private static FloatProperty _sound;
    public static FloatProperty Sound
    {
        get{
            if (_sound == null)
            {
                _sound = new FloatProperty("SoundVolume");
                _sound.Property.Subscribe( _ => {
                    _ = Mathf.Clamp(_, 0, 1);
                    AudioManager.Instance.ChangeSoundVolume(_);
                } );
            }
            return _sound;
        }
    }

    #endregion

    #region 震动

    private static IntProperty _vibrationOn;
    public static IntProperty VibrationOn
    {
        get{
            if (_vibrationOn == null)
                _vibrationOn = new IntProperty("VibrationVolume");
            return _vibrationOn;
        }
    }

    private static IntProperty _money;
    public static IntProperty Money
    {
        get
        {
            if (_money == null)
            {
                _money = new IntProperty("Money",0);
                _money.Property.Subscribe( _=> {
                    if (VibrationOn.Value == 1)
                        MMVibrationManager.Haptic(HapticTypes.Selection);
                } );
            }
            return _money;
        }
    }

    private static IntProperty _coin;
    public static IntProperty Coin
    {
        get
        {
            if (_coin == null)
                _coin = new IntProperty("Coin",0);
            return _coin;
        }
    }

    #endregion
}
public abstract class MyProperty<T>
{
    public ReactiveProperty<T> Property { get; protected set; }
    public readonly string saveStr;
    public T Value { get { return Property.Value; } set { Property.Value = value; } }
    public MyProperty(string str)
    {
        saveStr = str;
        Property = new ReactiveProperty<T>();
    }
}
public class BoolProperty: MyProperty<bool>
{
    public BoolProperty(string str, bool defaultValue = false) : base(str)
    {
        Value = PlayerPrefs.GetInt(saveStr, defaultValue ? 1 : 0 ) != 0 ;
        Property.Subscribe(a => { PlayerPrefs.SetInt(saveStr, a ? 1 : 0 ); });
    }
}
public class IntProperty : MyProperty<int>
{
    public IntProperty(string str,int defaultValue = 1) : base(str)
    {
        Value = PlayerPrefs.GetInt(saveStr, defaultValue) ;
        Property.Subscribe(_ => { PlayerPrefs.SetInt(saveStr, _);});
    }
}
public class FloatProperty : MyProperty<float>
{
    public FloatProperty(string str, float defaultValue = 1f) : base(str)
    {
        Value = PlayerPrefs.GetFloat(saveStr, defaultValue) ;
        Property.Subscribe( _ => { PlayerPrefs.SetFloat(saveStr, _ ); });
    }
}
public class StringProperty: MyProperty<string>
{
    public StringProperty(string str, string defaultValue = "") : base(str)
    {
        Value = PlayerPrefs.GetString(saveStr, defaultValue) ;
        Property.Subscribe(_ => { PlayerPrefs.SetString(saveStr, _); });
    }
}
public class IntPropertyMax : MyProperty<int>
{
    protected BoolProperty maxProperty;
    public readonly int Max;
    public bool IsMax { get { return maxProperty.Value; } set { maxProperty.Value = value; } }
    public IntPropertyMax(string str, int defaultValue = 1,int max = 3) : base(str)
    {
        Value = PlayerPrefs.GetInt(saveStr, defaultValue) ;
        Max = max;
        maxProperty = new BoolProperty(str + "maxFlag", false);
        Property.Skip(1).Subscribe( 
            _ => {
                maxProperty.Value = _ != 0 && _ % Max == 0;
                PlayerPrefs.SetInt(saveStr, _);
            }
        );
    }
}

