using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //静态实例
    public static Player Instance;
    //基础属性
    public int life;
    public int atk;
    public int def;
    public int gold;
    public int level;
    public int exp;
    //声音资源
    AudioSource Audio;
    public AudioClip WalkClip;
    public AudioClip AttackClip;
    public AudioClip DamageClip;
    public AudioClip JumpClip;
    enum MoveState
    {
        Idle,
        Walk,
        Run,
    }
    //角色组件
    Transform m_transform;
    //记录着地
    bool isGroud;
    Vector3 pointBottom, pointTop;
    float radius;
#if UNITY_ANDROID || UNITY_IPHONE
    //手机端摇杆
    ETCJoystick MoveJoystick;
    ETCJoystick RotateJoystick;
#endif
    //摄像机组件
    Transform cam;
    //摄像机
    Camera m_camera;
    //渲染层
    int FirstMask = ~(1 << 12 | 1 << 14);
    int ThirdMask = ~(1 << 14);
    int BattleMask = 1 << 12 | 1 << 13 | 1 << 16;
    //摄像机旋转角
    Vector3 camRot;
#if UNITY_STANDALONE_WIN
    //记录鼠标位置
    Vector2 MousePosition;
#endif
    //动画控制器
    [HideInInspector]
    public Animator ani = null;
    //动画状态
    AnimatorStateInfo stateInfo;
    //角色移动距离
#if UNITY_STANDALONE_WIN
    float move_x = 0;
    float move_z = 0;
