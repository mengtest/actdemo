using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESCUI : MonoBehaviour {
    public Button exit;
    public Button ret;
    private Vector3 mid= new Vector3(175.4f, -419.45f, 0f);
    private Vector3 outside = new Vector3(1000f, 1000f, 0f);
    RectTransform recttrans;

    private void Start()
    {
        recttrans = gameObject.GetComponent<RectTransform>();
        
        exit.onClick.AddListener(OnClickExit);
        ret.onClick.AddListener(OnClickRet);
    }

    public void OnClickExit () {
        Application.Quit();
        //Debug.Log("exit");
	}
	
	public void OnClickRet () {
        //Debug.Log("ret");
        recttrans.transform.localPosition = outside;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            recttrans.transform.localPosition = mid;
        }
    }
}
