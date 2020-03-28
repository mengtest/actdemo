using UnityEngine;
using SysUtils;
using System;
using System.Collections.Generic;
using System.Collections;
using LitJson;

public class UtilTools
{
    public static float GetGroundHeightAt(Vector3 target)
    {
        RaycastHit hitdist;
        int mask = 1 << LayerMask.NameToLayer("Ground");
        Ray r = new Ray();
        r.origin = new Vector3(target.x, 50, target.z);
        r.direction = Vector3.down;

        if (Physics.Raycast(r, out hitdist, 60, mask))
        {
            return hitdist.point.y + 0.2f;
        }
        return 0f;
    }

    public static void PrintVarlist(string strTitle, VarList args, bool flag = false)
    {
        if (!flag)
            return;
        string str = UtilTools.StringBuilder("[", strTitle, " ", DateTime.Now.ToLongTimeString(), "]");

        for (int i = 0; i < args.GetCount(); ++i)
        {
            switch (args.GetType(i))
            {
                case VarType.Bool:
                    str += (args.GetBool(i) ? "true" : "false");
                    break;

                case VarType.Int:
                    str += args.GetInt(i);
                    break;

                case VarType.String:
                    str += args.GetString(i);
                    break;

                case VarType.WideStr:
                    str += args.GetWideStr(i);
                    break;
                case VarType.Object:
                    str += args.GetObject(i);
                    break;
                case VarType.Int64:
                    str += args.GetInt64(i);
                    break;
                case VarType.Float:
                    str += args.GetFloat(i);
                    break;
                default:
                    str += "unknown";
                    break;
            }

            str += " | ";
        }
        LogSystem.Log(str);
    }

    /// <summary>
    /// 赋值VarList
    /// </summary>
    /// <param name="args"></param>
    /// <param name="index"></param>
    /// <param name="newList"></param>
    public static void CopyVarList(ref VarList args, ref VarList newList, int start, int count)
    {
        int index = start;

        for (; index < args.GetCount() && count > 0; index++, count--)
        {
            int type = args.GetType(index);

            switch (type)
            {
                case VarType.Bool:
                    newList.AddBool(args.GetBool(index));
                    break;
                case VarType.Int:
                    newList.AddInt(args.GetInt(index));
                    break;
                case VarType.String:
                    newList.AddString(args.GetString(index));
                    break;
                case VarType.WideStr:
                    newList.AddWideStr(args.GetWideStr(index));
                    break;
                case VarType.Object:
                    newList.AddObject(args.GetObject(index));
                    break;
                case VarType.Float:
                    newList.AddFloat(args.GetFloat(index));
                    break;
                case VarType.Double:
                    newList.AddDouble(args.GetDouble(index));
                    break;
                case VarType.Int64:
                    newList.AddInt64(args.GetInt64(index));
                    break;
            }
        }
    }

    /// <summary>
    /// 自动根据类型读取index位置的数据 返回值为string
    /// </summary>
    /// <param name="args"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string GetVarList(VarList args, int index)
    {
        int type = args.GetType(index);

        switch (type)
        {
            case VarType.Bool:
                return args.GetBool(index) ? "true" : "false";
            case VarType.Int:
                return args.GetInt(index).ToString();
            case VarType.String:
                return args.GetString(index);
            case VarType.WideStr:
                return args.GetWideStr(index);
            case VarType.Object:
                return args.GetObject(index).ToString();
            case VarType.Float:
                return args.GetFloat(index).ToString();
            case VarType.Double:
                return args.GetDouble(index).ToString();
            case VarType.Int64:
                return args.GetInt64(index).ToString();
            default:
                return "null";
        }
    }

    /// <summary>
    /// 格式化字符串 id,id,id,id,id
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string[] FormatStringDot(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        return new string[1] { string.Empty };
    }

    /// <summary>
    /// 格式化字符串"1.0,1.0,1.0"为Vector3
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 FormatStringVector3(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Vector3.zero;
        }

