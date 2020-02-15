using UnityEngine;
using System.Collections;

public class ShowUI : MonoBehaviour
{
    private UIEventListener mEnterFightButton;

    void Awake()
    {
        mEnterFightButton = transform.Find("Info/Button").GetComponent<UIEventListener>();
    }

    void AddEvent()
    {
        //mEnterFightButton.OnClick = EnterFight;
    }

    void EnterFight(GameObject go)
    {
        
    }
}