using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Jewel : MonoBehaviour
{
    //能力类型
    public enum Ability
    {
        Life,
        Atk,
        Def,
    }
    //基础信息
    public Ability AbilityType;
    public int Power;
    public float height;
    public AudioClip Clip;
    //初始化
    private void Start()
    {
        //给子物体添加碰撞函数
        ChildCollider.AddChildCollider(transform, Trigger);
    }
    //增加能力
    public void AddAbility()
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();
        switch (AbilityType)
        {
            case Ability.Atk:
                player.atk += Power;
                break;
            case Ability.Def:
                player.def += Power;
                break;
            case Ability.Life:
                player.life += Power;
                break;
            default:
                break;
        }
        GameManager.Instance.SetState();
    }
    //接触玩家时触发效果，若要保存其，请记得将之父类设为Item
    public void Trigger(GameObject other)
    {
        if (other.CompareTag("Player")&&gameObject.activeSelf)
        {
            AudioManager.Instance.SePlay(transform.position, Clip);
            AddAbility();
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Trigger(other.gameObject);
    }
}
