using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Reset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
