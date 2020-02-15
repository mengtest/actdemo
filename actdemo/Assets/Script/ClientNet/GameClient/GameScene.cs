using SysUtils;

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameScene : GameObj, IGameScene
    {
        private GameObjectSet m_Objects;
        public GameScene()
        {
            m_Objects = new GameObjectSet();
        }
        public void Clear()
        {
            m_Objects.Clear();
        }
        public GameSceneObj AddSceneObj(string object_ident)
        {
            GameSceneObj sceneObj = new GameSceneObj();
            m_Objects.Add(object_ident, ref sceneObj);
            return sceneObj;
        }

        public bool RemoveSceneObj(string object_ident)
        {

            return m_Objects.Remove(object_ident);
        }

        public GameSceneObj GetSceneObjByIdent(string object_ident)
        {
            return (GameSceneObj)m_Objects.GetObjectByIdent(object_ident);
        }

        //是否存在问题
        public IGameSceneObj GetSceneObj(string object_ident)
        {
            return (IGameSceneObj)m_Objects.GetObjectByIdent(object_ident);
        }

        public int GetSceneObjCount()
        {
            return m_Objects.GetObjectCount();
        }

        public void GetSceneObjList(ref VarList args, ref VarList result)
        {
            m_Objects.GetObjectList(ref result);
            return;
        }


    }
}



