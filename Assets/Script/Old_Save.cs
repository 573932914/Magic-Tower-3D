using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Old_Save
{
    public static Old_Save[] Instance;
    public int number;
    public Old_PlayerSave playerSave;
    public List<Old_SceneSave> sceneSaves;
    public Old_Save(int Number)
    {
        number = Number;
        sceneSaves = new List<Old_SceneSave>();
        playerSave = new Old_PlayerSave(Instance[Number]);
        for (int i = 0; i <= 1; i++)
        {
            _ = new Old_SceneSave(this, SceneManager.GetSceneByBuildIndex(i));
        }
    }
    public void SetPlayerSave()
    {
        playerSave.Save();
    }
    public bool SaveActived
    {
        get
        {
            return PlayerPrefs.GetInt("Save" + number) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Save" + number, value ? 1 : 0);
        }
    }
    public static void Load(int loadSaveNumber)
    {
        SceneManager.LoadScene(Instance[loadSaveNumber].playerSave.SceneName);
        Instance[loadSaveNumber].playerSave.Load();
        CopySave(0, loadSaveNumber);
    }
    public static void CopySave(int OriginSaveNumber, int CopySaveNumber)
    {
        Debug.Log("记录角色");
        Old_PlayerSave.CopyPlayerSave(Instance[OriginSaveNumber].playerSave, Instance[CopySaveNumber].playerSave);
        Instance[CopySaveNumber].Delete();
        foreach (Old_SceneSave sceneSave in Instance[OriginSaveNumber].sceneSaves)
        {
            Old_SceneSave _sceneSave = new Old_SceneSave(Instance[CopySaveNumber], sceneSave.scene);
            Old_SceneSave.CopySceneSave(sceneSave, _sceneSave);
        }
    }
    public static void Start()
    {
        Instance = new Old_Save[11];
        for (int i = 0; i <= 10; i++)
        {
            Instance[i] = new Old_Save(i);
        }
    }
    public void SaveScene(Scene scene)
    {
        foreach (Old_SceneSave sceneSave in sceneSaves)
        {
            if (sceneSave.scene == scene)
            {
                sceneSave.Save();
            }
        }
    }
    public void LoadScene(Scene scene)
    {
        foreach (Old_SceneSave sceneSave in sceneSaves)
        {
            if (sceneSave.scene == scene)
            {
                sceneSave.Load();
            }
        }
    }
    public void Delete()
    {
        playerSave.Delete();
        foreach (Old_SceneSave sceneSave in sceneSaves)
        {
            sceneSave.Delete();
        }
        sceneSaves = new List<Old_SceneSave>();
        SaveActived = false;
    }
}
