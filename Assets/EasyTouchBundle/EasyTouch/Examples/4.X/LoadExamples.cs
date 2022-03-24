using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadExamples : MonoBehaviour {

	public void LoadExample(string level){
#if UNITY_2019
        SceneManager.LoadScene(level);
#else
		Application.LoadLevel( level );
#endif
	}
}
