//--------------------------------------------------------------------
// 文件名	:	GameClient.cs
// 内  容	:
// 说  明	:   客户端数据管理
// 创建日期	:	2014年06月10日
// 创建人	:	丁有进
// 版权所有	:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------


using SysUtils;
using System;

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameClient : IGameClient
    {
        private GameScene mScene = null;
        private GameObjectSet mViews = null;
        private string mStrPlayerIdent = null;

        public GameClient()
        {
            mScene = null;
            mViews = new GameObjectSet();
        }

        public void SetPlayerIdent(string value)
        {
            this.mStrPlayerIdent = value;
        }

        public string GetPlayerIdent()
        {
            return this.mStrPlayerIdent;
        }

        public GameScene CreateScene(string player_ident)
        {
            if (mScene != null)
            {
                mScene.Clear();
            }
            mScene = new GameScene();
            mStrPlayerIdent = player_ident;
            return mScene;
        }

        public bool DeleteScene()
        {
            mScene = null;
            return true;
        }

        public IGameScene GetCurrentScene()
        {
            return mScene;
        }

        public IGameSceneObj GetCurrentPlayer()
        {
            if (null == mScene)
            {
                return null;
            }
            return mScene.GetSceneObjByIdent(mStrPlayerIdent);
        }

        public GameView CreateView(string view_ident, int capacity)
        {
            GameView view = new GameView();
            view.SetCapaticty(capacity);
            mViews.Add(view_ident, ref view);
            return view;
        }

        public bool DeleteView(string view_ident)
        {
            return mViews.Remove(view_ident);
        }

        public GameView GetViewByIdent(string view_ident)
        {
            return (GameView)mViews.GetObjectByIdent(view_ident);
        }

        public bool IsPlayer(string ident)
        {
            if (null == mScene)
            {
                return false;
            }

            return (ident == mStrPlayerIdent);
        }

        public IGameSceneObj GetSceneObj(string ident)
        {
            if (null == mScene)
            {
                return null;
            }

            GameSceneObj obj = mScene.GetSceneObjByIdent(ident);
            if (null == obj)
            {
                return null;
            }

            return obj;
        }


        public IGameView GetView(string ident)
        {
            GameView view = (GameView)mViews.GetObjectByIdent(ident);
            if (null == view)
            {
                return null;
            }

            return view;
        }

        public IGameViewObj GetViewObj(string view_ident, string item_ident)
        {
            GameView view = (GameView)mViews.GetObjectByIdent(view_ident);
            if (null == view)
            {
                return null;
            }

            IGameViewObj obj = view.GetViewObjByIdent(item_ident);
            if (obj == null)
            {
                return null;
            }
            return obj;
        }


        public int GetViewCount()
        {
            return mViews.GetObjectCount();
        }

        public void GetViewList(ref VarList args, ref VarList result)
        {
            mViews.GetObjectList(ref result);
            return;
        }

        public bool ClearAll()
        {
            try
            {
                if (null != mViews)
                {
                    mViews.Clear();
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
            return true;
        }

    }
}

