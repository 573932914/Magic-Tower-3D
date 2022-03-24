using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //怪物基础属性
    public string m_name;
    public int life;
    public int atk;
    public int def;
    public int gold;
    public int exp;
    public Transform mesh;
    //修正高度
    public float height = 0;
    //攻击间隔
    public float attack_time = 1.0f;
    //死亡时间
    public float death_time = 1.0f;
    //警戒范围
    public float guard = 1.5f;
    //攻击音效
    public AudioClip AttackClip;
    //受伤音效
    public AudioClip DamageClip;
    //角速度
    [HideInInspector]
    public float paltance = 2.0f;
    //获取角色及其属性
    Player player;
    [HideInInspector]
    public int Player_life;
    [HideInInspector]
    public int Player_atk;
    [HideInInspector]
    public int Player_def;
    //战斗前摇
    [HideInInspector]
    public float Battle_start = 1.0f;
    //自身组件
    Transform m_transform;
    //角色组件
    Transform player_transform;
    //动画控制器
    Animator ani;
    //动画状态
    AnimatorStateInfo stateInfo;
    //快速战斗的速率
    float QuickBattleRate = 2.5f;
    //初始化操作
    private void Start()
    {
        //获取组件
        m_transform = transform;
        player = GameObject.Find("Player").GetComponent<Player>();
        player_transform = player.transform;
        ani = GetComponent<Animator>();
    }
    private void Update()
    {
        //移动
        Move();
        //转向角色
        Rotato();
        //动画处理
        Ani();
    }
    //获取特性
    T GetSpecialty<T>()where T : Specialty
    {
        if (GetComponent<T>())
        {
            return GetComponent<T>();
        }
        else
        {
            return null;
        }
    }
    //战斗前的准备
    void Preparation()
    {
        GameManager.Battle = true;
        //角色视角改变
        Player.lastperspective = Player.perspective;
        Player.perspective = Player.Perspective.Battle; 
        //读入敌人
        player.enemy = this;
        player.BeginBattle();
        //初始化时读取玩家能力
        Player_atk = player.atk;
        Player_def = player.def;
        Player_life = player.life;
        //战斗UI
        GameManager.Instance.SetBattleUI(true);
        GameManager.Instance.Battle_Reporter("玩家与" + m_name + "发生了战斗!");
#if UNITY_ANDROID || UNITY_IPHONE
        GameManager.Instance.PhoneUIActive = false;
#endif
        //魔攻
        if (GetSpecialty<MagicAttack>())
        {
            MagicAttack magicAttack = gameObject.GetComponent<MagicAttack>();
            if (magicAttack.Active)
            {
                Player_def = (int)(1 - magicAttack.efficiency) * Player_def;
            }
        }
        //进入战斗协程
        StartCoroutine(Battle());
    }
    //控制战斗的协程
    IEnumerator Battle()
    {
        //快速战斗时修正战斗时间
        if (GameManager.QuickBattle)
        {
            Time.timeScale = QuickBattleRate;
        }
        else
        {
            player.attack_time = 1f;
        }
        yield return new WaitForSeconds(Battle_start);
        //先攻
        if (GetSpecialty<QuickAttack>())
        {
            QuickAttack quickAttack = gameObject.GetComponent<QuickAttack>();
            GameManager.Instance.Battle_Reporter("怪物拥有偷袭属性，能够偷袭角色"+quickAttack.times+"次！");
            for (int i = 0; i < quickAttack.times; i++)
            {
                Attack();
                yield return new WaitForSeconds(attack_time);
            }
        }
        //当怪物未死亡时持续战斗
        while (true)
        {
            //角色攻击
            Player_Attack();
            yield return new WaitForSeconds(player.attack_time); 
            //若怪物死亡
            if (IsDeath)
            {
                break;
            }
            //怪物攻击
            Attack();
            yield return new WaitForSeconds(attack_time);
            //若角色死亡
            if (Player_isDeath)
            {
                GameManager.Instance.Death();
                yield break;
            }
        }
        //死亡动画与等待
        ani.SetBool("Death", true);
        yield return new WaitForSeconds(death_time);
        //战斗结束
        End();
    }
    //控制怪物攻击的函数
    void Attack()
    {            
        //攻击动画
        ani.SetBool("Attack", true);
        player.ani.SetBool("Damage", true);
        //攻击音效
        AudioManager.Instance.SePlay(transform.position, AttackClip);
        //AudioManager.Instance.SePlay(player.transform, player.DamageClip);
        int damage;
        damage = atk - Player_def;
        //若伤害为0时
        if (damage < 0)
        {
            damage = 0;
        }
        Player_life -= damage;
        //UI更新
        GameManager.Instance.Battle_Reporter("怪物对玩家发动了攻击，造成了" + damage + "点伤害！");
        GameManager.Instance.SetBattlePlayerLife();
        //角色死亡
        if (Player_isDeath)
        {

        }
    }
    //控制角色攻击的函数
    void Player_Attack()
    {            
        //攻击动画
        player.ani.SetBool("Attack", true);
        ani.SetBool("Damage", true);
        //攻击音效
        AudioManager.Instance.SePlay(player_transform.position, player.AttackClip);
        //AudioManager.Instance.SePlay(transform, DamageClip);
        int damage;
        damage = Player_atk - def;
        //伤害为0
        if (damage < 0)
        {
            damage = 0;
        }
        life -= damage;
        if (life <= 0)
            life = 0;
        GameManager.Instance.Battle_Reporter("玩家对怪物发动了攻击，造成了"+damage+"点伤害！");
        GameManager.Instance.SetEnemyLife();
    }
    //判断怪物是否死亡的索引器
    public bool IsDeath
    {
        get
        {
            if (life <= 0)
            {
                //发布胜利信息
                GameManager.Instance.Battle_Reporter("战斗胜利！\r\n获得" + gold + "金币与" + exp + "经验");
                life = 0;
                return true;
            }
            return false;
        }
    }
    //判断角色是否死亡的索引器
    bool Player_isDeath
    {
        get
        {
            if (Player_life <= 0)
            {
                return true;
            }
            return false;
        }
    }
    //结束战斗时的函数
    void End()
    {
        //体力调整
        player.life = Player_life;
        GameManager.Instance.SetLife();
        //战斗奖励
        player.Addgold(gold);
        player.Addexp(exp);
        //脱离战斗
        GameManager.Battle = false;
        //允许角色移动
        player.movable = true;
        //战斗UI
        GameManager.Instance.SetBattleUI(false);
#if UNITY_ANDROID || UNITY_IPHONE
        GameManager.Instance.PhoneUIActive = true;
#endif
        //快速战斗时修正战斗时间
        if (GameManager.QuickBattle)
        {
            Time.timeScale = 1f;
        }
        //视角恢复
        Player.perspective = Player.lastperspective;
        player.EndBattle();
        //删除怪物
        gameObject.SetActive(false);
    }
    //控制移动与遇敌的函数（子类采用，此类不采用）
    void Move()
    {
        //以下战斗方式已经被改变
        //角色在警戒范围内时战斗
        //if (Vector3.Distance(transform.position, player.transform.position) < guard && !GameManager.Battle && Input.GetKey(KeyCode.Z))
        //{
        //    GameManager.Battle = true;
            //禁止角色移动与转向
        //    player.movable = false;
        //   player.rotable = false;
        //    Preparation();
        //}
    }
    //控制自身转向角色的函数
    void Rotato()
    {
        mesh.LookAt(new Vector3(player_transform.position.x, m_transform.position.y, player_transform.position.z));
    }
    //控制动画的函数
    void Ani()
    {
        //获取动画控制器的名称与动画状态
        string ani_name = ani.GetLayerName(0);
        stateInfo = ani.GetCurrentAnimatorStateInfo(0);
        //战斗时的动画处理
        if (GameManager.Battle)
        {
            if (stateInfo.fullPathHash == Animator.StringToHash(ani_name + ".Attack") && !ani.IsInTransition(0))
            {
                ani.SetBool("Attack", false);
            }
            if (stateInfo.fullPathHash == Animator.StringToHash(ani_name + ".Damage") && !ani.IsInTransition(0))
            {
                ani.SetBool("Damage", false);
            }
            return;
        }
    }
    //信息预览
    public string Preview
    {
        get
        {
            string text = name;
            if (GetComponent<QuickAttack>())
                text += "\r\n先攻";
            if (GetComponent<QuickAttack>())
                text += "\r\n魔攻";
            return text + "\r\n生命  " + life + "\r\n攻击力  " + atk + "\r\n防御力  " + def + "\r\n估计伤害  " + PreviewDamage;
        }
    }
    //估计伤害
    string PreviewDamage
    {
        get
        {
            //初始化时读取玩家能力
            Player_atk = player.atk;
            Player_def = player.def;
            Player_life = player.life;            
            if (Player_atk <= def)
            {
                return "???";
            }
            else if (Player_def >= atk)
            {
                return "0";
            }
            else
            {
                int SumDamage = 0;
                //伤害算法
                SumDamage = (life - 1) / (Player_atk - def) * (atk - Player_def);
                return "" + SumDamage;
            }
        }
    }
    //此处改成遇敌直接战斗
    public void ReadyBattle()
    {
        if (!GameManager.Battle)
        {
            //禁止角色移动与转向
            player.movable = false;
            //取消锁屏
            Player.Lock = false;
            //从顶部碰撞时
            if (!player.isGroudedForBattle)
            {
                //允许角色掉落
                player.movable = true;
                //删除碰撞体积
                GetComponent<BoxCollider>().enabled = false;
                //调整位置
                StartCoroutine(CritPosition());
                return;
            }
            //不是从顶部碰撞
            else
            {
                //询问是否战斗
                if (GameManager.AskForReadyBattle)
                {
                    //打开询问UI
                    GameManager.Instance.ReadyBattleUI(true);
                    //清空委托
                    GameManager.Instance.ReadyBattleButton_Yes.onClick.RemoveAllListeners();
                    GameManager.Instance.ReadyBattleButton_No.onClick.RemoveAllListeners();
                    //确认战斗按钮的委托
                    GameManager.Instance.ReadyBattleButton_Yes.onClick.AddListener(delegate () {
                        GameManager.Instance.ReadyBattleUI(false);
                        Preparation();
                    });
                    //取消战斗按钮的委托
                    GameManager.Instance.ReadyBattleButton_No.onClick.AddListener(delegate () {
                        GameManager.Instance.ReadyBattleUI(false);
                        player.movable = true;
                        Player.Lock = true;
                    });
                }
                //不询问战斗或者确认时直接战斗
                else
                {
                    Preparation();
                }
            }
        }
    }
    //怪物与角色调整位置的协程
    IEnumerator CritPosition()
    {
        //等待落地
        yield return new WaitUntil(() => player_transform.position.y - transform.position.y < 0.1f);
        //禁止角色移动
        player.movable = false;
        //拉开距离
        while (Vector3.Distance(mesh.position, player_transform.position) <= 0.8f)
        {
            transform.Translate(mesh.InverseTransformDirection(new Vector3( 0, 0,-Time.deltaTime)));
            yield return new WaitForFixedUpdate();
        }  
        Preparation();
        yield break;
    }
}
