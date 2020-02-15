//--------------------------------------------------------------------
// 文件名:		VarList.cs
// 内  容:		
// 说  明:		
// 创建日期:	2009年7月8日	
// 创建人:		陆利民
// 版权所有:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SysUtils
{
    // 基本游戏数据类型
    public class VarType
    {
        public const int Bool = 1;
        public const int Int = 2;
        public const int Int64 = 3;
        public const int Float = 4;
        public const int Double = 5;
        public const int String = 6;
        public const int WideStr = 7;
        public const int Object = 8;
        public const int Pointer = 9;
        public const int UserData = 10;

        // 是否有效的数据类型
        public static bool IsValid(int type)
        {
            return GetString(type) != string.Empty;
        }

        // 获得类型名称
        public static string GetString(int type)
        {
            if (type == VarType.Bool)
            {
                return "boolean";
            }
            else if (type == VarType.Int)
            {
                return "integer";
            }
            else if (type == VarType.Int64)
            {
                return "int64";
            }
            else if (type == VarType.Float)
            {
                return "float";
            }
            else if (type == VarType.Double)
            {
                return "double";
            }
            else if (type == VarType.String)
            {
                return "string";
            }
            else if (type == VarType.WideStr)
            {
                return "widestr";
            }
            else if (type == VarType.Object)
            {
                return "object";
            }
            else if (type == VarType.Pointer)
            {
                return "pointer";
            }
            else if (type == VarType.UserData)
            {
                return "binary";
            }
            else
            {
                return string.Empty;
            }
        }
    }
    // 可变类型变量
    public struct Var
    {
        public int nType  ;
        public object Data ;
        public int Type
        {
            get { return nType; }
        }
        public static Var zero = new Var(0,null);
        public Var(int iType, object odata)
        {
            nType = iType;
            Data = odata;
        }
        public Var Clone()
        {
            Var var ;
            var.nType = this.nType;
            var.Data = this.Data;
            return var;
        }

        public void Copy(Var var)
        {
            this.nType = var.nType;
            this.Data = var.Data;
        }

        public bool GetBool()
        {
            try
            {
                return (bool)Data;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetInt()
        {
            try
            {
                if (nType == VarType.Int64)
                {
                    return (int)((long)Data);
                }

                return (int)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetInt64()
        {
            try
            {
                if (nType == VarType.Int)
                {
                    return (long)((int)Data);
                }

                return (long)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float GetFloat()
        {
            try
            {
                return (float)Data;
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }

        public double GetDouble()
        {
            try
            {
                return (double)Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetString()
        {
            try
            {
                return Data as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetWideStr()
        {
            try
            {
                return Data as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public ObjectID GetObject()
        {
            if (nType != VarType.Object)
            {
                return ObjectID.zero;
            }

            return ((ObjectID)Data).Clone();
        }

        public byte[] GetUserData()
        {
            if (nType != VarType.UserData)
            {
                return null;
            }

            return (byte[])((byte[])Data).Clone();
        }

        public void SetBool(bool value)
        {
            nType = VarType.Bool;
            Data = value;
        }

        public void SetInt(int value)
        {
            nType = VarType.Int;
            Data = value;
        }

        public void SetInt64(long value)
        {
            nType = VarType.Int64;
            Data = value;
        }

        public void SetFloat(float value)
        {
            nType = VarType.Float;
            Data = value;
        }

        public void SetDouble(double value)
        {
            nType = VarType.Double;
            Data = value;
        }

        public void SetString(string value)
        {
            nType = VarType.String;
            Data = value;
        }

        public void SetWideStr(string value)
        {
            nType = VarType.WideStr;
            Data = value;
        }

        public void SetObject(ObjectID value)
        {
            nType = VarType.Object;
            Data = value.Clone();
        }

        public void SetUserData(byte[] value)
        {
            nType = VarType.UserData;
            Data = value.Clone();
        }
    }

    // 可变类型参数集合
    public class VarList
    {
        static public VarList GetVarList()
        {
            return Aollector.GetObject<VarList>() as VarList;
        }
       
        public void Collect()
        {
            this.Clear();
            Aollector.CollectObject<VarList>(this);
        }
        public struct VarData
        {
            public int nType;
            public object Data;
            public static VarData zero = new VarData(0, null);
            public VarData(int type, object data)
            {
                nType = type;
                Data = data;
            }
            public VarData Clone()
            {
                VarData newData = zero;
                newData.nType = nType;
                switch (nType)
                {
                    case VarType.Object:
                        newData.Data = ((ObjectID)Data).Clone();
                        break;
                    case VarType.UserData:
                        newData.Data = ((byte[])Data).Clone();
                        break;

                    default:
                        newData.Data = Data;
                        break;
                }

                return newData;
            }
        }

        private List<VarData> m_Values = new List<VarData>();

        public VarList()
        {
        }
        ~VarList()
        {
        }
        public void Clear()
        {
            m_Values.Clear();
        }

        public int GetCount()
        {
            return m_Values.Count;
        }

        public int GetType(int index)
        {
			if (index >= m_Values.Count)
				return 0;

            return m_Values[index].nType;
        }

        public bool GetBool(int index)
        {
            try
            {
				if (index >= m_Values.Count)
					return false;

                return (bool)m_Values[index].Data;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetInt(int index)
        {
			if (index >= m_Values.Count)
				return 0;

            int type = m_Values[index].nType;
            if (type == VarType.Int64)
            {
                long value = (long)m_Values[index].Data;

                return (int)value;
            }

            try
            {
                return (int)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public long GetInt64(int index)
        {
			if (index >= m_Values.Count)
				return 0;

            int type = m_Values[index].nType;
            if (type == VarType.Int)
            {
                int value = (int)m_Values[index].Data;

                return (long)value;
            }

            try
            {
                return (long)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float GetFloat(int index)
        {
			if (index >= m_Values.Count)
				return 0.0f;
            try
            {
                return (float)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }

        public double GetDouble(int index)
        {
			if (index >= m_Values.Count)
				return 0;
            try
            {
                return (double)m_Values[index].Data;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetString(int index)
        {
			if (index >= m_Values.Count)
				return string.Empty;
            try
            {
                return m_Values[index].Data as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetWideStr(int index)
        {
			if (index >= m_Values.Count)
				return string.Empty;

            try
            {
                return m_Values[index].Data as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public ObjectID GetObject(int index)
        {
			if (index >= m_Values.Count)
				return ObjectID.zero;

            if ( m_Values[index].nType != VarType.Object)
            {
                return ObjectID.zero;
            }

            return (ObjectID)m_Values[index].Data ;
        }

        public byte[] GetUserData(int index)
        {
			if (index >= m_Values.Count)
				return null;

            if (m_Values[index].nType != VarType.UserData)
            {
                return null;
            }

            return m_Values[index].Data as byte[];
        }

        public void SetBool(int index, bool value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Bool;
            vVardata.Data = value;
            m_Values[index] = vVardata;
        }

        public void SetInt(int index, int value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Int;
            vVardata.Data = value;
            m_Values[index] = vVardata;
        }

        public void SetInt64(int index, long value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Int64;
            vVardata.Data = value;
            m_Values[index] = vVardata;
        }

        public void SetFloat(int index, float value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Float;
            vVardata.Data = value;
            m_Values[index] = vVardata;  
        }

        public void SetDouble(int index, double value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Double;
            vVardata.Data = value;
            m_Values[index] = vVardata;  
        }

        public void SetString(int index, string value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.String;
            vVardata.Data = value;
            m_Values[index] = vVardata;  
        }

        public void SetWideStr(int index, string value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.WideStr;
            vVardata.Data = value;
            m_Values[index] = vVardata;  
        }

        public void SetObject(int index, ObjectID value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.Object;
            vVardata.Data =value.Clone();
            m_Values[index] = vVardata;  
        }

        public void SetUserData(int index, byte[] value)
        {
            VarData vVardata = VarData.zero;
            vVardata.nType = VarType.UserData;
            vVardata.Data = value.Clone();
            m_Values[index] = vVardata;  
        }

        public void AddBool(bool value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Bool;
            vVarData.Data = value;
            m_Values.Add(vVarData);
        }

        public void AddInt(int value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Int;
            vVarData.Data = value;
            m_Values.Add(vVarData);
        }

        public void AddInt64(long value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Int64;
            vVarData.Data = value;
            m_Values.Add(new VarData(VarType.Int64, value));
        }

        public void AddFloat(float value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Float;
            vVarData.Data = value;
            m_Values.Add(new VarData(VarType.Float, value));
        }

        public void AddDouble(double value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Double;
            vVarData.Data = value;
            m_Values.Add(vVarData);
        }

        public void AddString(string value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.String;
            vVarData.Data = value;
            m_Values.Add(vVarData);
        }

        public void AddWideStr(string value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.WideStr;
            vVarData.Data = value;
            m_Values.Add(vVarData);
        }

        public void AddObject(ObjectID value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.Object;
            vVarData.Data = value;
            m_Values.Add(vVarData);

        }

        public void AddUserData(byte[] value)
        {
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.UserData;
            vVarData.Data = value.Clone();
            m_Values.Add(vVarData);
        }
 
        public void AddUserData(byte[] value, int start, int size)
        {
            byte[] bytes = new byte[size];

            Array.Copy(value, start, bytes, 0, size);
            VarData vVarData = VarData.zero;
            vVarData.nType = VarType.UserData;
            vVarData.Data = bytes;
            m_Values.Add(vVarData);
        }

        public bool Copy(VarList varData)
        {
            return Copy(varData, 0, varData.GetCount());
        }
        public bool Copy(VarList varData, int start, int Count)
        {
            if (varData.m_Values == null)
            {
                return false;
            }
            int srcDataCount = varData.m_Values.Count;
            if (start >= srcDataCount)
            {
                return false;
            }
            int endPos = Count;
            if (start + Count > srcDataCount)
            {
                endPos = srcDataCount;
            }
            for (int i = start; i < endPos; i++)
            {
                m_Values.Add(varData.m_Values[i].Clone());
            }
            return true;
        }
    }
}