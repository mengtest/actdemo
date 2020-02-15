using System;
using System.Collections.Generic;
using SysUtils;


namespace Fm_ClientNet
{
    public class GameObjectSet
    {
        private Dictionary<string, GameObj> mObjects = null;

        public GameObjectSet()
        {
            mObjects = new Dictionary<string, GameObj>();
        }

        public bool Find(string ident)
        {
            if (ident == null || ident.Length == 0)
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }

            return true;
        }

        public bool Add(string ident, ref GameObj obj)
        {
            if (ident == null || obj == null
                || ident.Length == 0)
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.Add Exception:" , ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(string ident, ref GameView obj)
        {
            if (ident == null || obj == null
                || ident.Length == 0)
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.Add Exception:" , ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(string ident, ref GameSceneObj obj)
        {
            if (ident == null || obj == null
                || ident.Length == 0)
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.Add Exception:" , ex.ToString());
                return false;
            }
            return true;
        }

        public bool Add(string ident, ref GameViewObj obj)
        {
            if (ident == null || obj == null
                || ident.Length == 0)
            {
                return false;
            }

            try
            {
                obj.SetIdent(ident);
                obj.SetHash(Tools.GetHashValueCase(ident));
                if (mObjects.ContainsKey(ident))
                {
                    return false;
                }

                mObjects.Add(ident, obj);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.Add Exception:" , ex.ToString());
                return false;
            }
            return true;
        }

        public bool Remove(string ident)
        {
            if (ident == null || ident.Length == 0)
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return false;
                }
                
                GameObj obj = mObjects[ident];
                if (obj != null)
                {
                    ///删除对象自身信息
                    obj.onRemoveObject();
                }

                mObjects.Remove(ident);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.Remove Exception:" , ex.ToString());
            }
            return true;
        }

        public bool Change(string old_ident, string new_ident)
        {
            if (old_ident == null || new_ident == null
                || old_ident.Length == 0 || new_ident.Length == 0)
            {
                return false;
            }

            try
            {
                if (!mObjects.ContainsKey(old_ident))
                {
                    return false;
                }

                if (!mObjects.ContainsKey(new_ident))
                {
                    return false;
                }

                GameObj oldObj = mObjects[old_ident];
                GameObj newObj = mObjects[new_ident];
                if (oldObj == null || newObj == null)
                {
                    return false;
                }

                newObj.SetIdent(old_ident);
                newObj.SetHash(Tools.GetHashValueCase(old_ident));

                oldObj.SetIdent(new_ident);
                oldObj.SetHash(Tools.GetHashValueCase(new_ident));
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception " , ex.ToString());
                return false;
            }
            return true;
        }

        public void Clear()
        {
            foreach( GameObj oGameObj in mObjects.Values )
            {
                oGameObj.onRemoveObject();
            }
    
            mObjects.Clear();
        }

        public GameObj GetObjectByIdent(string ident)
        {
            if (ident == null || ident.Length == 0)
            {
                return null;
            }

            try
            {
                if (!mObjects.ContainsKey(ident))
                {
                    return null;
                }
                return mObjects[ident];
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObjectSet.GetObjectByIdent Exception:" , ex.ToString());
                return null;
            }
        }

        public ObjectID GetObjectID(string ident)
        {
            GameObj obj = GetObjectByIdent(ident);
            if (obj == null)
            {
                return ObjectID.zero;
            }

            return obj.GetObjId();
        }

        public int GetObjectCount()
        {
            return this.mObjects.Count;
        }

        public void GetObjectList(ref VarList result)
        {
            if (result == null)
            {
                return;
            }
            foreach (string strKey in mObjects.Keys)
            {
                result.AddString(strKey);
            }

        }

    }
}



