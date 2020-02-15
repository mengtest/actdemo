using SysUtils;
using System;
using Fm_ClientNet.Interface;

//通用逻辑无关的系统发送消息接口，不要添加逻辑界面的消息

//发送时为了区分宽字符用
public class WideString
{
    public WideString(string strContent)
    {
        mstrContent = strContent;
    }
    public static WideString ToWideString(string strContent)
    {
        return new WideString(strContent);
    }
    public string mstrContent;
}

public class GameCommand
{
    static IGameSender mGameSender = null;
    public static void SetSender(IGameSender gameSender)
    {
        mGameSender = gameSender;
    }
    //添加一个参数
    public static void AddObjectArgs(ref VarList args, ref  object o)
    {
        System.Type itype = o.GetType();
        if (itype == typeof(bool))
        {
            args.AddBool((bool)o);
        }
        else if (itype == typeof(int))
        {
            args.AddInt((int)o);
        }
        else if (itype == typeof(long))
        {
            args.AddInt64((long)o);
        }
        else if (itype == typeof(float))
        {
            args.AddFloat((float)o);
        }
        else if (itype == typeof(double))
        {
            args.AddDouble((double)o);
        }
        else if (itype == typeof(ObjectID))
        {
            args.AddObject((ObjectID)o);
        }
        else if (itype == typeof(string))
        {
            args.AddString((string)o);
        }
        else if (itype == typeof(byte[]))
        {
            args.AddUserData((byte[])o);
        }
        else if (itype == typeof(WideString))
        {
            args.AddWideStr(((WideString)o).mstrContent);
        }
        else if (itype == typeof(VarList))
        {
            int iCount = ((VarList)o).GetCount();
            for (int j = 0; j < iCount; j++)
            {
                switch (((VarList)o).GetType(j))
                {
                    case VarType.Bool:
                        args.AddBool(((VarList)o).GetBool(j));
                        break;
                    case VarType.Int:
                        args.AddInt(((VarList)o).GetInt(j));
                        break;
                    case VarType.Int64:
                        args.AddInt64(((VarList)o).GetInt64(j));
                        break;
                    case VarType.Float:
                        args.AddFloat(((VarList)o).GetFloat(j));
                        break;
                    case VarType.Double:
                        args.AddDouble(((VarList)o).GetDouble(j));
                        break;
                    case VarType.String:
                        args.AddString(((VarList)o).GetString(j));
                        break;
                    case VarType.WideStr:
                        args.AddWideStr(((VarList)o).GetWideStr(j));
                        break;
                    case VarType.Object:
                        args.AddObject(((VarList)o).GetObject(j));
                        break;
                    case VarType.UserData:
                        args.AddUserData(((VarList)o).GetUserData(j));
                        break;
                }
            }
        }
    }

    //发送客户端自定义消息
    public static bool SendCustom(CustomHeader iMSG, params object[] objects)
    {
        if (mGameSender == null)
            return false;

        VarList args = VarList.GetVarList();
        args.AddInt((int)iMSG);
        for (int i = 0; i < objects.Length; i++)
        {
            AddObjectArgs(ref args, ref objects[i]);
        }

        if (mGameSender.Custom(ref args))
        {
            args.Collect();
            return true;
        }
        args.Collect();
        return false;
    }

#if UNITY_EDITOR
    //发送客户端自定义消息,请保留此功能，谢谢！
    public static bool SendCustomGM(string content)
    {
        if (mGameSender == null)
            return false;

        VarList args = VarList.GetVarList();
        string[] split = content.Split(' ');
        int len = split.Length;
        if (len % 2 == 0)
        {
            for (int i = 0; i < len; ++i)
            {
                switch (split[i])
                {
                    case "BOOL":
                        ++i;
                        args.AddBool(Convert.ToBoolean(split[i]));
                        break;
                    case "INT":
                        ++i;
                        args.AddInt(Convert.ToInt32(split[i]));
                        break;
                    case "INT64":
                        ++i;
                        args.AddInt64(Convert.ToInt64(split[i]));
                        break;
                    case "FLOAT":
                        ++i;
                        args.AddFloat(Convert.ToSingle(split[i]));
                        break;
                    case "DOUBLE":
                        ++i;
                        args.AddDouble(Convert.ToDouble(split[i]));
                        break;
                    case "STRING":
                        ++i;
                        args.AddString(split[i]);
                        break;
                    case "WIDESTR":
                        ++i;
                        args.AddWideStr(split[i]);
                        break;
                }
            }
        }

        if (mGameSender.Custom(ref args))
        {
            args.Collect();
            return true;
        }
        args.Collect();
        return false;
    }
#endif

    /* @brief:请求移动消息
     * @param: iModel ，移动模式
     * @param: fsrcX ，移动时位置点X
     * @param: fsrcZ ，移动时位置点Z
     * @param: fReqX ，请求目标点X
     * @param: fReqZ ，请求目标点Z
        @return void
    */
    public static bool RequstMove(int iModel, float fsrcX, float fsrcZ, float fReqX, float fReqZ)
    {
        if (mGameSender == null)
            return false;

        VarList args = VarList.GetVarList();
        args.AddInt(iModel);
        args.AddFloat(fsrcX);
        args.AddFloat(fsrcZ);
        args.AddFloat(fReqX);
        args.AddFloat(fReqZ);
        VarList ret = VarList.GetVarList();
        if (mGameSender.RequestMove(ref args, ref ret))
        {
            args.Collect();
            bool bref = ret.GetBool(0);
            ret.Collect();
            return bref;
        }
        args.Collect();
        return false;
    }
    /// <summary>
    /// 请求停止
    /// </summary>
    /// <param name="iModel"></param>
    /// <param name="fsrcX"></param>
    /// <param name="fsrcY"></param>
    /// <param name="fsrcZ"></param>
    /// <param name="fReqX"></param>
    /// <param name="fReqZ"></param>
    /// <param name="fMoveSpeed"></param>
    /// <param name="strInfo"></param>
    /// <returns></returns>
    public static bool RequstStop(float fsrcX, float fsrcZ, float orient)
    {
        if (mGameSender == null)
            return false;

        VarList args = VarList.GetVarList();
        args.AddInt(0);
        args.AddFloat(fsrcX);
        args.AddFloat(fsrcZ);
        args.AddFloat(orient);
        VarList ret = VarList.GetVarList();
        if (mGameSender.RequestMove(ref args, ref ret))
        {
            args.Collect();
            //bool bret = ret.GetBool(0);
            //ret.Collect();
            //return bret;
            return true;
        }
        args.Collect();
        return false;
    }

    /// <summary>
    /// 发送选择消息
    /// </summary>
    /// <param name="strIdent">对象ID</param>
    /// <param name="iFuncId">选择方式</param>
    /// <returns></returns>
    public static bool Select(string strIdent, int iFuncId)
    {
        if (mGameSender == null)
            return false;

        return mGameSender.Select(strIdent, iFuncId);
    }

    /// <summary>
    /// 发送说话信息
    /// </summary>
    /// <param name="strInfo">说话内容</param>
    /// <returns></returns>
    public static bool Speech(string strInfo)
    {
        if (mGameSender == null)
            return false;

        return mGameSender.Speech(strInfo);
    }
    /// <summary>
    /// 客户端准备就绪
    /// </summary>
    /// <returns></returns>
    public static bool ClientReady()
    {
        if (mGameSender == null)
            return false;

        return mGameSender.ClientReady();
    }
}