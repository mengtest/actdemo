using System;
using System.Collections.Generic;
using SysUtils;


namespace Fm_ClientNet
{
    public class GameRecord
    {
        private string recName;
        //private int nCols;
        private Dictionary<int, int> colTypes;

        private List<VarList> rowSet;


        public GameRecord()
        {
            rowSet = new List<VarList>();
            colTypes = new Dictionary<int, int>();
        }

        public List<VarList> GetRowSet()
        {
            return this.rowSet;
        }

        public GameRecord(int nCols)
        {
            colTypes = new Dictionary<int, int>();
            for (int i = 0; i < nCols; i++)
            {
                colTypes.Add(i, 0);
            }
            this.rowSet = new List<VarList>();
        }

        public void SetName(string name)
        {
            this.recName = name;
        }

        public string GetName()
        {
            return this.recName;
        }

        public void SetColCount(int nCount)
        {
            //this.nCols = nCount;
            try
            {
                if (colTypes.Count != 0)
                {
                    colTypes.Clear();
                }

                for (int i = 0; i < nCount; i++)
                {
                    colTypes.Add(i, 0);
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
            }
        }

        public int GetColcount()
        {
            return colTypes.Count;
        }

        public bool SetColType(int index, int type)
        {
            if (index >= colTypes.Count)
            {
                return false;
            }

            colTypes[index] = type;
            return true;
        }

        public int GetColType(int col)
        {
            if (col >= colTypes.Count)
            {
                return 0;
            }

            if (!colTypes.ContainsKey(col))
            {
                return 0;
            }

            return colTypes[col];
        }

        public int GetRowCount()
        {
            return rowSet.Count;
        }

        public bool InsertRow(int row)
        {
            try
            {
                int col_num = colTypes.Count;
                VarList newRow = VarList.GetVarList();

                if (row > col_num)
                {
                    rowSet.Add(newRow);
                }
                else
                {
                    rowSet.Insert(row, newRow);
                }

            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
            return true;
        }

        public bool DeleteRow(int row)
        {
            try
            {
                if (row >= rowSet.Count)
                {
                    return false;
                }
                VarList varlit = rowSet[row];
                varlit.Collect();
                rowSet.RemoveAt(row);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
            return true;
        }

        public void ClearRow()
        {
            try
            {
                for (int i = 0; i < rowSet.Count; i++)
                {
                    VarList varlit = rowSet[i];
                    varlit.Collect();
                }
                rowSet.Clear();
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return;
            }
            return;
        }

        public int FindRow(int col, Var key, int begRow, bool ignoreCase)
        {
            if (col >= GetColcount())
            {
                return -1;
            }

            int row_num = GetRowCount();
            if (begRow > row_num)
            {
                return -1;
            }

            int nColType = GetColType(col);
            if (0 == nColType)
            {
                return -1;
            }

            for (int i = begRow; i < row_num; i++)
            {
                VarList rowValueItem = rowSet[i];
                if ( 0 == rowValueItem.GetCount())
                {
                    return -1;
                }

                switch (nColType)
                {
                    case VarType.Bool:
                        if (rowValueItem.GetBool(col) == key.GetBool())
                        {
                            return i;
                        }
                        break;
                    case VarType.Int:
                        if (rowValueItem.GetInt(col) == key.GetInt())
                        {
                            return i;
                        }
                        break;
                    case VarType.Int64:
                        if (rowValueItem.GetInt64(col) == key.GetInt64())
                        {
                            return i;
                        }
                        break;
                    case VarType.Float:
                        if (Tools.FloatEqual(rowValueItem.GetFloat(col), key.GetFloat()))
                        {
                            return i;
                        }
                        break;
                    case VarType.Double:
                        if (Tools.DoubleEqual(rowValueItem.GetDouble(col), key.GetDouble()))
                        {
                            return i;
                        }
                        break;
                    case VarType.String:
                        if (0 == String.Compare(rowValueItem.GetString(col), key.GetString(), ignoreCase))
                        {
                            return i;
                        }
                        break;
                    case VarType.WideStr:
                        if (0 == String.Compare(rowValueItem.GetWideStr(col), key.GetWideStr(), ignoreCase))
                        {
                            return i;
                        }
                        break;
                    case VarType.Object:
                        if (rowValueItem.GetObject(col).Equals(key.GetObject()))
                        {
                            return i;
                        }
                        break;
                    default:
                        break;
                }
            }
            return -1;
        }

        public bool SetValue(int row, int col, Var result)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount()
                    || row < 0 || col < 0)
                {
                    return false;
                }

                VarList rowItem = rowSet[row];
             
                if (col > rowItem.GetCount())
                {
                    LogSystem.Log("col error col=" , col.ToString());
                    return false;
                }

                switch (result.Type)
                {
                    case VarType.Int:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetInt(col, result.GetInt());
                            }
                            else
                            {
                                rowItem.AddInt(result.GetInt());
                            }
                        }
                        break;
                    case VarType.Int64:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetInt64(col, result.GetInt64());
                            }
                            else
                            {
                                rowItem.AddInt64(result.GetInt64());
                            }
                        }
                        break;
                    case VarType.Float:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetFloat(col, result.GetFloat());
                            }
                            else
                            {
                                rowItem.AddFloat(result.GetFloat());
                            }
                        }
                        break;
                    case VarType.Double:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetDouble(col, result.GetDouble());
                            }
                            else
                            {
                                rowItem.AddDouble(result.GetDouble());
                            }
                        }
                        break;
                    case VarType.String:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetString(col, result.GetString());
                            }
                            else
                            {
                                rowItem.AddString(result.GetString());
                            }
                        }
                        break;
                    case VarType.WideStr:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetWideStr(col, result.GetWideStr());
                            }
                            else
                            {
                                rowItem.AddWideStr(result.GetWideStr());
                            }
                        }
                        break;
                    case VarType.Object:
                        {
                            if (col < rowItem.GetCount())
                            {
                                rowItem.SetObject(col, result.GetObject());
                            }
                            else
                            {
                                rowItem.AddObject(result.GetObject());
                            }
                        }
                        break;
                    default:
                        LogSystem.Log("typer error type=", result.Type.ToString());
                        return false;
                }//end switch

                //rowSet[row] = rowItem;
                //int nCount = rowItem.GetCount();
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
        }
        public bool GetInt(int row, int col, ref int iValue)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                LogSystem.Log("Error,GameRecord.GetValue row or col out of range:");
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
             
                if (colTypes[col] == VarType.Int)
                {
                    iValue = rowItem.GetInt(col);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return false;
        }
        public bool GetFloat(int row, int col, ref float fValue)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                LogSystem.Log("Error,GameRecord.GetValue row or col out of range:");
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
       
                if (colTypes[col] == VarType.Int)
                {
                    fValue = rowItem.GetFloat(col);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return false;
        }
        public bool GetDouble(int row, int col, ref double dValue)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                LogSystem.Log("Error,GameRecord.GetValue row or col out of range:");
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
         
                if (colTypes[col] == VarType.Int)
                {
                    dValue = rowItem.GetDouble(col);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return false;
        }
        public bool GetBool(int row, int col, ref bool bValue)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                LogSystem.Log("Error,GameRecord.GetValue row or col out of range:");
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
           
                if (colTypes[col] == VarType.Int)
                {
                    bValue = rowItem.GetBool(col);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return false;
        }
        public bool GetValue(int row, int col, ref Var result)
        {
            if (row < 0 || col < 0 || row >= GetRowCount() || col >= GetColcount())
            {
                return false;
            }

            try
            {
                VarList rowItem = rowSet[row];
            

                if (rowItem.GetCount() != GetColcount())
                {
                    LogSystem.Log("Error,GameRecord.GetValue Col Error:");
                    return false;
                }

                switch (colTypes[col])
                {
                    case VarType.Bool:
                        {
                            bool value = rowItem.GetBool(col);
                            result.SetBool(value);
                        }

                        break;

                    case VarType.Int:
                        {
                            int value = rowItem.GetInt(col);
                            result.SetInt(value);
                        }
                        break;

                    case VarType.Int64:
                        {
                            long value = rowItem.GetInt64(col);
                            result.SetInt64(value);
                        }
                        break;

                    case VarType.Float:
                        {
                            float value = rowItem.GetFloat(col);
                            result.SetFloat(value);
                        }
                        break;

                    case VarType.Double:
                        {
                            double value = rowItem.GetDouble(col);
                            result.SetDouble(value);
                        }
                        break;

                    case VarType.String:
                        {
                            string value = rowItem.GetString(col);
                            result.SetString(value);
                        }
                        break;

                    case VarType.WideStr:
                        {
                            string value = rowItem.GetWideStr(col);
                            result.SetWideStr(value);
                        }
                        break;

                    case VarType.Object:
                        {
                            ObjectID objId = rowItem.GetObject(col);
                            if (objId == null || objId.IsNull())
                            {
                                LogSystem.Log("Error,GameRecord.GetValue objid is null:");
                                return false;
                            }
                            result.SetObject(objId);
                        }
                        break;

                    default:
                        LogSystem.Log("Error,GameRecord.GetValue type error:");
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log("Error,GameRecord.GetValue Exception:", ex.ToString());
                return false;
            }

            return true;
        }

        public bool GetInt64(int row, int col, ref long lValue)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount())
                {
                    lValue = 0;
                    return false;
                }

                VarList row_value = (VarList)rowSet[row];
                if (row_value.GetType(col) != VarType.Int64)
                {
                    lValue = 0;
                    return false;
                }

                lValue = row_value.GetInt64(col);
            }
            catch (Exception ex)
            {
                lValue = 0;
                LogSystem.LogError(ex.ToString());
                return false;
            }

            return true;
        }
        public string GetString(int row, int col)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount())
                {
                    return string.Empty;
                }

                VarList row_value = (VarList)rowSet[row];
                if (row_value.GetType(col) != VarType.String)
                {
                    return string.Empty;
                }
                return row_value.GetString(col);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return string.Empty;
            }
        }

        public string GetWideStr(int row, int col)
        {
            try
            {
                if (row >= GetRowCount() || col >= GetColcount())
                {
                    return string.Empty;
                }

                VarList row_value = (VarList)rowSet[row];
                if (row_value.GetType(col) != VarType.WideStr)
                {
                    return string.Empty;
                }
                return row_value.GetWideStr(col);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return string.Empty;
            }
        }

        public void ReleaseAll()
        {
        }
    }
}



