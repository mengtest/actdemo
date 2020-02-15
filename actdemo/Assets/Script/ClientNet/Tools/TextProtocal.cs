using System;
using System.Text;

namespace SysUtils
{
    // 文本方式通讯协议
    public class TextProtocal
    {
        // 消息编码
        public static bool EncodeMessage(VarList msg, ref string ret)
        {
            ret = string.Empty;

            int arg_num = msg.GetCount();

            for (int i = 0; i < arg_num; i++)
            {
                int type = msg.GetType(i);

                switch (type)
                {
                    case VarType.Int:
                        ret += Convert.ToString(msg.GetInt(i));
                        break;
                    case VarType.Int64:
                        ret += Convert.ToString(msg.GetInt64(i));
                        break;
                    case VarType.Float:
                        ret += Convert.ToString(msg.GetFloat(i));
                        break;
                    case VarType.Double:
                        ret += Convert.ToString(msg.GetDouble(i));
                        break;
                    case VarType.String:
                        {
                            byte[] bytes = Encoding.Default.GetBytes(
                                msg.GetString(i));

                            ret += "$" + StringToText(bytes);
                        }
                        break;
                    case VarType.WideStr:
                        ret += "#" + WideStrToText(msg.GetWideStr(i));
                        break;
                    case VarType.UserData:
                        ret += "*" + Convert.ToBase64String(
                            msg.GetUserData(i));
                        break;
                    default:
                        return false;
                }

                if (i < (arg_num - 1))
                {
                    // 以空格分割

                    ret += " ";
                }
            }

            return true;
        }

