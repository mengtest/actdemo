using System;
using SysUtils;



namespace Fm_ClientNet
{
    public partial class GameReceiver
    {
        public bool RecvRecordRow(ref GameRecord rec, int index,
            ref LoadArchive ar, int row, int count)
        {
            try
            {
                if (rec == null || ar == null)
                {
                    LogSystem.Log("rec or ar is null");
                    return false;
                }

                if (row < 0 || count < 0)
                {
                    LogSystem.Log("row or count < 0");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("record table doesn't contain index:", index.ToString());
                    return false;
                }

                int col_count = mRecordTable[index].nCols;
                Var key = Var.zero;

                for (int i = row; i < (row + count); i++)
                {
                    if (!rec.InsertRow(i))
                    {
                        LogSystem.Log("InsertRow failed row =", i.ToString());
                        return false;
                    }

                    for (int c = 0; c < col_count; c++)
                    {
                        int nColType = mRecordTable[index].ColTypes[c];
                        switch (nColType)
                        {
                            case OuterDataType.OUTER_TYPE_BYTE:
                                {
                                    int value = 0;
                                    if (!ar.ReadInt8(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetInt(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_WORD:
                                {
                                    int value = 0;
                                    if (!ar.ReadInt16(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetInt(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_DWORD:
                                {
                                    int value = 0;
                                    if (!ar.ReadInt32(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetInt(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_QWORD:
                                {
                                    long value = 0;
                                    if (!ar.ReadInt64(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetInt64(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_FLOAT:
                                {
                                    float value = 0.0f;
                                    if (!ar.ReadFloat(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetFloat(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_DOUBLE:
                                {
                                    double value = 0.0;
                                    if (!ar.ReadDouble(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetDouble(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_STRING:
                                {
                                    string value = string.Empty;
                                    if (!ar.ReadString(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetString(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_WIDESTR:
                                {
                                    string value = string.Empty;
                                    if (!ar.ReadWideStr(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetWideStr(value);
                                }
                                break;
                            case OuterDataType.OUTER_TYPE_OBJECT:
                                {
                                    ObjectID value = ObjectID.zero;

                                    if (!ar.ReadObject(ref value))
                                    {
                                        return false;
                                    }
                                    key.SetObject(value);
                                }
                                break;
                            default:
                                LogSystem.Log("type error=", rec.GetName(), "_", nColType);
                                return false;
                        }//end switch

                        if (!rec.SetValue(i, c, key))
                        {
                            LogSystem.Log("set value failed record name :", rec.GetName());
                            return false;
                        }

                    }//end(int c = 0;

                }//end for(int i = row
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }
            return true;
        }

        public GameView GetView(string ident)
        {
            try
            {
                if (ident == null || ident.Length == 0)
                {
                    LogSystem.Log("ident is null");
                    return null;
                }

                if (null == m_client)
                {
                    LogSystem.Log("GameClient is null");
                    return null;
                }

                return m_client.GetViewByIdent(ident);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return null;
            }
        }

        public GameViewObj GetViewObj(string view_ident, string item_ident)
        {
            try
            {
                if (view_ident == null ||
                    item_ident == null ||
                    view_ident.Length == 0 ||
                    item_ident.Length == 0)
                {
                    return null;
                }
                GameView view = m_client.GetViewByIdent(view_ident);
                if (null == view)
                {
                    return null;
                }
                return (GameViewObj)view.GetViewObjByIdent(item_ident);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex);
                return null;
            }
        }


        public bool AddRecord(ref GameObj obj, int index)
        {
            try
            {
                if (index < 0 || index > mRecordTable.Count)
                {
                    LogSystem.Log("index error");
                    return false;
                }

                if (null == obj)
                {
                    LogSystem.Log("obj is null");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("not include index:", index.ToString());
                    return false;
                }

                RecData recData = mRecordTable[index];
                GameRecord record = new GameRecord();
                record.SetName(recData.strName);
                record.SetColCount(recData.nCols);
                for (int i = 0; i < recData.nCols; i++)
                {
                    int type = recData.ColTypes[i];
                    switch (type)
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                        case OuterDataType.OUTER_TYPE_WORD:
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                record.SetColType(i, VarType.Int);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                record.SetColType(i, VarType.Int64);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                record.SetColType(i, VarType.Float);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                record.SetColType(i, VarType.Double);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                record.SetColType(i, VarType.String);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                record.SetColType(i, VarType.WideStr);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                record.SetColType(i, VarType.Object);
                            }
                            break;
                        default:
                            LogSystem.Log("unkown type");
                            break;
                    }
                }//end for	

                return obj.AddRecord2Set(recData.strName, ref record);

            }//end try
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return false;
            }
        }

        public bool AddRecord(ref GameView obj, int index)
        {
            try
            {
                if (index < 0 || index > mRecordTable.Count)
                {
                    LogSystem.Log("index error");
                    return false;
                }

                if (null == obj)
                {
                    LogSystem.Log("obj is null");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("not include index:", index.ToString());
                    return false;
                }

                RecData recData = mRecordTable[index];
                GameRecord record = new GameRecord();
                record.SetName(recData.strName);
                record.SetColCount(recData.nCols);
                for (int i = 0; i < recData.nCols; i++)
                {
                    int type = recData.ColTypes[i];
                    switch (type)
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                        case OuterDataType.OUTER_TYPE_WORD:
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                record.SetColType(i, VarType.Int);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                record.SetColType(i, VarType.Int64);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                record.SetColType(i, VarType.Float);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                record.SetColType(i, VarType.Double);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                record.SetColType(i, VarType.String);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                record.SetColType(i, VarType.WideStr);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                record.SetColType(i, VarType.Object);
                            }
                            break;
                        default:
                            LogSystem.Log("unkown type");
                            break;
                    }
                }//end for	

                return obj.AddRecord2Set(recData.strName, ref record);

            }//end try
            catch (Exception ex)
            {
                LogSystem.Log("Exception:" , ex.ToString());
                return false;
            }
        }

        public bool AddRecord(ref GameViewObj obj, int index)
        {
            try
            {
                if (index < 0 || index > mRecordTable.Count)
                {
                    LogSystem.Log("index error");
                    return false;
                }

                if (null == obj)
                {
                    LogSystem.Log("obj is null");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("not include index:", index.ToString());
                    return false;
                }

                RecData recData = mRecordTable[index];
                GameRecord record = new GameRecord();
                record.SetName(recData.strName);
                record.SetColCount(recData.nCols);
                for (int i = 0; i < recData.nCols; i++)
                {
                    int type = recData.ColTypes[i];
                    switch (type)
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                        case OuterDataType.OUTER_TYPE_WORD:
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                record.SetColType(i, VarType.Int);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                record.SetColType(i, VarType.Int64);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                record.SetColType(i, VarType.Float);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                record.SetColType(i, VarType.Double);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                record.SetColType(i, VarType.String);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                record.SetColType(i, VarType.WideStr);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                record.SetColType(i, VarType.Object);
                            }
                            break;
                        default:
                            LogSystem.Log("unkown type");
                            break;
                    }
                }//end for	

                return obj.AddRecord2Set(recData.strName, ref record);

            }//end try
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return false;
            }
        }

        public bool AddRecord(ref GameSceneObj obj, int index)
        {
            try
            {
                if (index < 0 || index > mRecordTable.Count)
                {
                    LogSystem.Log("index error");
                    return false;
                }

                if (null == obj)
                {
                    LogSystem.Log("obj is null");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("not include index:", index.ToString());
                    return false;
                }

                RecData recData = mRecordTable[index];
                GameRecord record = new GameRecord();
                record.SetName(recData.strName);
                record.SetColCount(recData.nCols);
                for (int i = 0; i < recData.nCols; i++)
                {
                    int type = recData.ColTypes[i];
                    switch (type)
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                        case OuterDataType.OUTER_TYPE_WORD:
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                record.SetColType(i, VarType.Int);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                record.SetColType(i, VarType.Int64);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                record.SetColType(i, VarType.Float);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                record.SetColType(i, VarType.Double);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                record.SetColType(i, VarType.String);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                record.SetColType(i, VarType.WideStr);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                record.SetColType(i, VarType.Object);
                            }
                            break;
                        default:
                            LogSystem.Log("unkown type");
                            break;
                    }
                }//end for	

                return obj.AddRecord2Set(recData.strName, ref record);

            }//end try
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return false;
            }
        }

        public bool AddRecord(ref GameScene obj, int index)
        {
            try
            {
                if (index < 0 || index > mRecordTable.Count)
                {
                    LogSystem.Log("index error");
                    return false;
                }

                if (null == obj)
                {
                    LogSystem.Log("obj is null");
                    return false;
                }

                if (!mRecordTable.ContainsKey(index))
                {
                    LogSystem.Log("not include index:", index.ToString());
                    return false;
                }

                RecData recData = mRecordTable[index];
                GameRecord record = new GameRecord();
                record.SetName(recData.strName);
                record.SetColCount(recData.nCols);
                for (int i = 0; i < recData.nCols; i++)
                {
                    int type = recData.ColTypes[i];
                    switch (type)
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                        case OuterDataType.OUTER_TYPE_WORD:
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                record.SetColType(i, VarType.Int);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                record.SetColType(i, VarType.Int64);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                record.SetColType(i, VarType.Float);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                record.SetColType(i, VarType.Double);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                record.SetColType(i, VarType.String);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                record.SetColType(i, VarType.WideStr);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                record.SetColType(i, VarType.Object);
                            }
                            break;
                        default:
                            LogSystem.Log("unkown type");
                            break;
                    }
                }//end for	

                return obj.AddRecord2Set(recData.strName, ref record);

            }//end try
            catch (Exception ex)
            {
                LogSystem.Log("Exception:", ex.ToString());
                return false;
            }

        }

        public bool RecvRecordGrid(ref GameRecord rec, int isViewObj,
            int nIdent, int nSerial, int index, ref LoadArchive loadAr,
            int count)
        {
            try
            {
                Var key = Var.zero;

                for (int i = 0; i < count; i++)
                {
                    int row = 0;
                    int col = 0;
                    if (!loadAr.ReadInt16(ref row))
                    {
                        return false;
                    }
                    if (!loadAr.ReadInt8(ref col))
                    {
                        return false;
                    }

                    if (col >= mRecordTable[index].nCols)
                    {
                        LogSystem.Log("col error");
                        return false;
                    }

                    switch (mRecordTable[index].ColTypes[col])
                    {
                        case OuterDataType.OUTER_TYPE_BYTE:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt8(ref value))
                                {
                                    return false;
                                }
                                key.SetInt(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WORD:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt16(ref value))
                                {
                                    return false;
                                }
                                key.SetInt(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DWORD:
                            {
                                int value = 0;
                                if (!loadAr.ReadInt32(ref value))
                                {
                                    return false;
                                }
                                key.SetInt(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_QWORD:
                            {
                                long value = 0;
                                if (!loadAr.ReadInt64(ref value))
                                {
                                    return false;
                                }
                                key.SetInt64(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_FLOAT:
                            {
                                float value = 0.0f;
                                if (!loadAr.ReadFloat(ref value))
                                {
                                    return false;
                                }
                                key.SetFloat(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_DOUBLE:
                            {
                                double value = 0.0;
                                if (!loadAr.ReadDouble(ref value))
                                {
                                    return false;
                                }
                                key.SetDouble(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_STRING:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadString(ref value))
                                {
                                    return false;
                                }
                                key.SetString(value);

                            }
                            break;
                        case OuterDataType.OUTER_TYPE_WIDESTR:
                            {
                                string value = string.Empty;
                                if (!loadAr.ReadWideStr(ref value))
                                {
                                    return false;
                                }
                                key.SetWideStr(value);
                            }
                            break;
                        case OuterDataType.OUTER_TYPE_OBJECT:
                            {
                                ObjectID value = ObjectID.zero;


                                if (!loadAr.ReadObject(ref value))
                                {
                                    return false;
                                }
                                key.SetObject(value);
                            }
                            break;
                        default:
                            LogSystem.Log("unknown type record name ", rec.GetName());
                            return false;
                    }//end switch

                    if (!rec.SetValue(row, col, key))
                    {
                        LogSystem.Log("set value error recname ", rec.GetName());
                        return false;
                    }

                    if (isViewObj == 0)
                    {
                        string ident = UtilTools.StringBuilder(nIdent.ToString(), "-", nSerial.ToString());
                        VarList argList = VarList.GetVarList();
                        argList.AddString(ident);
                        argList.AddString(rec.GetName());
                        argList.AddInt(row);
                        argList.AddInt(col);
                        if (!Excute_CallBack("on_record_single_grid", argList))
                        {
                            LogSystem.Log("does not RegistCallBack on_record_single_grid");
                        }
                        argList.Collect();
                    }//end if (isViewObj == 0)
                    else if (isViewObj == 3)
                    {
                        string view_ident = nIdent.ToString();
                        VarList argList = VarList.GetVarList();
                        argList.AddString(view_ident);
                        argList.AddString(rec.GetName());
                        argList.AddInt(row);
                        argList.AddInt(col);
                        if (!Excute_CallBack("on_view_record_single_grid", argList))
                        {
                            LogSystem.Log("does not RegistCallBack on_view_record_single_grid");
                        }
                        argList.Collect();
                    }//end if (isViewObj == 3)
                    else if (isViewObj == 1)
                    {
                        string view_ident = nIdent.ToString();
                        string item_ident = nSerial.ToString();
                        VarList argList = VarList.GetVarList();
                        argList.AddString(view_ident);
                        argList.AddString(item_ident);
                        argList.AddString(rec.GetName());
                        argList.AddInt(row);
                        argList.AddInt(col);
                        if (!Excute_CallBack("on_viewobj_record_single_grid", argList))
                        {
                            LogSystem.Log("does not RegistCallBack on_viewobj_record_single_grid");
                        }
                        argList.Collect();
                    }//end if (isViewObj == 1)
                    else if (isViewObj == 2)
                    {
                        VarList argList = VarList.GetVarList();
                        argList.AddString(rec.GetName());
                        argList.AddInt(row);
                        argList.AddInt(col);
                        if (!Excute_CallBack("on_scene_record_single_grid", argList))
                        {
                            LogSystem.Log("does not RegistCallBack on_scene_record_single_grid");
                        }
                        argList.Collect();
                    }//end if (isViewObj == 2)

                }// end for (int i = 0; i < count; i ++ )



            }
            catch (System.Exception ex)
            {
                LogSystem.LogError(ex);
                return false;
            }

            return true;
        }
        public void SetSimpleProtocal(bool bSimple)
        {
            m_RecvHandle.m_bSimpleProtocal = bSimple;
        }
    }
}



