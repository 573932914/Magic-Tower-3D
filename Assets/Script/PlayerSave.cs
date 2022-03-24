using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Old_PlayerSave
{
    Old_Save save;
    public Old_PlayerSave(Old_Save _save)
    {
        save = _save;
    }
    public string SceneName
    {
        get
        {
            return PlayerPrefs.GetString("Player_Scene" + save.number);
        }
        set
        {
            PlayerPrefs.SetString("Player_Scene" + save.number, value);
        }
    }
    Vector3 Player_position
    {
        get
        {
            return new Vector3(PlayerPrefs.GetFloat("Player_x" + save.number), PlayerPrefs.GetFloat("Player_y" + save.number), PlayerPrefs.GetFloat("Player_z" + save.number));
        }
        set
        {
            PlayerPrefs.SetFloat("Player_x" + save.number, value.x);
            PlayerPrefs.SetFloat("Player_y" + save.number, value.y);
            PlayerPrefs.SetFloat("Player_z" + save.number, value.z);
        }
    }
    int Player_Life
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Life" + save.number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Life" + save.number, value);
        }
    }
    int Player_Atk
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Atk" + save.number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Atk" + save.number, value);
        }
    }
    int Player_Def
    {
        get
        {
            return PlayerPrefs.GetInt("Player_Def" + save.number);
        }
        set
        {
            PlayerPrefs.SetInt("Player_Def" + save.number, value);
        }
    }
    public void Save()
    {
        SceneName = SceneManager.GetActiveScene().name;
        Player_position = Player.Instance.transform.position;
        Player_Life = Player.Instance.life;
        Player_Atk = Player.Instance.atk;
        Player_Def = Player.Instance.def;
    }
    public void Load()
    {       
        Player.Instance.transform.position = Player_position;
        Player.Instance.life = Player_Life;
        Player.Instance.atk = Player_Atk;
        Player.Instance.def = Player_Def;
    }
    public static void CopyPlayerSave(Old_PlayerSave originPlayerSave, Old_PlayerSave copyPlayerSave)
    {
        copyPlayerSave.SceneName = originPlayerSave.SceneName;
        copyPlayerSave.Player_position = originPlayerSave.Player_position;
        copyPlayerSave.Player_Life = originPlayerSave.Player_Life;
        copyPlayerSave.Player_Atk = originPlayerSave.Player_Atk;
        copyPlayerSave.Player_Def = originPlayerSave.Player_Def;
        Debug.Log(copyPlayerSave.Player_Life);
    }
    public void Delete()
    {
        PlayerPrefs.DeleteKey("Player_Scene" + save.number);
        PlayerPrefs.DeleteKey("Player_x" + save.number);
        PlayerPrefs.DeleteKey("Player_y" + save.number);
        PlayerPrefs.DeleteKey("Player_z" + save.number);
        PlayerPrefs.DeleteKey("Player_Life" + save.number);
        PlayerPrefs.DeleteKey("Player_Atk" + save.number);
        PlayerPrefs.DeleteKey("Player_Def" + save.number);
    }
}