#endif
    [HideInInspector]
    public float move_y = 0;
    //记录角色位置以计算速度
    Vector3 LatePos;
    //战斗时转向敌人所用的记录位置时间的参数
    Vector3 BattlePlayerPos;
    Vector3 BattleEnemyPos;
    float BattleRotateTime = 0;
    //记录速度
    float Speed;
    //视角类型
    public enum Perspective
    {
        First = 1,
        Second = 2,
        Third = 3,
        Battle = 4
    }
    //视角
    public static Perspective perspective;
    public static Perspective lastperspective;
    //光线碰撞
    Ray ray;
    RaycastHit hit;
    //交互中的场景/物体与物品
    GameObject Delegater;
    Item DelegaterItem;
    Door door;
    float RayDistance = 1f;
    //角色控制器组件
    CharacterController ch;
    //第三视角距离参数
    [HideInInspector]
    public float Third_perspective = 1f;
    //摄像机高度
    [HideInInspector]
    public float camHeight = 0.6f;
    //重力
    public float gravity;
    //跳跃间隔
    public float jump_time;
    //跳跃CD
    [HideInInspector]
    public float jump_cd;
    //攻击间隔
    [HideInInspector]
    public float attack_time = 1.0f;
    //奔跑速度
    public float run_speed = 2.0f;
    //角速度
    public float paltance;
    public float max_paltance;
    public float min_paltance;
    //步行速度
    public float walk_speed = 1.0f;
    //跳跃起始速度
    public float jump_speed;
    //移动许可
    [HideInInspector]
    public bool movable = true;
    //转向许可
    [HideInInspector]
    public bool rotable = true;
    //交互中的怪物
    [HideInInspector]
    public Enemy enemy = null;
    //初始化操作
    private void Start()
    {
        //实例
        Instance = GameObject.Find("Player").GetComponent<Player>();
        //自动存档
        //Old_Save.Instance[0].SetPlayerSave();
        //增加监听器
        gameObject.transform.Find("Main Camera").gameObject.AddComponent<AudioListener>();
        //获取组件
#if UNITY_ANDROID || UNITY_IPHONE
        MoveJoystick = GameObject.Find("New Joystick").GetComponent<ETCJoystick>();
        RotateJoystick = GameObject.Find("New Joystick2").GetComponent<ETCJoystick>();
#endif
        Audio = GetComponent<AudioSource>();
        perspective = Perspective.First;
        ch = gameObject.GetComponent<CharacterController>();
        m_transform = transform;
        cam = Camera.main.transform;
        ani = GetComponent<Animator>();
        m_camera = cam.GetComponent<Camera>();
        //相机初始位置调整
        cam.position = m_transform.TransformPoint(0, camHeight, 0);
        cam.rotation = m_transform.rotation;
        camRot = m_transform.eulerAngles;
        //动画待机开关
        ani.SetBool("Idle", true);
        stateInfo = ani.GetCurrentAnimatorStateInfo(0);
        //交互物体置空
        DelegaterItem = Item.FindWithName("红钥匙");
        //人物记录坐标调整
        LatePos = transform.position;
        //跳跃CD重置
        jump_cd = jump_time;
#if UNITY_STANDALONE_WIN
        //锁屏
        Lock = true;
#endif
    }
    private void Update()
    {
        //锁屏时允许转向
        if (Lock)
        {
            Rotato();
        }
        //战斗时转向敌人
        else if (GameManager.Battle)
        {
            BattleRotato();
        }
        Ani();
        //允许移动
        if (movable && !GameManager.Battle)
        {
            //跳跃
            JumpOrDrop();
            //移动
            Move();
            //交互场景信息
            RayHit();
        }
        //不检测光线时关闭HELP UI
        else
        {
            if (GameManager.Instance.Help)
            {
                GameManager.Instance.Help = false;
            }
        }
        Cam();
        //手机版UI控制
#if UNITY_ANDROID || UNITY_IPHONE
        GameManager.Instance.PhoneUIActive = movable;
#endif
    }
    //检测碰撞物体
    void RayHit()
    {
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit, RayDistance, ~((1 << LayerMask.NameToLayer("Invisible")) | (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("UI")))))
        {
            RayhitTag(hit.collider.transform);
            return;
        }
        //不交互
        else
        {
            if (DelegaterItem.MomentoryUsable)
            {
                DelegaterItem.MomentoryUsable = false;
                door.AbleUseKey = false;
                if (GameManager.Instance.Help)
                {
                    GameManager.Instance.Help = false;
                }
            }
        }
    }
    //检测照射物
    void RayhitTag(Transform obj)
    {
        switch (obj.tag)
        {
            //与门交互
            case "Door":
                Delegater = hit.collider.gameObject;
                door = hit.collider.GetComponentInParent<Door>();
                DelegaterItem = Item.FindWithName(door.KeyName);
                GameManager.Instance.HelpTip("使用" + door.KeyName + "开门");
                if (!DelegaterItem.MomentoryUsable)
                {
                    DelegaterItem.MomentoryUsable = true;
                    door.AbleUseKey = true;
                }
                return;
            //与敌人交互
            case "Enemy":
                enemy = hit.collider.GetComponent<Enemy>();
                GameManager.Instance.PreviewEnemy(enemy);
                return;
            //与场景交互
            default:
                if (!obj.parent.CompareTag("Scene"))
                {
                    RayhitTag(obj.parent);
                    return;
                }
                if (DelegaterItem.MomentoryUsable)
                {
                    DelegaterItem.MomentoryUsable = false;
                    door.AbleUseKey = false;
                }
                if (GameManager.Instance.Help)
                {
                    GameManager.Instance.Help = false;
                }
                return;
        }
    }
    //控制转向的函数
    void Rotato()
    {
#if UNITY_STANDALONE_WIN
        //获取鼠标的移动距离
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        //战斗时禁止手动转向
        if (!GameManager.Battle)
        {
            camRot.x -= v;
            camRot.y += h;
        }
        cam.eulerAngles = camRot;
        Vector3 camrot = cam.eulerAngles;
        camrot.x = 0;
        camrot.z = 0;
        m_transform.eulerAngles = camrot;
#elif UNITY_ANDROID || UNITY_IPHONE
#if false
        if (!GameManager.Battle)
        {
            //触屏时
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    MousePosition = Input.GetTouch(0).position;
                }
                if ((Input.touches[0].phase == TouchPhase.Moved))
                {
                    cam.eulerAngles = new Vector3(MousePosition.y - Input.GetTouch(0).position.y, MousePosition.x - Input.GetTouch(0).position.x, 0);
                    MousePosition = Input.GetTouch(0).position;
                }
                Vector3 camrot = cam.eulerAngles;
                camrot.x = 0;
                camrot.z = 0;
                player.localEulerAngles = camrot;
            }
        }
#endif
#endif
    }
    //控制移动的函数
    void Move()
    {
        //计算速度
        Speed = speed;
#if UNITY_STANDALONE_WIN
        //PC端键盘控制移动
        move_x = 0;
        move_z = 0;
        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKey(KeyCode.W)) move_z += run_speed;
            if (Input.GetKey(KeyCode.S)) move_z -= run_speed;
            if (Input.GetKey(KeyCode.A)) move_x -= run_speed;
            if (Input.GetKey(KeyCode.D)) move_x += run_speed;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) move_z += walk_speed;
            if (Input.GetKey(KeyCode.S)) move_z -= walk_speed;
            if (Input.GetKey(KeyCode.A)) move_x -= walk_speed;
            if (Input.GetKey(KeyCode.D)) move_x += walk_speed;
        }
        //更新上一帧的坐标
        LatePos = transform.position;
        //移动命令处理
        ch.Move(m_transform.TransformDirection(move_x, move_y, move_z) * Time.deltaTime);