        string[] values = value.Split(',');
        if (values == null || values.Length != 3)
        {
            return Vector3.zero;
        }

        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 格式化秘籍道具字符串
    /// 格式 ： 编号:数量；编号:数量
    /// 例如 :  item0001:5;item0002:5
    /// </summary>
    /// <param name="value"></param>
    /// <param name="itemNo"></param>
    /// <param name="itemNum"></param>
    public static Dictionary<string, int> FormatKeyNumber(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, new char[] { ',' }, new char[] { ':' });
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001,5;item0002,5
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumberDot(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, new char[] { ';' }, new char[] { ',' });
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001:5,item0002:5
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumberDot2(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, new char[] { ',' }, new char[] { ':' });
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001:5;item0002:5;
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumberDot3(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, new char[] { ';' }, new char[] { ':' });
    }

    /// <summary>
    /// 格式化字符串数据
    /// </summary>
    /// <param name="value"></param>
    /// <param name="splt1"></param>
    /// <param name="splt2"></param>
    /// <returns> Dictionary<string, string> </returns>
    public static Dictionary<string, string> FormatKeyString(string value, char[] splt1, char[] splt2)
    {
        Dictionary<string, string> taskitems = new Dictionary<string, string>();
        //string[] ss = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] ss = value.Split(splt1, System.StringSplitOptions.RemoveEmptyEntries);
        // string[] no = new string[] { };
        //string[] num = new string[] { };

        string item = string.Empty;
        string[] temp;
        for (int i = 0; i < ss.Length; i++)
        {
            item = ss[i];
            temp = item.Split(splt2, System.StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2)
            {
                if (taskitems.ContainsKey(temp[0]))
                {
                    string key = temp[0];
                    int old = UtilTools.IntParse(taskitems[key]);
                    int newi = UtilTools.IntParse(temp[1]);
                    taskitems[key] = UtilTools.StringBuilder(old, newi);
                }
                else
                {
                    taskitems.Add(temp[0], temp[1]);
                }
            }
        }

        return taskitems;
    }

    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumber(string value, char[] first, char[] second)
    {
        Dictionary<string, int> taskitems = new Dictionary<string, int>();
        //string[] ss = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] ss = value.Split(first, System.StringSplitOptions.RemoveEmptyEntries);
        //         string[] no = new string[] { };
        //         string[] num = new string[] { };

        string item = string.Empty;
        string[] temp;
        for (int i = 0; i < ss.Length; i++)
        {
            item = ss[i];
            temp = item.Split(second, System.StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2)
            {
                if (taskitems.ContainsKey(temp[0]))
                {
                    string key = temp[0];
                    int old = taskitems[key];
                    int newi = UtilTools.IntParse(temp[1]);
                    taskitems[key] = old + newi;
                }
                else
                {
                    taskitems.Add(temp[0], UtilTools.IntParse(temp[1]));
                }
            }
        }

        return taskitems;
    }

    /// <summary>
    /// String 强转 Int 时调用 默认返回 0
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IntParse(string value, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        int result;
        if (int.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }
    
    /// <summary>
    /// String强转Long
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long LongParse(string value, long defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        long result;
        if (long.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// String 强转 Float 时调用 默认返回 0
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float FloatParse(string value, float defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        float result;
        if (float.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 保留小数
    /// </summary>
    /// <param name="f">值</param>
    /// <param name="count">保留位数</param>
    /// <returns></returns>
    public static float KeepFloat(float f, int count = 1)
    {
        double b = System.Math.Round(f, count);
        return (float)b;
    }

    /// <summary>
    /// String 强转 bool 时调用 默认返回 false
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool BoolParse(string value, bool defaultValue = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        Boolean result;
        int iResult;
        if (Boolean.TryParse(value, out result))
        {
            return result;
        }
        else if (int.TryParse(value, out iResult))
        {
            return iResult == 1;
        }
        return defaultValue;
    }

    /// <summary>
    /// 检测文件加载完成后回调
    /// </summary>
    /// <param name="UIAsset"></param>
    /// <param name="NameAsset"></param>
    /// <param name="NumAsset"></param>
    public static void OnStringFileLoaded(System.Action completeCallback, int iTotal, bool[] args)
    {
        int iLoad = 0;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == true)
            {
                iLoad++;
            }
        }

        if (iLoad == args.Length)
        {
            ///完成回调
            if (completeCallback != null)
            {
                completeCallback();
            }
            return;
        }
    }

    public static string[] Split(string src, char p)
    {
        return src.Split(new char[] { p });
    }

    public static bool isEmpty(string src)
    {
        return src == null || src == string.Empty || src.Equals(string.Empty);
    }

    public static Vector4 ParseVector4(string str)
    {
        if (isEmpty(str)) return Vector4.zero;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 4) return Vector4.zero;
        return new Vector4(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]), float.Parse(strp[3]));
    }

    public static Vector3 ParseVector3(string str)
    {
        if (isEmpty(str)) return Vector3.zero;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 3) return Vector3.zero;
        return new Vector3(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]));
    }

    public static bool TryParseVector3(string str, out Vector3 vector)
    {
        vector = Vector3.zero;
        if (isEmpty(str)) return false;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 3) return false;
        vector = new Vector3(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]));
        return true;
    }

    public static Vector2 ParseVector2(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Vector2.zero;

        string[] strp = Split(str, ',');
        if (strp.Length != 2)
            return Vector2.zero;

        Vector2 vTemp2 = Vector2.zero;
        vTemp2.x = float.Parse(strp[0]);
        vTemp2.y = float.Parse(strp[1]);
        return vTemp2;
    }

    public static string Vector3String(Vector3 v)
    {
        return UtilTools.StringBuilder(v.x, ",", v.y, ",", v.z);
    }

    /// <summary>
    /// 转2维数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int[,] ParseInts(string str)
    {
        int[,] temp = null;
        if (!string.IsNullOrEmpty(str))
        {
            string[] strp = str.Split(',');
            if (strp.Length % 2 == 0)
            {
                int len = strp.Length / 2;
                temp = new int[len, 2];
                for (int i = 0; i < len; i++)
                {
                    temp[i, 0] = int.Parse(strp[2 * i]);
                    temp[i, 1] = int.Parse(strp[2 * i + 1]);
                }
            }
        }
        return temp;
    }

    private static System.Text.StringBuilder mstrbuilder = new System.Text.StringBuilder();
    /// <summary>
    /// 合并字符
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string StringBuilder(params object[] args)
    {
        mstrbuilder.Remove(0, mstrbuilder.Length);
        if (args != null)
        {
            int len = args.Length;
            for (int i = 0; i < len; ++i)
            {
                mstrbuilder.Append(args[i]);
            }
        }
        return mstrbuilder.ToString();
    }

    public delegate void OnWaitFrame();
    public static IEnumerator WaitForEndOfFrame(OnWaitFrame onWaitFrame)
    {
        yield return new WaitForEndOfFrame();
        if (onWaitFrame != null)
        {
            onWaitFrame();
        }
    }
    public delegate void OnCapture(Texture2D tex2d);
    /// <summary>
    /// 截屏
    /// </summary>
    /// <param name="rt">屏幕范围</param>
    /// <returns></returns>
    public static IEnumerator CaptureScreenshot(Rect rt, OnCapture onCapture)
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenShot = new Texture2D((int)rt.width, (int)rt.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rt, 0, 0);
        screenShot.Apply();
        if (onCapture != null)
        {
            onCapture(screenShot);
        }
    }
    /// <summary>
    /// 保存分享文件到指定路径
    /// </summary>
    /// <param name="strFileName">文件名</param>
    /// <param name="tex2d">图片信息</param>
    public static void SaveScreenShot(string strFileName, Texture2D tex2d)
    {
        byte[] bytes = tex2d.EncodeToPNG();
        if (bytes != null)
        {
            //System.IO.File.WriteAllBytes(strFileName, bytes);
        }
    }

    /// <summary>
    /// 获取字符串中的url参数
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetUrlValue(string text)
    {
        int linkStart = text.IndexOf("[url=", 0);
        if (linkStart != -1)
        {
            linkStart += 5;
            int linkEnd = text.IndexOf("]", linkStart);

            if (linkEnd != -1)
            {
                int closingStatement = text.IndexOf("[/url]", linkEnd);
                if (closingStatement != -1)
                    return text.Substring(linkStart, linkEnd - linkStart);
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// 得到位置随机点
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dis"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 pos, float minDis, float maxDis)
    {
        float radius = UnityEngine.Random.Range(minDis, maxDis);
        Quaternion rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 359), 0);
        Vector3 _Pos = Vector3.forward * radius;
        pos += rot * _Pos;
        pos.y += 0.3f;
        return pos;
    }

    /// <summary>
    /// 掉落位置
    /// </summary>
    /// <param name="qua"></param>
    /// <param name="pos"></param>
    /// <param name="minZ"></param>
    /// <param name="maxZ"></param>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <returns></returns>
    public static Vector3 GetBoxObjectPos(Vector3 pos, Quaternion qua, int z, int x)
    {
        Vector3 dir = new Vector3(x, 0, z);
        Vector3 p = qua * dir + pos;
        p.y += 0.3f;
        return p;
    }

    /// <summary>
    /// 随机位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="qua"></param>
    /// <param name="z"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector3 GetBoxObjectRandomPos(Vector3 pos, Quaternion qua, float fminz, float fmaxz, float fminx, float fmaxx)
    {
        float x = UnityEngine.Random.Range(fminx, fmaxx);
        float z = UnityEngine.Random.Range(fminz, fmaxz);
        Vector3 dir = new Vector3(x, 0, z);
        Vector3 p = qua * dir + pos;
        p.y += 0.3f;
        return p;
    }

    /// <summary>
    /// 通过秒转换为时间格式
    /// </summary>
    /// <param name="sceond"></param>
    /// <returns></returns>
    public static string TimeFormat(int sceond)
    {
        DateTime me = new DateTime().AddSeconds(sceond);
        return UtilTools.StringBuilder(me.Hour.ToString("00"), ":", me.Minute.ToString("00"), ":", me.Second.ToString("00"));
    }

    /// <summary>
    /// 随机数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputList"></param>
    /// <returns></returns>
    public static List<T> GetRandomList<T>(List<T> inputList)
    {
        //Copy to a array
        T[] copyArray = new T[inputList.Count];
        inputList.CopyTo(copyArray);

        //Add range
        List<T> copyList = new List<T>();
        copyList.AddRange(copyArray);

        //Set outputList and random
        List<T> outputList = new List<T>();
        System.Random rd = new System.Random(DateTime.Now.Millisecond);

        while (copyList.Count > 0)
        {
            //Select an index and item
            int rdIndex = rd.Next(0, copyList.Count - 1);
            T remove = copyList[rdIndex];

            //remove it from copyList and add it to output
            copyList.Remove(remove);
            outputList.Add(remove);
        }
        return outputList;
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        if (go.transform.childCount > 0)
        {
            int len = go.transform.childCount;
            for (int i = 0; i < len; i++)
            {
                SetLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }

    /// <summary>
    /// 换shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="shader"></param>
    public static void ChangeShader(GameObject role, params string[] args)
    {
        if (args == null || args.Length == 0)
            return;

        Shader[] shaders = new Shader[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            shaders[i] = Shader.Find(args[i]);
            if (shaders[i] == null)
            {
                LogSystem.LogError("shader not found");
            }
        }

        Renderer[] mSkinnedMeshRender = role.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
        {
            return;
        }

        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            if (mSkinnedMeshRender[i] != null && mSkinnedMeshRender[i].material != null)
            {
                for (int j = 0; j < args.Length; j++)
                {
                    if (shaders[j] == null)
                        continue;

                    string strPrefix = args[j].Substring(0, args[j].Length - 1);
                    if (mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix))
                    {
                        mSkinnedMeshRender[i].material.shader = shaders[j];
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取膜拜坐标点
    /// </summary>
    /// <param name="strScene"></param>
    /// <param name="strScenePos"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool GetWorship(string strScene, string strScenePos, out Vector3 pos)
    {
        pos = Vector3.zero;
        if (string.IsNullOrEmpty(strScenePos) || string.IsNullOrEmpty(strScene))
            return false;
        string[] strTemps = strScenePos.Split(',');
        for (int i = 0; i < strTemps.Length; i+=3)
        {
            if (strScene == strTemps[i])
            {
                pos.x = IntParse(strTemps[i + 1]);
                pos.z = IntParse(strTemps[i + 2]);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 更新shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="strShader"></param>
    /// <param name="strPrefix"></param>
    public static void ChangeShader(GameObject role, Shader shader, string strPrefix, string strPrefix2)
    {
        if (shader == null)
        {
            LogSystem.LogError("ChangeShader::LoadError");
        }
        Renderer[] mSkinnedMeshRender = role.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
        {
            return;
        }

        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            if (mSkinnedMeshRender[i] != null && mSkinnedMeshRender[i].material != null)
            {
                if (mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix)
                    || mSkinnedMeshRender[i].material.shader.name.Equals(strPrefix2))
                {
                    mSkinnedMeshRender[i].material.shader = shader;
                }
            }
        }
    }


    /// <summary>
    /// 设置RenderQ
    /// </summary>
    /// <param name="oModel"></param>
    public static void SetUIModelRenderQ(GameObject oModel, int renderQueue = 3100)
    {
        if (oModel == null)
            return;
        int effect = LayerMask.NameToLayer("Effect");
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }

        ParticleSystemRenderer[] particles = oModel.GetComponentsInChildren<ParticleSystemRenderer>(true);
        if (particles != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// && 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 隐藏粒子特效
    /// </summary>
    /// <param name="oModel"></param>
    public static void HideParticles(GameObject oModel)
    {
        if (oModel == null)
            return;

        ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>(true);
        if (particles != null)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].enableEmission = false;
                particles[i].GetComponent<Renderer>().enabled = false;
            }
        }
    }

    /// <summary>
    /// 粒子特效适应界面模型
    /// </summary>
    /// <param name="oModel"></param>
    public static void AdjustParticlesToUI(GameObject oModel, float modelScale, float uiScale)
    {
        if (oModel == null)
            return;

        if (modelScale == 0f)
        {
            return;
        }

        ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>(true);
        if (particles != null)
        {
            float scaleRate = uiScale * modelScale;
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem particleSystem = particles[i];
                particleSystem.startSize *= scaleRate;
                particleSystem.startSpeed *= scaleRate;
                float maxParticles = particleSystem.maxParticles;
                maxParticles *= scaleRate;
                particleSystem.maxParticles = (int)maxParticles;
                ParticleSystemRenderer render = particleSystem.GetComponent<ParticleSystemRenderer>();
                if (render)
                {
                    render.maxParticleSize *= scaleRate;
                }
            }
        }
    }

    public static void SetObjRenderQ(GameObject oModel, int iLayer, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == iLayer)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 渲染层级
    /// </summary>
    /// <param name="oModel"></param>
    public static void SetObjRenderQ(GameObject oModel, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 计算go对象下所有子对象的最大层级最大等级
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    //public static int CalculateMaxDepth(GameObject go)
    //{
    //    int depth = -1;
    //    UIWidget[] widgets = go.GetComponentsInChildren<UIWidget>();
    //    for (int i = 0, imax = widgets.Length; i < imax; ++i)
    //        depth = Mathf.Max(depth, widgets[i].depth);
    //    return depth;
    //}

    //public static void AdjustBgTexture(UITexture tex)
    //{
    //    //默认分辨率1280*720

    //}

    /// <summary>
    /// 解析json串
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static JsonData GetJsonData(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        JsonData jsonData = JsonMapper.ToObject(json);
        if (jsonData == null)
        {
            return null;
        }

        string code = jsonData["code"].ToString();
        if (code.Equals("0"))
        {
            return null;
        }

        return jsonData;
    }

    public static List<string> GetStringSplit(string str, char split = ',')
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        List<string> list = new List<string>();
        string[] strs = str.Split(split);
        for (int i = 0; i < strs.Length; ++i)
        {
            list.Add(strs[i]);
        }

        return list;
    }

    public static void DestroyGameObject(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        GameObject.Destroy(go);
        go = null;
    }

    public static void DestroyGameObject(params UnityEngine.Object[] args)
    {
        if (args == null)
        {
            return;
        }

        int count = args.Length;
        if (count < 1)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            GameObject go = args[i] as GameObject;

            GameObject.Destroy(go);
            go = null;
        }
    }

    /// <summary>
    /// 向量间的平面距离
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    /// <returns></returns>
    public static float Vec2Distance(Vector3 obj1, Vector3 obj2)
    {
        if (obj1 == null || obj2 == null)
        {
            return 0.0f;
        }

        Vector2 v1 = new Vector2(obj1.x, obj1.z);
        Vector2 v2 = new Vector2(obj2.x, obj2.z);

        return Vector2.Distance(v1, v2);
    }

    /// <summary>
    /// 对象之间的平面距离
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    /// <returns></returns>
    public static float Vec2Distance(IObject obj1, IObject obj2)
    {
        if (obj1 == null || obj2 == null)
        {
            return 0.0f;
        }

        Vector2 v1 = new Vector2(obj1.mPosition.x, obj1.mPosition.z);
        Vector2 v2 = new Vector2(obj2.mPosition.x, obj2.mPosition.z);

        return Vector2.Distance(v1, v2);
    }

    public static Vector3 Vec3Direction(Vector3 target, Vector3 self)
    {
        if (target == null || self == null)
        {
            return Vector3.zero;
        }

        Vector3 targetPosition = target;
        Vector3 selfPosition = self;

        targetPosition.y = 0.0f;
        selfPosition.y = 0.0f;

        return (targetPosition - selfPosition).normalized;
    }

    public static Vector3 Vec3Direction(IObject target, IObject self)
    {
        if (target == null || self == null)
        {
            return Vector3.zero;
        }

        Vector3 targetPosition = target.mPosition;
        Vector3 selfPosition = self.mPosition;

        return Vec3Direction(targetPosition, selfPosition);
    }

    public static Vector3 SwitchVector(Vector3 vec)
    {
        if (Camera.main != null)
        {
            float rectWidth = UIManager.Instance.mRectWidth;
            float rectHeight = UIManager.Instance.mRectHeight;

            Vector3 screenpoint = Camera.main.WorldToScreenPoint(vec);
            return new Vector3(screenpoint.x / Screen.width * rectWidth - rectWidth / 2, screenpoint.y / Screen.height * rectHeight - rectHeight / 2, 0);
        }
        else
        {
            return vec;
        }
    }

    public static void DestroyChild(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        int count = go.transform.childCount;
        if (count < 1)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child == null)
            {
                continue;
            }

            DestroyGameObject(child);
        }
    }

    /// <summary>
    /// 取浮点数小数部分
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float GetFloatDot(float value)
    {
        if (value < 0.0f)
        {
            return 0.0f;
        }

        if (value < 1.0f)
        {
            return value;
        }

        while (value > 1.0f)
        {
            value -= 1.0f;
        }

        return value;
    }
}