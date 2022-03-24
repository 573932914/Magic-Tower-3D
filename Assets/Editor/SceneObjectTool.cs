using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class SceneObjectTool : EditorWindow
{
    Transform CurrentObj;
    Transform Scene;
    public Object source;
    Vector3 Position;
    Vector3 Scale;
    Vector3 Rotation;
    float Height = 0;
    bool Error = false;
    //可伸缩的物体的标签
    string[] ScaleTags = new string[] { "Wall" };
    //可移动的物体的标签
    string[] MoveTags = new string[] { "Wall", "Item", "Enemy", "TagLight", "Door" };
    [MenuItem("MagicTowerTools/SetSceneObject")]
    //绘制窗口
    static void Window()
    {
        SceneObjectTool window = CreateInstance<SceneObjectTool>();
        window.Show();
    }
    void OnGUI()
    {
        //设置字体
        GUIStyle style1 = new GUIStyle();
        style1.fontSize = 15;
        style1.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        GUIStyle style2 = new GUIStyle();
        style2.fontSize = 13;
        style2.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        //垂直绘制
        GUILayout.BeginHorizontal();
        {
            source = EditorGUILayout.ObjectField(source, typeof(UnityEngine.Object), true);
            //文字信息的绘制
            EditorGUILayout.LabelField("▼TransformDirection", style1);
            //按钮的绘制
            //z
            if (GUI.Button(new Rect(100, 40, 25, 25), "↑"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        Forward();
                }
            }
            //-z
            if (GUI.Button(new Rect(100, 100, 25, 25), "↓"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        Back();
                }
            }
            //-x
            if (GUI.Button(new Rect(70, 70, 25, 25), "←"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        Left();
                }
            }
            //x
            if (GUI.Button(new Rect(130, 70, 25, 25), "→"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        Right();
                }
            }
            //y
            if (GUI.Button(new Rect(10, 70, 50, 25), "升高"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        High();
                }
            }
            //-y
            if (GUI.Button(new Rect(10, 100, 50, 25), "降低"))
            {
                if (Selection.activeGameObject)
                {
                    PositionTranslate();
                    if (!Error)
                        Low();
                }
            }
            //变大
            if (GUI.Button(new Rect(165, 70, 50, 25), "增高"))
            {
                if (Selection.activeGameObject)
                {
                    WallScale();
                    if (!Error)
                        Big();
                }
            }
            //变小
            if (GUI.Button(new Rect(165, 100, 50, 25), "矮化"))
            {
                if (Selection.activeGameObject)
                {
                    WallScale();
                    if (!Error)
                        Small();
                }
            }
            //旋转   
            if (GUI.Button(new Rect(100, 70, 25, 25), "卍"))
            {
                if (Selection.activeGameObject)
                {
                    Revolve();
                }
            }
            //创建
            if (GUI.Button(new Rect(10, 40, 50, 25), "创建"))
            {
                Create();
            }
            //删除
            if (GUI.Button(new Rect(165, 40, 50, 25), "删除"))
            {
                Delete();
            }
            //临时功能的实现
            if (GUI.Button(new Rect(165, 150, 50, 25), "调整"))
            {
                Adjust();
            }
        }
        GUILayout.EndHorizontal();
        AssetDatabase.Refresh();
    }
    //判断物体在标签内否
    bool IsInTags(string[] Tags)
    {
        if (!CurrentObj)
        {
            EditorGUILayout.HelpBox("请选择物体", MessageType.Warning);
            Error = true;
            return false;
        }
        CurrentObj = Selection.activeTransform;
        foreach (string tag in Tags)
        {
            if (CurrentObj.CompareTag(tag))
            {
                Error = false;
                return true;
            }
        }
        EditorGUILayout.HelpBox("选择了错误的物体", MessageType.Warning);
        Error = true;
        return false;
    }
    //设置父物体
    void SetParent()
    {
        Scene = GameObject.Find("Scene").transform;
        CurrentObj.SetParent(Scene);
    }
    //创建物体
    void Create()
    {
        if (IsInTags(MoveTags))
        {
            string Name = CurrentObj.name;
            CurrentObj = Selection.activeTransform = (Transform)PrefabUtility.InstantiatePrefab(CurrentObj);
            SetParent();
        }
    }
    //删除物体
    void Delete()
    {
        CurrentObj = Selection.activeTransform;
        if (IsInTags(MoveTags))
        {
            DestroyImmediate(CurrentObj.gameObject);
        }
    }
    //墙体位置转换提前工作
    void PositionTranslate()
    {
        CurrentObj = Selection.activeTransform;
        if (IsInTags(MoveTags))
        {
            SetHeight();
            SetParent();
            Position = new Vector3(Mathf.Ceil(CurrentObj.position.x), Mathf.Ceil((CurrentObj.position.y - Height) * 2) / 2, Mathf.Ceil(CurrentObj.position.z));
        }
    }
    //修正高度
    void SetHeight()
    {
        if (CurrentObj.GetComponent<Door>())
            Height = CurrentObj.GetComponent<Door>().height;
        if (CurrentObj.GetComponent<Item>())
            Height = CurrentObj.GetComponent<Item>().height;
        if (CurrentObj.GetComponent<Enemy>())
            Height = CurrentObj.GetComponent<Enemy>().height;
        if (CurrentObj.GetComponent<Jewel>())
            Height = CurrentObj.GetComponent<Jewel>().height;
        if (CurrentObj.GetComponent<Wall>())
            Height = 0f;
    }
    //调整墙体高度提前工作
    void WallScale()
    {
        CurrentObj = Selection.activeTransform;
        if (IsInTags(ScaleTags))
        {
            SetParent();
        }
        Scale = CurrentObj.localScale;
        SetHeight();
        Position = new Vector3(Mathf.Ceil(CurrentObj.position.x), Mathf.Ceil((CurrentObj.position.y - Height) * 4) / 4, Mathf.Ceil(CurrentObj.position.z));
    }
    //旋转
    void Revolve()
    {
        CurrentObj.Rotate(0, 90, 0);
    }
    void Forward()
    {
        Position.z++;
        Position.y += Height;
        CurrentObj.position = Position;
    }
    void Left()
    {
        Position.x--;
        Position.y += Height;
        CurrentObj.position = Position;
    }
    void Back()
    {
        Position.z--;
        Position.y += Height;
        CurrentObj.position = Position;
    }
    void Right()
    {
        Position.x++;
        Position.y += Height;
        CurrentObj.position = Position;
    }
    void High()
    {
        Position.y += 0.5f;
        Position.y += Height;
        CurrentObj.position = Position;
    }
    void Low()
    {
        Position.y -= 0.5f - Height;
        CurrentObj.position = Position;
    }
    void Big()
    {
        Scale.y += 0.5f;
        Position.y += 0.25f;
        Position.y += Height;
        CurrentObj.position = Position;
        CurrentObj.localScale = Scale;
        CurrentObj.GetComponent<Wall>().SetPlane();
    }
    void Small()
    {
        if (Scale.y < 0.5f)
            return;
        Scale.y -= 0.5f;
        Position.y -= 0.25f;
        Position.y += Height;
        CurrentObj.position = Position;
        CurrentObj.localScale = Scale;
        CurrentObj.GetComponent<Wall>().SetPlane();
    }
    //生成
    void ChangeEnemyName()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            switch (enemy.name)
            {
                case "Blue Slime":
                    enemy.name = "蓝史莱姆";
                    break;
                case "Green Slime":
                    enemy.name = "绿史莱姆";
                    break;
                case "Brown Ghost":
                    enemy.name = "普通鬼魂";
                    break;
                case "Green Rabbit":
                    enemy.name = "绿兔子";
                    break;
                case "Cyan Rabbit":
                    enemy.name = "青兔子";
                    break;
                case "Green Bat":
                    enemy.name = "绿蝙蝠";
                    break;
            }
        }
    }
    //全部调整墙壁皮肤
    void Adjust()
    {
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Wall"))
        {
            if (gameObject.GetComponent<Wall>())
            {
                gameObject.GetComponent<Wall>().SetPlane();
            }
        }
    }
    //临时
    void SetWallMaterial()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (wall.GetComponent<Wall>())
            {
                wall.GetComponent<Wall>().SetPlane();
            }
        }
    }
}
