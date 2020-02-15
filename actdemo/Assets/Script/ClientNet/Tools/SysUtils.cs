//--------------------------------------------------------------------
// 文件名:		SysUtil.cs
// 内  容:		
// 说  明:		
// 创建日期:	2006年11月30日	
// 创建人:		陆利民
// 版权所有:	苏州蜗牛电子有限公司
//--------------------------------------------------------------------

using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SysUtils
{
    // 工具
    public class SysUtil
    {
        // 字节流转换成字符串
        public static string BytesToString(byte[] buf)
        {
            string res = string.Empty;

            foreach (byte b in buf)
            {
                res += String.Format("{0:X}", b / 16);
                res += String.Format("{0:X}", b % 16);
            }

            return res;
        }

        // 获得MD5编码
        public static string GetMD5String(string origin)
        {
            byte[] buf = Encoding.Default.GetBytes(origin);

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] value = md5.ComputeHash(buf);

            return BytesToString(value);
        }

        // 获得文件的MD5校验码
        public static string GetMD5File(string file_name)
        {
            try
            {
                FileStream fs = File.OpenRead(file_name);

                byte[] data = new byte[fs.Length];

                fs.Read(data, 0, (int)fs.Length);
                fs.Close();

                MD5 md5 = new MD5CryptoServiceProvider();

                byte[] value = md5.ComputeHash(data);

                return BytesToString(value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // 字符串转换整数
        public static int StringToInt(string s)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // 字符串转换整数
        public static long StringToLong(string s)
        {
            try
            {
                return Convert.ToInt64(s);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // 字符串转换浮点数
        public static float StringToFloat(string s)
        {
            try
            {
                return Convert.ToSingle(s);
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }

        // 字符串转换浮点数
        public static double StringToDouble(string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }
    }
}
