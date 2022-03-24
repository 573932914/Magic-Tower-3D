using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    //物体碰撞时委托
    public delegate void Triger(GameObject Obj);
    public Triger trigerEvent;
    Transform father;
    private void Start()
    {
        //获取最高级父物体信息
        father = transform;
        do
        {
            father = father.parent;
        } while (!father.CompareTag("Scene"));
    }
    private void OnTriggerEnter(Collider other)
    {
        trigerEvent.Invoke(other.gameObject);
        return;
    }
    public static void AddChildCollider(Transform Obj,Triger triger)
    {
        //为子物体添加碰撞器类
        foreach (Collider collider in Obj.GetComponentsInChildren<Collider>())
        {
            ChildCollider childCollider = collider.gameObject.AddComponent<ChildCollider>();
            //注册碰撞事件
            childCollider.trigerEvent = new Triger(triger);
        }
    }
}