        // 消息解码
        public static bool DeocdeMessage(string str, ref VarList ret)
        {
            ret.Clear();

            int start_pos = 0;
            int space_pos = str.IndexOf(' ', start_pos);

            while (space_pos >= 0)
            {
                string s = str.Substring(start_pos, space_pos - start_pos);

                if (s != string.Empty)
                {
                    if (!DecodeString(s, ref ret))
                    {
                        return false;
                    }
                }

                start_pos = space_pos + 1;
                space_pos = str.IndexOf(' ', start_pos);
            }

            if (start_pos < str.Length)
            {
                string s = str.Substring(start_pos);

                if (s != string.Empty)
                {
                    if (!DecodeString(s, ref ret))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // 字符串转换成合法的协议文本

        private static string StringToText(byte[] bytes)
        {
            string ret = string.Empty;

            foreach (byte ch in bytes)
            {
                if ((ch > 0x20) && (ch < 0x7F) && (ch != 0x25))
                {
                    if (ch == '\\')
                    {
                        ret += '\\';
                    }

                    ret += (char)ch;
                }
                else
                {
                    ret += "\\x";

                    uint byte1 = ((uint)ch >> 4) & 0xF;
                    uint byte2 = (uint)ch & 0xF;

                    ret += IntToHex(byte1);
                    ret += IntToHex(byte2);
                }
            }

            return ret;
        }

        // 宽字符串转换成合法的协议文本
        private static string WideStrToText(string ws)
        {
            string ret = string.Empty;

            foreach (char ch in ws)
            {
                if ((ch > 0x20) && (ch < 0x7F) && (ch != 0x25))
                {
                    if (ch == '\\')
                    {
                        ret += '\\';
                    }

                    ret += ch;
                }
                else if (ch < 256)
                {
                    ret += "\\x";

                    uint byte1 = ((uint)ch >> 4) & 0xF;
                    uint byte2 = (uint)ch & 0xF;

                    ret += IntToHex(byte1);
                    ret += IntToHex(byte2);
                }
                else
                {
                    ret += "\\u";

                    uint byte1 = ((uint)ch >> 12) & 0xF;
                    uint byte2 = ((uint)ch >> 8) & 0xF;
                    uint byte3 = ((uint)ch >> 4) & 0xF;
                    uint byte4 = (uint)ch & 0xF;

                    ret += IntToHex(byte1);
                    ret += IntToHex(byte2);
                    ret += IntToHex(byte3);
                    ret += IntToHex(byte4);
                }
            }

            return ret;
        }

        // 将文本解码为参数
        private static bool DecodeString(string str, ref VarList ret)
        {
            char first = str[0];

            if (((first >= '0') && (first <= '9')) || (first == '.')
                || (first == '-'))
            {
                if (str.IndexOf('.') < 0)
                {
                    // 整数
                    if (str.Length < 10)
                    {
                        int v = 0;

                        try
                        {
                            v = Convert.ToInt32(str);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        ret.AddInt(v);
                    }
                    else
                    {
                        long v = 0;

                        try
                        {
                            v = Convert.ToInt64(str);
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        ret.AddInt64(v);
                    }
                }
                else
                {
                    // 浮点数

                    double v = 0;

                    try
                    {
                        v = Convert.ToDouble(str);
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    ret.AddDouble(v);
                }
            }
            else if (first == '#')
            {
                // 宽字符串
                string v = string.Empty;

                int i = 1;

                while (i < str.Length)
                {
                    char ch = str[i];

                    if (ch == '\\')
                    {
                        ++i;

                        if (i >= str.Length)
                        {
                            return false;
                        }

                        ch = str[i];

                        if (ch == '\\')
                        {
                            v += ch;
                            ++i;
                        }
                        else if (ch == 'x')
                        {
                            ++i;

                            if ((i + 2) >= str.Length)
                            {
                                return false;
                            }

                            uint byte1 = HexToInt(str[i]);
                            uint byte2 = HexToInt(str[i + 1]);

                            v += (char)((byte1 << 4) + byte2);
                            i += 2;
                        }
                        else if (ch == 'u')
                        {
                            ++i;

                            if ((i + 4) > str.Length)
                            {
                                return false;
                            }

                            uint byte1 = HexToInt(str[i]);
                            uint byte2 = HexToInt(str[i + 1]);
                            uint byte3 = HexToInt(str[i + 2]);
                            uint byte4 = HexToInt(str[i + 3]);

                            v += (char)((byte1 << 12) + (byte2 << 8)
                                + (byte3 << 4) + byte4);
                            i += 4;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if ((ch > 0x20) && (ch < 0x7F) && (ch != 0x25))
                    {
                        v += ch;
                        ++i;
                    }
                    else
                    {
                        return false;
                    }
                }

                ret.AddString(v);
            }
            else if (first == '*')
            {
                // 二进制数据

                string s = str.Substring(1);

                byte[] v = null;

                try
                {
                    v = Convert.FromBase64String(s);
                }
                catch (Exception)
                {
                    return false;
                }

                ret.AddUserData(v, 0, v.Length);
            }
            else
            {
                // 字符串

                int start_pos = 0;

                if (first == '$')
                {
                    start_pos = 1;
                }

                byte[] bytes = new byte[str.Length];

                int k = 0;
                int i = start_pos;

                while (i < str.Length)
                {
                    char ch = str[i];

                    if (ch == '\\')
                    {
                        ++i;

                        if (i >= str.Length)
                        {
                            return false;
                        }

                        ch = str[i];

                        if (ch == '\\')
                        {
                            bytes[k++] = (byte)'\\';
                            ++i;
                        }
                        else if (ch == 'x')
                        {
                            ++i;

                            if ((i + 2) >= str.Length)
                            {
                                return false;
                            }

                            uint byte1 = HexToInt(str[i]);
                            uint byte2 = HexToInt(str[i + 1]);

                            bytes[k++] = (byte)((byte1 << 4) + byte2);
                            i += 2;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if ((ch > 0x20) && (ch < 0x7F) && (ch != 0x25))
                    {
                        bytes[k++] = (byte)ch;
                        ++i;
                    }
                    else
                    {
                        return false;
                    }
                }

                string v = Encoding.Default.GetString(bytes, 0, k);

                ret.AddString(v);
            }

            return true;
        }

        // 从整数转换成十六进制字符表示
        private static char IntToHex(uint data)
        {
            if (data < 10)
            {
                return (char)((uint)'0' + data);
            }
            else
            {
                return (char)((uint)'A' + (data - 10));
            }
        }

        // 从十六进制字符表示转换成整数
        private static uint HexToInt(char hex)
        {
            if ((hex >= '0') && (hex <= '9'))
            {
                return (uint)(hex - '0');
            }
            else if ((hex >= 'A') && (hex <= 'F'))
            {
                return (uint)(hex - 'A' + 10);
            }
            else if ((hex >= 'a') && (hex <= 'f'))
            {
                return (uint)(hex - 'a' + 10);
            }
            else
            {
                return 0;
            }
        }
    }
}
