using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    //静态实例
    public static GameManager Instance;
    //静态初始化开关
    public static bool Initial = false;
    //模式
    public enum Mode
    {
        title,
        play
    }
    public Mode mode;
    //记录读档
    int LoadSave
    {
        get
        {
            return PlayerPrefs.GetInt("LoadSign");
        }
        set
        {
            PlayerPrefs.SetInt("LoadSign", value);
        }
    }
    //标题UI
    Button Play;
    Button QuitButton;
    //音效控制UI
    Toggle SeToggle;
    Toggle BgmToggle;
    Slider SeSlider;
    Slider BgmSlider;
    //鼠标灵敏度调试UI
    Slider MouseSlider;
    //测试音效
    public AudioClip Se;
    public AudioClip Bgm;
    //点击音效
    public AudioClip ClickSe;
    //手机模式下控制移动用UI
    Transform JoystickTransform;
    [HideInInspector]
    public EnrichedButton Jump;
    //状态栏的文字UI
    Transform State;
    Text Level;
    Text Life;
    Text Atk;
    Text Def;
    Text Gold;
    Text Exp;
    //提示UI
    Text ItemHelp;
    //物品栏UI
    EnrichedButton[] ItemButton = new EnrichedButton[9];
    Text[] ItemCount = new Text[9];
    Image[] ItemImage = new Image[9];
    Text ItemExplain;
    GameObject ItemExplainObj;
    int MouseOnItem = -1;
    //物品提示UI
    Transform ItemHelpUI;
    Image ItemHelpImage;
    Text ItemHelpText;
    Text ItemNameText;
    Button ItemHelpQuitButton;
    //跳跃进度条UI
    Image JumpProgressImage;
    Text JumpProgressText;
    //战斗时的文字UI
    Transform ReadyBattle;
    [HideInInspector]
    public Button ReadyBattleButton_Yes;
    [HideInInspector]
    public Button ReadyBattleButton_No;
    Toggle ReadyBattleToggle;
    Transform BattleUI;
    Text PlayerName;
    Text EnemyName;
    Text PlayerAtk;
    Text PlayerDef;
    Text PlayerLife;
    Text EnemyLife;
    Text EnemyDef;
    Text EnemyAtk;
    Transform BattleReporter;
    Text BattleReporterText;
    Scrollbar BattleReporterScrollbar;
    ScrollRect BattleReporterScrollRect;
    //死亡时的UI
    Text DeathText;
    Button RestartButton;
    //角色组件
    Player player;
    //敌人组件
    Enemy enemy;
    //敌人信息预览
    Text PreviewEnemyText;
    //战斗状态开关
    public static bool Battle;
    //按键冷却时间
    public float press_time = 1f;
    //按键计时数组
    float[] pressed_time = new float[509];
    //UI淡出时间
    public float UIWeakenTime = 3f;
    //按键冷却委托
    delegate void Func();
    //暂停开关
    public static bool Pause = false;
    //战斗询问
    public static bool AskForReadyBattle;
    //迅速战斗
    public static bool QuickBattle = false;
    Toggle QuickBattleToggle;
    //暂停、存档、设置UI
    Button PauseButton;
    Button ContinueButton;
    Button TitletButton;
    Button SettingButton;
    Button SettingToPauseButton;
    Button SaveUIButton;
    Button LoadUIButton;
    EnrichedButton[] SaveButtons = new EnrichedButton[10];
    Button SaveToPauseButton;
    Transform PauseUI;
    Transform SettingUI;
    Transform SaveUI;
    Transform TipUI;
    Image TipImage;
    Text TipText;
    Button ContinueButtonInSetting;
    //判断锁屏的UI的父物体
    List<GameObject> UIObjects = new List<GameObject>();
    //交互信息物体中排序组件集合
    [HideInInspector]
    public List<ComparableComponent> comparableComponents;
    //根据不同模式进行初始化
    private void Start()
    {
        Instance = this;
        switch (mode)
        {
            case Mode.title:TitleModeStart();break;
            case Mode.play:PlayModeStart();break;
        }
    }
    //标题模式下初始化
    void TitleModeStart()
    {
        //存档初始化
        //Old_Save.Start();
        Save.Start();
        LoadSave = 0;
        //获取组件
        Play = GameObject.Find("Play").GetComponent<Button>();
        QuitButton = GameObject.Find("Quit").GetComponent<Button>();
        //添加委托
        Play.onClick.AddListener(NewStart);
        Play.onClick.AddListener(EndInitial);
        QuitButton.onClick.AddListener(Application.Quit);
    }
    //游戏模式下初始化
    void PlayModeStart()
    {
        //角色实例
        Player.Instance = GameObject.Find("Player").GetComponent<Player>();
        //场景改变时编号
        NumberObject();
        /*
        //场景改变时存档
        SceneManager.activeSceneChanged += delegate (Scene scene1, Scene scene2) 
        {
            if (scene1 != null)
            {
                SaveCurrentFloor(scene1);    
            }       
            LoadCurrentFloor(scene2);
        };
        */
#if UNITY_ANDROID || UNITY_IPHONE
        //亮度修改
        //SetBright(1f);
#endif
        //获取组件
        player = GameObject.Find("Player").GetComponent<Player>();
        Battle = false;
        Instance = this;
        Jump = GameObject.Find("Jump").GetComponent<EnrichedButton>();
        JoystickTransform = GameObject.Find("EasyTouchControlsCanvas").transform;
        //非手机模式下UI删除
#if UNITY_STANDALONE_WIN
        Destroy(JoystickTransform.gameObject);
        Jump.gameObject.SetActive(false);
#endif
        //获取UI
        SeToggle = GameObject.Find("SeToggle").GetComponent<Toggle>();
        BgmToggle = GameObject.Find("BgmToggle").GetComponent<Toggle>();
        SeSlider = GameObject.Find("SeSlider").GetComponent<Slider>();
        BgmSlider = GameObject.Find("BgmSlider").GetComponent<Slider>();
        MouseSlider = GameObject.Find("MouseSlider").GetComponent<Slider>();
        State = GameObject.Find("State").transform;
        Level = GameObject.Find("Level").GetComponent<Text>();
        Life = GameObject.Find("Life").GetComponent<Text>();
        Atk = GameObject.Find("Atk").GetComponent<Text>();
        Def = GameObject.Find("Def").GetComponent<Text>();
        Gold = GameObject.Find("Gold").GetComponent<Text>();
        Exp = GameObject.Find("Exp").GetComponent<Text>();
        ItemHelp = GameObject.Find("Help").GetComponent<Text>();
        for (int i = 1; i <= 9; i++)
        {
            ItemButton[i - 1] = GameObject.Find("Item" + i).GetComponent<EnrichedButton>();
            ItemCount[i - 1] = GameObject.Find("Itemcount" + i).GetComponent<Text>();
            ItemImage[i - 1] = ItemButton[i - 1].image;
        }
        ItemExplain = GameObject.Find("ItemExplain").GetComponent<Text>();
        ItemExplainObj = ItemExplain.gameObject;
        ItemHelpUI = GameObject.Find("ItemHelpUI").transform;
        ItemHelpText = GameObject.Find("ItemHelpText").GetComponent<Text>();
        ItemNameText = GameObject.Find("ItemNameText").GetComponent<Text>();
        ItemHelpImage = GameObject.Find("ItemImage").GetComponent<Image>();
        ItemHelpQuitButton = GameObject.Find("ItemHelpQuitButton").GetComponent<Button>();
        JumpProgressImage = GameObject.Find("JumpProgress").GetComponent<Image>();
        JumpProgressText = GameObject.Find("JumpProgressText").GetComponent<Text>();
        PreviewEnemyText = GameObject.Find("PreviewEnemy").GetComponent<Text>();
        ReadyBattle = GameObject.Find("ReadyBattle").transform;
        ReadyBattleButton_Yes = GameObject.Find("ReadyBattleButton_Yes").GetComponent<Button>();
        ReadyBattleButton_No = GameObject.Find("ReadyBattleButton_No").GetComponent<Button>();
        ReadyBattleToggle = GameObject.Find("ReadyBattleToggle").GetComponent<Toggle>();
        BattleUI = GameObject.Find("Battle").transform;
        PlayerName = GameObject.Find("PlayerName").GetComponent<Text>();
        PlayerLife = GameObject.Find("PlayerLife").GetComponent<Text>();
        PlayerAtk = GameObject.Find("PlayerAtk").GetComponent<Text>();
        PlayerDef = GameObject.Find("PlayerDef").GetComponent<Text>();
        EnemyName = GameObject.Find("EnemyName").GetComponent<Text>();
        EnemyLife = GameObject.Find("EnemyLife").GetComponent<Text>();
        EnemyAtk = GameObject.Find("EnemyAtk").GetComponent<Text>();
        EnemyDef = GameObject.Find("EnemyDef").GetComponent<Text>();
        BattleReporter = GameObject.Find("BattleReporter").transform;
        BattleReporterText = GameObject.Find("BattleReporterText").GetComponent<Text>();
        BattleReporterScrollbar = GameObject.Find("BattleReporterScrollbar").GetComponent<Scrollbar>();
        BattleReporterScrollRect = BattleReporter.GetComponent<ScrollRect>();
        DeathText = GameObject.Find("Death").GetComponent<Text>();
        RestartButton = GameObject.Find("Restart").GetComponent<Button>();
        PauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        TitletButton = GameObject.Find("TitleButton").GetComponent<Button>();
        ContinueButton = GameObject.Find("ContinueButton").GetComponent<Button>();
        SettingButton = GameObject.Find("SettingButton").GetComponent<Button>();
        SaveUI = GameObject.Find("SaveUI").transform;
        SaveUIButton = GameObject.Find("SaveButton").GetComponent<Button>();
        LoadUIButton = GameObject.Find("LoadButton").GetComponent<Button>();
        SaveToPauseButton = GameObject.Find("QuitSaveUIButton").GetComponent<Button>();
        for (int i = 1; i <= 10; i++)
        {
            SaveButtons[i - 1] = GameObject.Find("SaveButton" + i).GetComponent<EnrichedButton>();
        }
        TipUI = GameObject.Find("TipImage").transform;
        TipImage = TipUI.GetComponent<Image>();
        TipText = GameObject.Find("TipText").GetComponent<Text>();
        PauseUI = GameObject.Find("Pause").transform;
        SettingUI = GameObject.Find("SettingUI").transform;
        SettingToPauseButton = GameObject.Find("SettingToPauseButton").GetComponent<Button>();
        ContinueButtonInSetting = GameObject.Find("ContinueButtonInPause").GetComponent<Button>();
        //音效委托
        SeToggle.onValueChanged.AddListener(delegate (bool value) { AudioManager.Instance.SeActive = value; AudioManager.Instance.SePlayOnece(transform, Se); });
        SeToggle.isOn = AudioManager.Instance.SeActive;
        BgmToggle.onValueChanged.AddListener(delegate (bool value) { AudioManager.Instance.BgmActive = value; AudioManager.Instance.BgmPlay(transform, Bgm); });
        BgmToggle.isOn = AudioManager.Instance.BgmActive;
        SeSlider.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SeVolume = value; AudioManager.Instance.SePlayOnece(transform, Se); });
        SeSlider.value = AudioManager.Instance.SeVolume;
        BgmSlider.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.BgmVolume = value; AudioManager.Instance.BgmPlay(transform, Bgm); });
        BgmSlider.value = AudioManager.Instance.BgmVolume;
        //鼠标灵敏度调试委托
        MouseSlider.value = (player.paltance - player.min_paltance) / (player.max_paltance - player.min_paltance);
        MouseSlider.onValueChanged.AddListener((float value) => { player.paltance = (player.max_paltance - player.min_paltance) * value + player.min_paltance; });
        //帮助UI隐藏
        ItemHelp.gameObject.SetActive(false);
        //物品说明UI隐藏
        ItemExplainObj.SetActive(false);
        //物品栏委托添加
        for(int i = 0; i < 9; i++)
        {
            ItemButton[i].HighlightedEvent.AddListener(ExplainItem);
            ItemButton[i].NormalEvent.AddListener(RefreshItem);
            ItemButton[i].onClick.AddListener(UseItem);
        }
        //物品提示UI委托添加与隐藏
        ItemHelpUI.gameObject.SetActive(false);
        ItemHelpQuitButton.onClick.AddListener(delegate ()
        {
            ItemHelpUI.gameObject.SetActive(false);
            Pause = false;
            Player.Lock = true;
            UIControlLock();
        });
        //怪物信息UI隐藏
        PreviewEnemyText.gameObject.SetActive(false);
        //战斗UI委托与隐藏
        ReadyBattle.gameObject.SetActive(false);
        BattleReporter.gameObject.SetActive(false);
        BattleUI.gameObject.SetActive(false);
        ReadyBattleToggle.onValueChanged.AddListener(delegate (bool value) { AskForReadyBattle = value; });
        AskForReadyBattle = ReadyBattleToggle.isOn;
        //快速战斗
        QuickBattleToggle = GameObject.Find("QuickBattleToggle").GetComponent<Toggle>();
        QuickBattleToggle.onValueChanged.AddListener(call: delegate (bool value) { QuickBattle = value; });
        QuickBattle = QuickBattleToggle.isOn;
        //死亡UI隐藏
        DeathText.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        //暂停继续UI隐藏以及委托添加
        ContinueButton.onClick.AddListener(QuitPauseUI);
        PauseButton.onClick.AddListener(CallPauseUI);
        PauseUI.gameObject.SetActive(false);
        SettingUI.gameObject.SetActive(false);
        SettingToPauseButton.onClick.AddListener(CallPauseUI);
        SettingButton.onClick.AddListener(CallSettingUI);
        ContinueButtonInSetting.onClick.AddListener(QuitPauseUI);
        TitletButton.onClick.AddListener(Title);
        SaveUI.gameObject.SetActive(false);
        SaveUIButton.onClick.AddListener(CallSaveUI);
        LoadUIButton.onClick.AddListener(CallLoadUI);
        SaveToPauseButton.onClick.AddListener(QuitSaveUI);
        TipUI.gameObject.SetActive(false);
        //电脑端隐藏暂停
