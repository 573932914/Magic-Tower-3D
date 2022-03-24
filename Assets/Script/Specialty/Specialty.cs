using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specialty : MonoBehaviour
{
    public string Name;
    //令该属性失效的物品
    public string InvalidItemName;
    //效果
    public string Effect;
    public bool Active
    {
        get
        {
            if (Item.FindWithName(InvalidItemName))
            {
                GameManager.Instance.Battle_Reporter("玩家拥有" + InvalidItemName + "，怪物的" + Name + "属性无效。");
                return false;
            }
            else
            {
                GameManager.Instance.Battle_Reporter("怪物拥有" + Name + "属性，" + Effect);
                return true;
            }
        }
    }
}
