using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save
{
    public static Save[] Instance = new Save[11];
    public int number;
    public static int SaveTime = 0;
    public static int LoadTime = 0;
    //角色信息
    Vector3 Player_position
    {
        get
        {
            return new Vector3(PlayerPrefs.GetFloat("Player_x" + number), PlayerPrefs.GetFloat("Player_y" + number), PlayerPrefs.GetFloat("Player_z" + number));
        }
        set
        {
            PlayerPrefs.SetFloat("Player_x" + number, value.x);
            PlayerPrefs.SetFloat("Player_y" + number, value.y);
            PlayerPrefs.SetFloat("Player_z" + number, value.z);
        }
    }
    int Player_Life
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Life" + number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Life" + number, value);
        }
    }
    int Player_Atk
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Atk" + number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Atk" + number, value);
        }
    }
    int Player_Def
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Def" + number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Def" + number, value);
        }
    }
    //创建存档
    public Save(int number)
    {
        SaveTime += 1;
        this.number = number;
        Instance[number] = this;
        PlayerPrefs.SetInt("Save" + "_" + number, 1);
        //记录角色信息
        Player_position = Player.Instance.transform.position;
        Player_Life = Player.Instance.life;
        Player_Atk = Player.Instance.atk;
        Player_Def = Player.Instance.def;
        //记录物品
        foreach(Item item in Item.Items)
        {
            PlayerPrefs.SetInt(number + "_" + "Item" + "_" + item.Name, item.count);
        }
        //记录交互者信息
        foreach (ComparableComponent comparableComponent in GameManager.Instance.comparableComponents)
        {
            bool Enable;
            if (comparableComponent.gameObject.GetComponent<Door>())
            {
                Enable = comparableComponent.gameObject.GetComponent<Door>().Active;
            }
            //一次性物品
            else
            {
                Enable = comparableComponent.gameObject.activeSelf;
            }
            PlayerPrefs.SetInt(number + "_" + "ObjectSave" + "_" + comparableComponent.id, Enable ? 1 : 0);;
        }
    }
    public Save()
    {
    }
    //初始化
    public static void Start()
    {
        for(int i = 0; i <= 10; i++)
        {
            Instance[i] = new Save() { number = i };
        }
    }
    //清空存档
    public void Delete()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.CallTipUI("清空所有存档！");
    }
    //读取存档
    void Load()
    {
        LoadTime += 1;
        //读取物品
        foreach(Item item in Item.Items)
        {
            item.count = PlayerPrefs.GetInt(number + "_" + "Item" + "_" + item.Name, item.count);
            if (item.count > 0)
            {
                GameManager.Instance.SetItem(item.Name);
            }
        }
        //读取交互者信息
        List<ComparableComponent> comparableComponents;
        comparableComponents = new List<ComparableComponent>(GameObject.Find("Scene").GetComponentsInChildren<ComparableComponent>());
        foreach (ComparableComponent comparableComponent in comparableComponents)
        {
            bool Enable = (PlayerPrefs.GetInt(number + "_" + "ObjectSave" + "_" + comparableComponent.id) == 1);
            if (comparableComponent.gameObject.GetComponent<Door>())
            {
                comparableComponent.gameObject.SetActive(true);
                if (!Enable)
                {
                    comparableComponent.gameObject.GetComponent<Door>().Open();
                }
            }
            //一次性物品
            else
            {
                comparableComponent.gameObject.SetActive(Enable);
            }
        }
        //读取角色信息
        Player.Instance.Move(Player_position);
        Player.Instance.life = Player_Life;
        Player.Instance.atk = Player_Atk;
        Player.Instance.def = Player_Def;
        GameManager.Instance.SetState();
    }
    public static void Load(int number)
    {
        //检验是否有存档
        if (PlayerPrefs.GetInt("Save" + "_" + number) == 0)
        {
            GameManager.Instance.CallTipUI("读档 " + number + " 失败!");
            return;
        }
        Instance[number].Load();
        GameManager.Instance.CallTipUI("读档 " + number + " 成功!");
    }
}