#if UNITY_STANDALONE_WIN
        PauseButton.gameObject.SetActive(false);
#endif
        ContinueButton.gameObject.SetActive(false);
        //状态栏初始化
        SetState();
        //重来按钮委托添加
        RestartButton.onClick.AddListener(Restart);
        //判断锁屏所需要的UI
        UIObjects.Add(PauseUI.gameObject);
        UIObjects.Add(ReadyBattle.gameObject);
        UIObjects.Add(RestartButton.gameObject);
        UIObjects.Add(BattleUI.gameObject);
        UIObjects.Add(SettingUI.gameObject);
        UIObjects.Add(SaveUI.gameObject);
        UIObjects.Add(ItemHelpUI.gameObject);
        //判断是否读档
        if (LoadSave > 0)
        {
            Save.Load(LoadSave);
            LoadSave = 0;
        }
    }
    private void Update()
    {
        //标题状态下不处理按键等判断
        if (mode == Mode.title)
            return;
        //播放BGM
        AudioManager.Instance.BgmPlay(transform, Bgm);
        //非战斗状态按T切换视角
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (!Battle)
            {
                Perspective_trans();
            }
        }
        //按P切换快速战斗
        if (Input.GetKeyUp(KeyCode.P))
        {
            QuickBattle = !QuickBattle;
            QuickBattleToggle.isOn = QuickBattle;
        }
        //非战斗状态按ESC暂停
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!Battle)
            {
                CallPauseUI();
            }
        }
        //按回车键继续
        if (Input.GetKeyDown(KeyCode.Return))
        {
            QuitPauseUI();
        }
        //按1-9键快速使用物品
        for(KeyCode key = KeyCode.Alpha1; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyUp(key))
            {
                //更新物品下标
                MouseOnItem = key - KeyCode.Alpha1;
                ItemButton[MouseOnItem].onClick.Invoke();
                //防止同时使用多个物品
                break;
            }
        }
    }
    //每个场景为所有必要物体编号
    void NumberObject()
    {
        //排序的组件
        comparableComponents = new List<ComparableComponent>();
        AddComparableComponents<Enemy>(comparableComponents);
        AddComparableComponents<Jewel>(comparableComponents);
        AddComparableComponents<Door>(comparableComponents);
        AddComparableComponents<Item>(comparableComponents);
        comparableComponents.Sort(new CompareSaveObjects());
        int i = 1;
        foreach(ComparableComponent comparableComponent in comparableComponents)
        {
            comparableComponent.id = i;
            i += 1;
        }
    }
    void AddComparableComponents<T>(List<ComparableComponent> comparableComponents) where T : MonoBehaviour
    {
        foreach (T t in GameObject.Find("Scene").GetComponentsInChildren<T>())
        {
            comparableComponents.Add(t.gameObject.AddComponent<ComparableComponent>());
        }
    }
    /*
    //即时存储楼层
    void SaveCurrentFloor(Scene scene)
    {
        Old_Save.Instance[0].SaveScene(scene);
    }
    //即时调用楼层
    void LoadCurrentFloor(Scene scene)
    {
        Old_Save.Instance[0].LoadScene(scene);
    }
    //建立存档的函数
    public void NewSave(int number)
    {
        Old_Save.Instance[number] = new Old_Save(number);
    }
    //存档的函数
    public void SaveTo(int number)
    {
        Old_Save.CopySave(0, number);
    }
    //读档的函数
    public void LoadTo(int number)
    {
        Old_Save.Load(number);
    }
    */
    //按键冷却委托（使用时请放在Update函数中）
    void Press(KeyCode key, Func func)
    {
        //当对应按键未冷却时
        if (pressed_time[(int)key] > 0)
        {
            pressed_time[(int)key] -= Time.deltaTime;
            return;
        }
        //当对应按键冷却时
        else
        {
            pressed_time[(int)key] = 0;
            if (Input.GetKey(key))
            {
                pressed_time[(int)key] = press_time;
                func();
            }
        }
    }
    //切换视角的函数
    void Perspective_trans()
    {
        if (Player.perspective == Player.Perspective.First)
        {
            Player.perspective = Player.Perspective.Third;
        }
        else
        {
            Player.perspective = Player.Perspective.First;
        }
    }
    //亮度调节函数
    private void SetBright(float Brightness)
    {
        AndroidJavaObject Activity = null;
        Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentAcitivity");
        Activity.Call("runOnUiThread", new AndroidJavaRunnable(delegate ()
        {
            AndroidJavaObject Window = null, Attributes = null;
            Window = Activity.Call<AndroidJavaObject>("getWindow");
            Attributes = Window.Call<AndroidJavaObject>("getAttributes");
            Attributes.Set("screenBrightness", Brightness);
            Window.Call("setAttributes", Attributes);
        }));
    }
    //角色能力改变时调整UI界面
    public void SetState()
    {
        SetLife();
        SetAtk();
        SetDef();
        SetGold();
        SetLevel();
        SetExp();
    }
    public void SetLife()
    {
        Life.text = "生命  " + player.life;
    }
    void SetAtk()
    {
        Atk.text = "攻击  " + player.atk;
    }
    void SetDef()
    {
        Def.text = "防御  " + player.def;
    }
    void SetLevel()
    {
        Level.text = "等级  " + player.level;
    }
    public void SetGold()
    {
        Gold.text = "金币  " + player.gold;
    }
    public void SetExp()
    {
        Exp.text = "经验  " + player.exp;
    }
    //鼠标提示使用物品信息
    public void HelpTip(string txt)
    {
        ItemHelp.gameObject.SetActive(true);
        ItemHelp.text = txt;
    }
    //没有提示信息时刷新部分UI
    public bool Help
    {
        get
        {
            if(ItemHelp.gameObject.activeSelf|| PreviewEnemyText.gameObject.activeSelf)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        set
        {
            if (!value)
            {
                ItemHelp.gameObject.SetActive(false);
                PreviewEnemyText.gameObject.SetActive(false);
            }
        }
    }
    //刷新当前鼠标所在的物体栏下标
    void RefreshItem()
    {
        for (int i = 0; i < 9; i++)
        {
            if (ItemButton[i].State== 1 || ItemButton[i].State== 2)
            {
                MouseOnItem = i;
                return;
            }
        }
        MouseOnItem = -1;
        if (ItemExplainObj.activeSelf)
            ItemExplainObj.SetActive(false);
        return;
    }
    //首次获取物品暂停画面并显示信息
    void FirstGetItem(string Name)
    {
        CallPauseUI();
        ItemExplainObj.SetActive(true);
        ItemExplain.text = Item.FindWithName(Name).Explain;
    }
    //获取物品时改变UI界面
    public void SetItem(string Name)
    {
        //获取对应物品
        Item item = Item.FindWithName(Name);
        //物品多于0
        if (item.count > 0)
        {
            //判断是否添加物品图标
            for (int i = 0; i < 9; i++)
            {
                //检索到图标直接修改数字
                if (ItemImage[i].sprite == item.sprite)
                {
                    ItemCount[i].text = "" + item.count;
                    return;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                //检索到空格子添加图标并修改数字
                if (ItemImage[i].sprite == null)
                {
                    ItemImage[i].sprite = item.sprite;
                    //修改透明度
                    RefreshItemUseAble(Name);
                    ItemCount[i].text = "" + item.count;
                    return;
                }
            }
        }
        //物品被使用完
        else
        {
            //搜索物品图标
            for (int i = 0; i < 9; i++)
            {
                //检索到图标时删去图标与数字
                if (ItemImage[i].sprite == item.sprite)
                {
                    ItemImage[i].sprite = null;
                    ItemImage[i].color = new Color(1, 1, 1, 0);
                    ItemCount[i].text = "";
                    return;
                }
            }
        }
    }
    //物品的使用类型改变时修改图标透明度
    public void RefreshItemUseAble(string Name)
    {
        Item item = Item.FindWithName(Name);
        for(int i=0;i<9;i++)
        {
            if (item.sprite == ItemImage[i].sprite)
            {
                //判断物品的使用类型赋予透明度
                float alpha;
                switch (item.useType)
                {
                    case Item.UseType.AlwaysUsable:
                        alpha = 1f;
                        break;
                    case Item.UseType.Disusable:
                        alpha = 0.5f;
                        break;
                    case Item.UseType.MomentoryDisusable:
                        alpha = 0.5f;
                        break;
                    case Item.UseType.MomentoryUsable:
                        alpha = 1f;
                        break;
                    case Item.UseType.OnceUsable:
                        alpha = 1f;
                        break;
                    default:
                        alpha = 0f;
                        break;
                }
                ItemImage[i].color = new Color(1, 1, 1, alpha);
                return;
            }
        }
    }
    //高亮时显示信息事件
    void ExplainItem()
    {
        //刷新物品栏鼠标位置
        RefreshItem();
        if (MouseOnItem < 0)
        {
            return;
        }
        //悬浮在空格上时
        if (ItemImage[MouseOnItem].sprite == null)
        {
            if (ItemExplainObj.activeSelf)
            {
                ItemExplainObj.SetActive(false);
            }
            return;
        }
        //悬浮在正常物品上时
        else
        {
            //依靠图片得到物品并获取说明文字开启说明UI
            if (!ItemExplainObj.activeSelf)
            {
                ItemExplainObj.SetActive(true);
            }
            ItemExplain.text = Item.FindWithSprite(ItemImage[MouseOnItem].sprite).Explain;
        }
    }
    //按下物品使用
    void UseItem()
    {
        if (MouseOnItem < 0)
        {
            return;
        }
        //无法使用的物品
        if (!Item.FindWithSprite(ItemImage[MouseOnItem].sprite).Useable)
        {
            return;
        }
        Item.Use(ItemImage[MouseOnItem].sprite);
    }
    //物品提示UI
    public void CallItemHelpUI(string itemName)
    {
        Pause = true;
        Player.Lock = false;
        Item item = Item.FindWithName(itemName);
        ItemHelpUI.gameObject.SetActive(true);
        ItemHelpText.text = item.explain;
        ItemHelpImage.sprite = item.sprite;
        ItemNameText.text = itemName;
    }
    //跳跃进度条UI
    public void SetJumpProgess()
    {
        JumpProgressImage.fillAmount = player.jump_cd / player.jump_time;
        if (player.jump_cd == player.jump_time)
        {
            JumpProgressText.text = "跳跃就绪";
        }
        else
        {
            JumpProgressText.text = (int)(player.jump_cd * 100f / player.jump_time) + "%";
        }
    }
    //怪物信息预览UI
    public void PreviewEnemy(Enemy enemy)
    {
        PreviewEnemyText.gameObject.SetActive(true);
        PreviewEnemyText.text = enemy.Preview;
    }
    //战斗准备UI
    public void ReadyBattleUI(bool value)
    {
        ReadyBattle.gameObject.SetActive(value);
    }
    //战斗时调用UI界面以及改变
    public void SetBattleUI(bool active)
    {
        //显示UI
        if (active)
        {
            enemy = player.enemy;
            BattleUI.gameObject.SetActive(true);
            BattleReporter.gameObject.SetActive(true);
            SetBattlePlayerAtk();
            SetBattlePlayerDef();
            SetBattlePlayerLife();
            SetEnemyAtk();
            SetEnemyDef();
            SetEnemyLife();
            SetEnemyName();
        }
        //关闭UI
        else
        {
            enemy = null;
            BattleUI.gameObject.SetActive(false);
            BattleReporter.gameObject.SetActive(false);
            BattleReporterText.text = "战斗信息：";
        }
    }
    public void SetBattlePlayerLife()
    {
        PlayerLife.text = "生命  " + enemy.Player_life;
    }
    void SetBattlePlayerAtk()
    {
        PlayerAtk.text = "攻击  " + enemy.Player_atk;
    }
    void SetBattlePlayerDef()
    {
        PlayerDef.text = "防御  " + enemy.Player_def;
    }
    void SetEnemyName()
    {
        EnemyName.text = enemy.name;
    }
    public void SetEnemyLife()
    {
        EnemyLife.text = "生命  " + enemy.life;
    }
    void SetEnemyAtk()
    {
        EnemyAtk.text = "攻击  " + enemy.atk;
    }
    void SetEnemyDef()
    {
        EnemyDef.text = "防御  " + enemy.def;
    }
    public void Battle_Reporter(string message)
    {
        BattleReporterText.text += "\r\n" + message;
    }
    //死亡时调用UI界面以及暂停游戏
    public void Death()
    {
        Pause = true;
        DeathText.gameObject.SetActive(true);
        RestartButton.gameObject.SetActive(true);
    }
    //新游戏
    void NewStart()
    {
        mode = Mode.play;
        Pause = false;
        Item.Initial();
        SceneManager.LoadScene("魔塔");
    }
    //重新开始
    void Restart()
    {       
        Pause = false;
        //物品数量清零
        foreach(Item item in Item.Items)
        {
            item.count = 0;
        }
        SceneManager.LoadScene("魔塔");
    }
    //返回标题
    void Title()
    {
        Pause = false;
        mode = Mode.title;
        Initial = false;
        SceneManager.LoadScene("游戏加载画面");
    } 
    //确认初始化结束
    void EndInitial()
    {
        Initial = true;
    }
    //当一些UI存在时固定锁屏的函数（每个UI关闭时进行判断）
    public static void UIControlLock()
    {
        foreach(GameObject obj in Instance.UIObjects)
        {
            if (obj.activeSelf)
            {
                Player.Lock = false;
                return;
            }
        }
    }
    //暂停的函数
    void CallPauseUI()
    {
        //判断是否在战斗询问
        if (ReadyBattle.gameObject.activeSelf)
        {
            ReadyBattleButton_No.onClick.Invoke();
            return;
        }
        Help = false;
        Pause = true;
        //显示暂停UI
        PauseUI.gameObject.SetActive(true);
        //隐藏设置UI
        SettingUI.gameObject.SetActive(false);
#if UNITY_ANDROID || UNITY_IPHONE
        //手机端开启继续UI
        PhoneUIActive = false;
        ContinueButton.gameObject.SetActive(true);
#elif UNITY_STANDALONE_WIN
        //取消锁屏
        Player.Lock = false;
#endif
    }
    //设置的函数
    void CallSettingUI()
    {
        //隐藏暂停UI
        PauseUI.gameObject.SetActive(false);
        //显示设置UI
        SettingUI.gameObject.SetActive(true);
    }
    //存档界面的函数
    void CallSaveUI()
    {
        //隐藏暂停UI
        PauseUI.gameObject.SetActive(false);
        //显示存档UI
        SaveUI.gameObject.SetActive(true);
        //调整UI文字
        for(int i = 1; i <= 10; i++)
        {
            SaveButtons[i - 1].transform.Find("Text").GetComponent<Text>().text = "存  档  " + i;
            SaveButtons[i - 1].onClick.RemoveAllListeners();
            SaveButtons[i - 1].onClick.AddListener(delegate ()
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (SaveButtons[j - 1].State == 3)
                    {
                        _ = new Save(j);
                        CallTipUI("存档 "+j+" 成功!");
                        return;
                    }
                }
            });
        }
    }
    //脱离存档界面的函数
    void QuitSaveUI()
    {
        //显示暂停UI
        PauseUI.gameObject.SetActive(true);
        //隐藏存档UI
        SaveUI.gameObject.SetActive(false);
    }
    //读档界面的函数
    void CallLoadUI()
    {
        //隐藏暂停UI
        PauseUI.gameObject.SetActive(false);
        //显示存档UI
        SaveUI.gameObject.SetActive(true);
        //调整UI文字
        for (int i = 1; i <= 10; i++)
        {
            SaveButtons[i - 1].transform.Find("Text").GetComponent<Text>().text = "读  档  " + i;
            SaveButtons[i - 1].onClick.RemoveAllListeners();
            SaveButtons[i - 1].onClick.AddListener(delegate ()
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (SaveButtons[j - 1].State == 3)
                    {
                        LoadSave = j;
                        foreach(ComparableComponent comparableComponent in comparableComponents)
                        {
                            comparableComponent.gameObject.SetActive(false);
                        }
                        SceneManager.LoadScene("魔塔");
                    }
                }
            });
        }
    }
    //显示存档提示的函数
    public void CallTipUI(string TipString)
    {
        TipText.text = TipString;
        TipImage.color += new Color(0, 0, 0, 1f);
        TipText.color += new Color(0, 0, 0, 1f);
        TipUI.gameObject.SetActive(true);
        StartCoroutine(WeakenTipUI());
    }
    //关闭提示UI的协程
    IEnumerator WeakenTipUI()
    {
        yield return new WaitUntil(()=> {
            TipImage.color -= new Color(0, 0, 0, Time.deltaTime / UIWeakenTime);
            TipText.color -= new Color(0, 0, 0, Time.deltaTime / UIWeakenTime);
            return TipImage.color.a <= 0;
        });
        TipUI.gameObject.SetActive(false);
        yield break;
    }
    //脱离暂停的函数
    void QuitPauseUI()
    {
        //暂停UI界面关闭
        PauseUI.gameObject.SetActive(false);
        ItemHelpUI.gameObject.SetActive(false);
        if (ReadyBattle.gameObject.activeSelf)
        {
            ReadyBattleButton_Yes.onClick.Invoke();
        }
        //脱离暂停时关闭物品信息
        if (ItemExplainObj.activeSelf)
        {
            ItemExplainObj.SetActive(false);
        }
        Pause = false;
#if UNITY_ANDROID || UNITY_IPHONE
        //手机端关闭继续UI
        ContinueButton.gameObject.SetActive(false);
        PhoneUIActive = true;
#elif UNITY_STANDALONE_WIN
        //锁屏
        Player.Lock = true;
#endif
    }
#if UNITY_ANDROID || UNITY_IPHONE
    //手机端关闭UI
    public bool PhoneUIActive
    {
        set
        {
            JoystickTransform.gameObject.SetActive(value);
            Jump.gameObject.SetActive(value);
        }
    }
#endif
}
