using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //基本信息
    public string m_name;
    public string KeyName;
    public float height;
    public AudioClip Clip;
    Item Key;
    //互动开关
    public bool Active = true;
    //动画控制器
    Animator ani;
    private void Start()
    {
        ani = GetComponent<Animator>();
        Key = Item.FindWithName(KeyName);
    }
    //开门的函数
    public void Open()
    {
        AudioManager.Instance.SePlayOnece(transform, Clip);
        ani.SetBool("Open", true);
        foreach(Transform child in transform.GetComponentsInChildren<Transform>())
        {
            child.tag = "Scene";
        }
        transform.tag = "Scene";
        AbleUseKey = false;
        Active = false;
    }
    //注册/注销钥匙与自身关系
    public bool AbleUseKey
    {
        set
        {
            if (value)
            {
                Key.Effect.AddListener(Open);
            }
            else
            {
                Key.Effect.RemoveListener(Open);
            }
        }
    }
}
