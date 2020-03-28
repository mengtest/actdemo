using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ULoginUI : MonoBehaviour {

    private Button btn_start;

	// Use this for initialization
	void Start () {
        btn_start = GameObject.Find("ULoginUI/Button_Start").GetComponent<Button>();
        btn_start.onClick.AddListener(OnClickStart);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnClickStart()
    {
        SceneManager.Instance.ChangeScene("TestScene2");
    }
}
