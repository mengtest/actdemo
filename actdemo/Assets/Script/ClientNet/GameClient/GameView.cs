using SysUtils;

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameView : GameObj, IGameView
    {
        private int m_nCapacity;
        private GameObjectSet m_Objects;
        public GameView()
        {
            m_Objects = new GameObjectSet();
        }

        public void SetCapaticty(int value)
        {
            this.m_nCapacity = value;
        }

        public int GetCapacity()
        {
            return this.m_nCapacity;
        }

        public GameViewObj AddViewObj(string item_ident)
        {
            GameViewObj viewObj = new GameViewObj();
            m_Objects.Add(item_ident, ref viewObj);
            return viewObj;
        }


        public bool RemoveViewObj(string item_ident)
        {
            return m_Objects.Remove(item_ident);
        }

        public bool ChangeViewObj(string old_item_ident, string new_item_ident)
        {
            return m_Objects.Change(old_item_ident, new_item_ident);
        }

        public IGameViewObj GetViewObjByIdent(string item_ident)
        {
            return (IGameViewObj)m_Objects.GetObjectByIdent(item_ident);
        }

        public ObjectID GetViewObj(string item_ident)
        {
            return m_Objects.GetObjectID(item_ident);
        }

        public int GetViewObjCount()
        {
            return m_Objects.GetObjectCount();
        }

        public void GetViewObjList(ref VarList args, ref VarList result)
        {
            m_Objects.GetObjectList(ref result);
            return;
        }


    }

}