#elif UNITY_ANDROID || UNITY_IPHONE
        //更新上一帧的坐标
        LatePos = transform.position;
#endif
        //音效处理
        switch (State)
        {
            //奔跑
            case MoveState.Run:
                Runing();
                break;
            //行走
            case MoveState.Walk:
                Walking();
                break;
            //待机
            case MoveState.Idle:
                break;
            //待补充
            default:
                break;
        }
    }
    //跳跃
    public void JumpOrDrop()
    {
        //碰头时
        if ((ch.collisionFlags & CollisionFlags.Above) != 0)
        {
            move_y = move_y > 0 ? -move_y : move_y;
        }
        //跳跃CD
        if (jump_time > jump_cd)
        {
            jump_cd += Time.deltaTime;
            if (jump_cd > jump_time)
            {
                jump_cd = jump_time;
            }
            GameManager.Instance.SetJumpProgess();
        }
        //着地时
        else if (ch.isGrounded)
        {
            //跳跃
            if (Input.GetKey(KeyCode.Space) || GameManager.Instance.Jump.State == 2)
            {
                jump_cd = 0;
                GameManager.Instance.SetJumpProgess();
                move_y = jump_speed;
                ani.SetBool("Jump", true);
                AudioManager.Instance.SePlayOnece(transform, JumpClip);
            }
            else
            {
                move_y = 0;
                return;
            }
        }
        //重力
        move_y -= gravity * Time.deltaTime;
    }
    //控制锁屏与视角转动的函数
    public static bool Lock
    {
#if UNITY_STANDALONE_WIN
        //PC端
        get
        {
            return Instance.rotable;
        }
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Instance.rotable = value;
            Instance.movable = value;
            if (value)
            {
                GameManager.UIControlLock();
            }
        }
#elif UNITY_ANDROID || ANDROID_IPHONE
        //手机端
        get
        {
            return Instance.rotable;
        }
        set
        {
            Instance.rotable = value;
        }
