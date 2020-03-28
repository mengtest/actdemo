using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButtonUI : MonoBehaviour {

 //   private GameObject mGame;
    private GameObject mCanvas;
    private Transform[] mChildren;
    private void Awake()
    {
 //       mGame = GameObject.Find("Game");
        mCanvas = GameObject.Find("Canvas");
        mChildren = mCanvas.GetComponentsInChildren<Transform>();
        foreach (Transform child in mChildren)
        {
            if (child.name.Contains("loading"))
            {
                child.gameObject.SetActive(false);
            }
        }
        
        
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        foreach(Transform child in mChildren)
        {

            if (child.name.Contains("Button_Start"))
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }

        SceneManager.Instance.ChangeScene("TestScene2");
        //Debug.Log("Button Clicked. ClickHandler.");
//        gameObject.SetActive(false);
//        mGame.SetActive(true);
    }
}
