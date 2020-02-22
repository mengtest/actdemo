using UnityEngine;
using System.Collections;
using Fm_ClientNet;
using Fm_ClientNet.Interface;
using SysUtils;
using System.Collections.Generic;

/// <summary>
/// 游戏主入口
/// </summary>
public class GameMain : MonoBehaviour
{
    void Awake()
    {
        // 添加UI管理器
        //       GameObject.Find("UI Root/Camera").AddComponent<UIManager>();
//        AddComponent("UIManager");

        // 计时器
        AddComponent("TimerManager");

        // 平台管理器
        AddComponent("PlatformManager");

        // 游戏控制
        AddComponent("Game");

        // 场景管理器
        AddComponent("SceneManager");

        // 提示管理器
        AddComponent("PromptManager");

        // 本地存储管理器
        AddComponent("RestoreManager");

        // 等待界面管理器
    //    AddComponent("WaitingManager");

        // 对象池
        AddComponent("GameObjectManager");

        // 主角控制管理器
        AddComponent("RoleControllerManager");

        // 战斗管理器
        AddComponent("FightManager");
        
        // 相机动画控制器
        AddComponent("RoleCameraManager");

        // 初始化日志系统
        LogSystem.Init("Log.txt", false, true);
    }

    void Start()
    {
 //       UIManager.Instance.OpenUI("LoadingUI");

        XmlManager.Instance.LoadAllConfig();


        // 音效管理器
        AddComponent("AudioManager");

        // 特效管理器
        AddComponent("EffectManager");
    }

    void AddComponent(string component)
    {
        if (string.IsNullOrEmpty(component))
        {
            LogSystem.LogError("component name is null");

            return;
        }

        GameObject go = GameObject.Find(component);
        if (go == null)
        {
            go = new GameObject(component);
        }

        go.AddComponent(System.Type.GetType(component));
        go.transform.parent = transform;
    }

    void OnApplicationQuit()
    {
        if (Game.Instance.mClientNet != null)
        {
            Game.Instance.mClientNet.ShutDown();
            Game.Instance.mClientNet.UnInit();
            Game.Instance.mClientNet = null;
        }
    }

    void OnApplicationPause(bool isPause)
    {

    }
}