#endif
    }
    //控制相机的函数
    void Cam()
    {
        //调整相机视角
        switch (perspective)
        {
            case Perspective.First:
                FirstPerspective();
                break;
            case Perspective.Second:
                break;
            case Perspective.Third:
                ThirdPerspective();
                break;
            case Perspective.Battle:
                BattlePerspective();
                break;
        }
    }
    //相机与人物位置一步对齐初始位置
    void CamStart()
    {
        cam.transform.rotation = new Quaternion(0, 0, 0, 0);
        cam.transform.position = transform.TransformPoint(0, camHeight, 0);
    }
    //不同视角下的相机算法
    void FirstPerspective()
    {
        //剔除角色渲染层
        m_camera.cullingMask = FirstMask;
        m_camera.nearClipPlane = 0.15f;
        cam.position = m_transform.TransformPoint(0, camHeight, 0);
    }
    void ThirdPerspective()
    {
        //相机不渲染
        m_camera.cullingMask = ThirdMask;
        m_camera.nearClipPlane = 0.3f;
        cam.position = m_transform.TransformPoint(0, camHeight + Third_perspective * 0.3f, -Third_perspective);
    }
    void BattlePerspective()
    {
        //修正视角避免转向视角
        if (perspective != Perspective.Battle)
        {
            perspective = lastperspective;
        }
        //相机渲染Battle层
        m_camera.cullingMask = BattleMask;
        m_camera.nearClipPlane = 0.5f;
        //此处可添加修正角色高度

        //获取转心
        Vector3 pos = transform.TransformPoint(0, 0, 0.3f);
        //判断旋转角度
        if (cam.localRotation.y > -0.3f)
        {
            cam.transform.RotateAround(pos, Vector3.up, -Time.deltaTime * 80f);
        }
        //判断战斗视角距离转心的距离
        if ((cam.transform.position - pos).magnitude < 1f)
        {
            cam.position = cam.transform.TransformPoint(0, 0, -Time.deltaTime * 20f);
        }
    }
    //战斗时处理渲染者
    void BattleMaskSet()
    {
        enemy.gameObject.layer = LayerMask.NameToLayer("Battle");
        foreach (Transform obj in enemy.GetComponentsInChildren<Transform>())
        {
            obj.gameObject.layer = LayerMask.NameToLayer("Battle");
        }
    }
    //战斗时控制角色强制转向怪物的函数
    void BattleRotato()
    {
        //获取水平分量插值坐标
        Vector3 SlerpPos = Vector3.Slerp(BattlePlayerPos, BattleEnemyPos, BattleRotateTime * paltance);
        //角色转向插值坐标
        transform.LookAt(SlerpPos);
        //增加时间达到匀速插值
        BattleRotateTime += Time.deltaTime;
    }
    //进入战斗时触发的函数
    public void BeginBattle()
    {
        //战斗前控制相机为第一视角状态
        FirstPerspective();
        //初始化角色与相机的位置与角度
        CamStart();
        //战斗时处理渲染的物体为角色与怪物
        BattleMaskSet();
        //记录初始位置与角度
        BattlePlayerPos = transform.TransformPoint(0, 0, 1f);
        BattleEnemyPos = enemy.mesh.position;
        //将敌人的竖直高度调整为与角色相同
        BattleEnemyPos.y = BattlePlayerPos.y;
        //清空时间
        BattleRotateTime = 0;
    }
    //结束战斗时触发的函数
    public void EndBattle()
    {
        //锁屏
        Lock = true;
        //转回战斗偏移视角
        Vector3 pos = transform.TransformPoint(0, 0, 0.3f);
        cam.transform.RotateAround(pos, Vector3.up, 0.3f);
        //修正视角
        CamStart();
    }
    //判断玩家是否死亡的索引器
    public bool IsDeath
    {
        get
        {
            if (life <= 0)
            {
                return true;
            }
            return false;
        }
    }
    //获得金币
    public void Addgold(int addition)
    {
        gold += addition;
        GameManager.Instance.SetGold();
    }
    //获得经验
    public void Addexp(int addition)
    {
        exp += addition;
        GameManager.Instance.SetExp();
    }
    //控制动画的函数
    void Ani()
    {
        //读取动画状态
        stateInfo = ani.GetCurrentAnimatorStateInfo(0);
        //战斗时的动画处理
        if (GameManager.Battle)
        {
            if (stateInfo.fullPathHash == Animator.StringToHash("Player.Attack") && !ani.IsInTransition(0))
            {
                ani.SetBool("Attack", false);
            }
            if (stateInfo.fullPathHash == Animator.StringToHash("Player.Damage") && !ani.IsInTransition(0))
            {
                ani.SetBool("Damage", false);
            }
            ani.SetBool("Run", false);
            ani.SetBool("Walk", false);
            ani.SetBool("Jump", false);
            return;
        }
        //判断跳跃状态结束
        if (stateInfo.fullPathHash == Animator.StringToHash("Player.Jump") && !ani.IsInTransition(0))
        {
            ani.SetBool("Jump", false);
        }
        //移动时的动画处理
        if (State != MoveState.Idle)
        {
            if (State == MoveState.Run)
            {
                ani.SetBool("Run", true);
                ani.SetBool("Walk", false);
            }
            else
            {
                ani.SetBool("Run", false);
                ani.SetBool("Walk", true);
            }
        }
        //待机时的动画处理
        else
        {
            ani.SetBool("Walk", false);
            ani.SetBool("Run", false);
        }
    }
    //移动时的动画与音效
    public void Walking()
    {
        AudioManager.Instance.SePlayLoop(transform, WalkClip, 1.1f);
    }
    //奔跑时的动画与音效
    public void Runing()
    {
        AudioManager.Instance.SePlayLoop(transform, WalkClip, 2.5f);
    }
    //获取运动速度
    float speed
    {
        get
        {
            Vector3 a = transform.position;
            a.y = 0;
            Vector3 b = LatePos;
            b.y = 0;
            float v = Vector3.Distance(a, b) / Time.deltaTime;
            return v;
        }
    }
    //获取运动状态
    MoveState State
    {
        get
        {
            if (Speed < 0.1f)
            {
                return MoveState.Idle;
            }
            if
#if UNITY_ANDROID || UNITY_IPHONE
                (Speed > 1f)
#elif UNITY_STANDALONE_WIN
                (Input.GetKey(KeyCode.R))
#endif
            {
                return MoveState.Run;
            }
            else
            {
                return MoveState.Walk;
            }
        }
    }
    //读档改变位置
    public void Move(Vector3 vector3)
    {
        transform.position = vector3;
    }
    //检测着地
    public bool isGroudedForBattle;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (transform.position.y - hit.transform.position.y < 0.4f)
            {
                isGroudedForBattle = true;
            }
            else
            {
                isGroudedForBattle = false;
            }
        }
        else
        {
            isGroudedForBattle = true;
        }
        if (hit.gameObject.CompareTag("Enemy"))
        {
            hit.gameObject.GetComponent<Enemy>().ReadyBattle();
        }
    }
}
