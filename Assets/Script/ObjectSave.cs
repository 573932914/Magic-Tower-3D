using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSave
{
    Old_SceneSave sceneSave;
    public GameObject gameObject;
    public int id;
    ObjectSave()
    {

    }
    public ObjectSave(Old_SceneSave scene, int objID, GameObject obj)
    {
        Debug.Log(id + "   物品存档");
        sceneSave = scene;
        id = objID;
        gameObject = obj;
        sceneSave.objectSaves.Add(this);
        //门
        SaveObject();
    }
    void SaveObject()
    {
        if (gameObject.GetComponent<Door>())
        {
            Enable = gameObject.GetComponent<Door>().Active;
        }
        //一次性物品
        else
        {
            Enable = gameObject.activeSelf;
        }
    }
    public bool Enable
    {
        get
        {
            if (PlayerPrefs.GetInt(sceneSave.save.number + "_" + sceneSave.scene.name + "_" + "ObjectSave" + "_" + id) == 0)
            {
                return false;
            }
            return true;
        }
        set
        {
            PlayerPrefs.SetInt(sceneSave.save.number + "_" + sceneSave.scene.name + "_" + "ObjectSave" + "_" + id, value ? 1 : 0);
        }
    }
    public void Load()
    {
        Debug.Log(id + "   物品读档");
        //门
        if (gameObject.GetComponent<Door>())
        {
            if (!Enable)
            {
                gameObject.GetComponent<Door>().Open();
            }
        }
        //一次性物品
        else
        {
            gameObject.SetActive(Enable);
        }
    }
    public static void CopySceneSave(ObjectSave originObjectSave, ObjectSave copyObjectSave)
    {
        copyObjectSave.Enable = originObjectSave.Enable;
    }
    public void Delete()
    {
        PlayerPrefs.DeleteKey(sceneSave.save.number + "_" + sceneSave.scene.name + "_" + "ObjectSave" + "_" + id);
    }
}
