using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    //物品基础信息（请保证每个物品的该组件的名字与图像时绝对不同的信息，此外在创建了一个物品预制体后建议将COUNT设为1并保存，并且将其ITEM组件复制到标题界面的ITEM物体下并且改动其COUNT为0（初始拥有数））
    public string Name;
    public int count = 1;
    public Sprite sprite;
    public string explain;
    public float height = 0;
    //物品使用类型
    public enum UseType
    {
        OnceUsable,
        AlwaysUsable,
        MomentoryUsable,
        MomentoryDisusable,
        Disusable
    }
    public UseType useType;
    //音效
    public AudioClip Clip;
    //获得过物品开关
    bool have_get = false;
    //物品使用事件定义
    public UnityEvent Effect;
    //物品索引与拥有表,用于记录物品信息
    public static List<Item> Items = new List<Item>();
    //单个物品初始化
    private void Start()
    {
        //给子物体添加碰撞函数
        ChildCollider.AddChildCollider(transform, Trigger);
    }
    //初始化物品表
    public static void Initial()
    {
        GameObject obj = GameObject.Find("Item");
        Items = new List<Item>(obj.GetComponents<Item>());
        DontDestroyOnLoad(obj);
    }
    //通过名字索引物品
    public static Item FindWithName(string Name)
    {
        foreach(Item item in Items)
        {
            if (item.Name == Name)
            {
                return item;
            }
        }
        Debug.LogError("物品名字不在目录中！");
        return null;
    }
    //通过图片索引物品
    public static Item FindWithSprite(Sprite sprite)
    {
        foreach (Item item in Items)
        {
            if (item.sprite == sprite)
            {
                return item;
            }
        }
        Debug.LogError("物品名字不在目录中！");
        return null;
    }
    //物品的说明文字
    public string Explain
    {
        get
        {
            return (Name + "\r\n" + explain);
        }
    }
    //获得物品时
    public static void Get(string Name, int number)
    {
        Item item = FindWithName(Name);
        //若未取得过物品
        if (!item.have_get)
        {
            FirstGet(Name);
            //打开获得过开关
            item.have_get = true;
        }
        Add(Name, number);
    }
    //增加一定数量的某种类型的物品
    public static void Add(string Name, int number)
    {
        Item item = FindWithName(Name);
        item.count += number;
        GameManager.Instance.SetItem(Name);
    }
    public static void Add(Item item ,int number)
    {
        item.count += number;
        GameManager.Instance.SetItem(item.Name);
    }
    //首次获得物品
    public static void FirstGet(string name)
    {
        GameManager.Instance.CallItemHelpUI(name);
    }
    //获得组件信息上的物品
    public void Get()
    {
        Get(Name, count);
    }
    //绑定物品与使用函数（名字）
    public static void Use(string UseName)
    {
        //遍历背包
        Item item = FindWithName(UseName);
        //确认物品数量足够
        if (item.count > 0)
        {
            //一次性物品
            if (item.useType == UseType.OnceUsable)
            {
                item.count -= 1;
                GameManager.Instance.SetItem(UseName);
            }
            item.Effect.Invoke();
        }
        //无法使用的情况
        return;
    }
    //绑定物品与使用函数（图片）
    public static void Use(Sprite sprite)
    {
        //遍历背包
        Item item = FindWithSprite(sprite);
        //确认物品数量足够
        if (item.count > 0)
        {
            //一次性物品
            if (item.useType == UseType.OnceUsable || item.useType == UseType.MomentoryUsable)
            {
                item.count -= 1;
                GameManager.Instance.SetItem(item.Name);
            }
            item.Effect.Invoke();
        }
        //无法使用的情况
        return;
    }
    //当与角色碰撞时获得
    public void Trigger(GameObject other)
    {
        if (other.CompareTag("Player") && gameObject.activeSelf)
        {
            AudioManager.Instance.SePlay(transform.position, Clip);
            Get();
            gameObject.SetActive(false);
        }
    }
    //检索父物体碰撞的函数
    private void OnTriggerEnter(Collider other)
    {
        Trigger(other.gameObject);
    }
    //允许使用消耗性物品开关（仅适用于非交互状态时不能使用，进入使用范围允许使用的物品）
    public bool MomentoryUsable
    {
        get
        {
            return useType == UseType.MomentoryUsable;
        }
        set
        {
            useType = value ? UseType.MomentoryUsable : UseType.MomentoryDisusable;
            GameManager.Instance.RefreshItemUseAble(Name);
        }
    }
    //快速判断物品能否立即使用
    public bool Useable
    {
        get
        {
            switch (useType)
            {
                case UseType.AlwaysUsable:return true;
                case UseType.MomentoryUsable:return true;
                case UseType.OnceUsable:return true;
                default:return false;
            }
        }
    }
}
