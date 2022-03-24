using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Old_SceneSave
{
    public Old_Save save;
    public Scene scene;
    public List<ObjectSave> objectSaves;

    public Old_SceneSave(Old_Save save, Scene scene)
    {
        this.scene = scene;
        this.save = save;
        objectSaves = new List<ObjectSave>();
        save.sceneSaves.Add(this);
    }
    //仅能存储当前场景
    public void Save()
    {
        objectSaves = new List<ObjectSave>();
        foreach(ComparableComponent comparableComponent in GameObject.Find("Scene").GetComponentsInChildren<ComparableComponent>())
        {
            _ = new ObjectSave(this, comparableComponent.id, comparableComponent.gameObject);
        }
    }
    public void Load()
    {
        foreach(ObjectSave objectSave in objectSaves)
        {
            objectSave.Load();
        }
    }
    public static void CopySceneSave(Old_SceneSave originSceneSave, Old_SceneSave copySceneSave)
    {
        foreach(ObjectSave objectSave in originSceneSave.objectSaves)
        {
            ObjectSave _objectSave = new ObjectSave(copySceneSave, objectSave.id, objectSave.gameObject);
            ObjectSave.CopySceneSave(objectSave, _objectSave);
        }
    }
    public void Delete()
    {
        foreach (ObjectSave objectSave in objectSaves)
        {
            objectSave.Delete();
        }
        objectSaves = new List<ObjectSave>();
    }
}
