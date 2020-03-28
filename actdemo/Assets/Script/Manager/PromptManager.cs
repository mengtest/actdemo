using UnityEngine;
using System.Collections;

public class PromptManager : MonoBehaviour
{
    private static PromptManager _Instance = null;
    public static PromptManager Instance
    {
        get { return _Instance; }
    }

    void Awake()
    {
        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ShowPromptUI(string content)
    {
        Transform trans = UIManager.Instance.OpenUI("PromptUI");
        if (trans == null)
        {
            return;
        }

        //PromptUI ui = trans.GetComponent<PromptUI>();
        //if (ui == null)
        //{
        //    return;
        //}

        //ui.SetContent(content);
    }
}