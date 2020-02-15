using System;
using System.Collections.Generic;
using SysUtils;

using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameObj : IGameObj
    {
        private string mIdent;
        private ObjectID objId;
        private uint mHash;
        private DictionaryEx<string, GameRecord> mRecordSet = null;
        private Dictionary<string, GameProperty> mPropSet = null;


        public GameObj()
        {
            mPropSet = new Dictionary<string, GameProperty>();

            mRecordSet = new DictionaryEx<string, GameRecord>();

        }
        public void onRemoveObject()
        {
            for (int i = 0; i < mRecordSet.mList.Count; i++) 
            {
                string strKey = mRecordSet.mList[i];
                GameRecord gamerecord = mRecordSet[strKey];
                if (gamerecord != null)
                {
                    gamerecord.ClearRow();
                }
            }
        }
        public GameRecord GetGameRecordByName(string name)
        {
            try
            {
                if (name == null || name.Length == 0)
                {
                    return null;
                }

                if (!mRecordSet.ContainsKey(name))
                {
                    return null;
                }
                return mRecordSet[name];
            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return null;
            }
        }

        public bool UpdateProperty(ref string name, Var val)
        {
            try
            {
                if (mPropSet == null)
                {
                    return false;
                }
                GameProperty gameProp = GameProperty.zero;
                gameProp.setPropValue(val);
                if (!mPropSet.ContainsKey(name))
                {
                    mPropSet.Add(name, gameProp);
                }
                else
                {
                    mPropSet[name] = gameProp;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameObject.UpdateProperty Exception:", ex.ToString());
                return false;
            }
            return true;
        }

        public int GetRecordCols(string name)
        {
            int nCols = 0;
            if (name == null || name.Length == 0)
            {
                LogSystem.Log("Error:GameObject.GetRecordCols name para is emtpy");
                return nCols;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    LogSystem.Log("Error:GameObject.GetRecordCols does not exist record:", name);
                    return nCols;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    nCols = record.GetColcount();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.GetRecordCols Exception :", ex.ToString());
                return nCols;
            }
            return nCols;
        }

        public int GetRecordRows(string name)
        {
            int nRows = 0;
            if (name == null || name.Length == 0)
            {
                LogSystem.Log("Error:GameObject.GetRecordRows name para is emtpy");
                return nRows;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    LogSystem.Log("Error:GetRecordRows does not exist record:", name);
                    return nRows;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    nRows = record.GetRowCount();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.GetRecordRows Exception :", ex.ToString());
                return nRows;
            }
            return nRows;
        }

        public bool FindRecord(string name)
        {
            if (name == null || name.Length == 0)
            {
                return false;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameObject.FindRecord Exception :", ex.ToString());
                return false;
            }
            return false;
        }

        public int GetPropType(string name)
        {
            int type = 0;
            if (name == null || name.Length == 0)
            {
                LogSystem.Log("Error:GameObject.GetPropType name para is emtpy");
                return type;
            }

            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    LogSystem.Log("Error:GameObject.GetPropType does not exist property:", name);
                    return type;
                }
                GameProperty property = mPropSet[name];
                 type = property.ValueType;
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.GetPropType Exception :", ex.ToString());
                return type;
            }
            return type;
        }

        public bool FindProp(string name)
        {
            if (name == null || name.Length == 0)
            {
                LogSystem.Log("Error:GameObject.FindProp name para is emtpy");
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    LogSystem.Log("Error:GameObject.FindProp does not exist record:", name);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.FindRecord Exception :", ex.ToString());
                return false;
            }
        }

        public int GetRecordColType(string name, int col)
        {
            int nColType = 0;
            if (name == null || name.Length == 0)
            {
                LogSystem.Log("Error:GameObject.GetRecordColType name para is emtpy");
                return nColType;
            }

            if (col < 0)
            {
                LogSystem.Log("Error:GameObject.GetRecordColType col para must  > 0");
                return nColType;
            }

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    LogSystem.Log("Error:GameObject.GetRecordColType does not have rercord :", name);
                    return nColType;
                }

                GameRecord record = mRecordSet[name];
                if (null == record)
                {
                    LogSystem.Log("Error:GameObject.GetRecordColType GameRecord is null");
                    return nColType;
                }

                if (col >= record.GetColcount())
                {
                    LogSystem.Log("Error:GameObject.GetRecordColType GameRecord col para is too larger");
                    return nColType;
                }

                nColType = record.GetColType(col);
                return nColType;
            }
            catch (Exception ex)
            {
                LogSystem.Log("GameObject.GetRecordColType Exception :", ex.ToString());
                return 0;
            }
        }

        public int FindRecordRowCI(string name, int col, Var key, int begRow)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return -1;
                }
                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return -1;
                }

                return record.FindRow(col, key, begRow, true);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.FindRecordRowCI Exception :", ex.ToString());
                return -1;
            }
        }

        public int FindRecordRow(string name, int col, Var key, int begRow)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return -1;
                }
                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return -1;
                }

                return record.FindRow(col, key, begRow, false);
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.FindRecordRow Exception :", ex.ToString());
                return -1;
            }
        }

        public Var QueryRecord(string name, int row, int col)
        {
            Var result = Var.zero;

            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return result;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return result;
                }

                if (!record.GetValue(row, col, ref result))
                {
                    LogSystem.Log("Error,GameObject.QueryRecord GetValue Failed");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }

            return result;
        }

        public bool QueryRecordBool(string name, int row, int col, ref bool bResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                bResult = result.GetBool();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordInt(string name, int row, int col, ref int iResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                iResult = result.GetInt();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordInt64(string name, int row, int col, ref long lResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result  = Var.zero;
                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                lResult = result.GetInt64();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordFloat(string name, int row, int col, ref float fResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                fResult = result.GetFloat();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordDouble(string name, int row, int col, ref double dResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                dResult = result.GetDouble();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordString(string name, int row, int col, ref string strResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                strResult = result.GetString();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public bool QueryRecordStringW(string name, int row, int col, ref string strResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result  = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                strResult = result.GetWideStr();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }

        public bool QueryRecordObject(string name, int row, int col, ref ObjectID oResult)
        {
            try
            {
                if (!mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                GameRecord record = mRecordSet[name];
                if (record == null)
                {
                    return false;
                }
                Var result = Var.zero;

                if (!record.GetValue(row, col, ref result))
                {
                    return false;
                }
                oResult = result.GetObject();
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error:GameObject.QueryRecord Exception :", ex.ToString());
            }
            return true;
        }
        public Var QueryProp(string name)
        {
            try
            {
                if (!mPropSet.ContainsKey(name))
                {
                    return Var.zero;
                }
                GameProperty prop = mPropSet[name];
             
                //防止值被逻辑重写，此处拷贝复制
                return prop.PropValue.Clone();
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return Var.zero;
            }
        }

        public bool QueryPropBool(string strPropName, ref bool bResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];
                Var propValue = prop.getPropValue();
             
                if (propValue.Type != VarType.Bool)
                    return false;

                bResult = propValue.GetBool();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropInt(string strPropName, ref int iResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();
              
                if (propValue.Type != VarType.Int)
                    return false;

                iResult = propValue.GetInt();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropInt64(string strPropName, ref long lResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();
            
                if (propValue.Type != VarType.Int64)
                    return false;

                lResult = propValue.GetInt64();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropFloat(string strPropName, ref float fResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();
              
                if (propValue.Type != VarType.Float)
                    return false;

                fResult = propValue.GetFloat();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropDouble(string strPropName, ref double dResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();
              
                if (propValue.Type != VarType.Double)
                    return false;

                dResult = propValue.GetDouble();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropString(string strPropName, ref string strResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();
              
                if (propValue.Type != VarType.String)
                    return false;

                strResult = propValue.GetString();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropStringW(string strPropName, ref string strResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();

                if (propValue.Type != VarType.WideStr)
                    return false;

                strResult = propValue.GetWideStr();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public bool QueryPropObject(string strPropName, ref ObjectID oResult)
        {
            if (strPropName == null)
            {
                return false;
            }

            try
            {
                if (!mPropSet.ContainsKey(strPropName))
                {
                    return false;
                }

                GameProperty prop = mPropSet[strPropName];

                Var propValue = prop.getPropValue();

                if (propValue.Type != VarType.Object)
                    return false;

                oResult = propValue.GetObject();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError("Error, exception ", ex.ToString());
                return false;
            }
        }
        public void GetPropList(ref VarList args, ref VarList result)
        {
            foreach (KeyValuePair<string, GameProperty> kvp in mPropSet)
            {
                string name = kvp.Key;
                if ((name != null) &&
                    (name.Length != 0))
                {
                    result.AddString(name);
                }
            }
        }

        public void GetRecordList(ref VarList args, ref VarList result)
        {
      
            foreach (KeyValuePair<string, GameRecord> kvp in mRecordSet)
            {
                string name = kvp.Key;
                if ((name != null) &&
                    (name.Length != 0))
                {
                    result.AddString(name);
                }
            }
        }

        public string Ident
        {
            get { return this.mIdent; }
        }

        public string GetIdent()
        {
            return this.mIdent;
        }

        public void SetIdent(string value)
        {
            if (value != null && value.Length != 0)
            {
                this.mIdent = value;
            }
        }

        public void SetHash(uint value)
        {
            this.mHash = value;
        }

        public uint GetHash()
        {
            return mHash;
        }

        public ObjectID GetObjId()
        {
            return this.objId;
        }

        public void SetObjId(ObjectID id)
        {
            this.objId = id;
        }

        public bool AddRecord2Set(string name, ref GameRecord record)
        {
            try
            {
                if (name == null || name.Length == 0)
                {
                    LogSystem.Log("name is null");
                    return false;
                }

                if (record == null)
                {
                    LogSystem.Log("record is null");
                    return false;
                }

                if (mRecordSet.ContainsKey(name))
                {
                    return false;
                }

                mRecordSet.Add(name, record);
                //test begin
                //test end

            }
            catch (Exception ex)
            {
                LogSystem.Log("Exception:" , ex.ToString());
                return false;
            }
            return true;
        }
    }
}



