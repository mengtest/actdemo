using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fm_ClientNet.Interface;

/// <summary>
/// 公式解析器管理类，供外部调用
/// </summary>
public class FormulaParserManager
{
    private static FormulaParserManager ms_instance;
    public static FormulaParserManager Instance
    {
        get
        {
            if (ms_instance == null)
            {
                ms_instance = new FormulaParserManager();
            }

            return ms_instance;
        }
    }

    // 公式最大长度
    public const int MAX_FORMULA_LEN = 256;

    // 单个节点最大长度
    public const int MAX_NODE_LEN = 64;

    // 最大节点个数
    public const int MAX_NODE_COUNT = 256;

    // 最大参数个数
    public const int MAX_PARAM_COUNT = 16;

    // 公式解析树集合
    Dictionary<string, FormulaParser> m_formulaParser = new Dictionary<string, FormulaParser>();

    // 检测公式合法性
    public static bool CheckFormula(string formula)
    {
        if (formula == null || formula == string.Empty)
        {
            return false;
        }

        int strLen = formula.Length;
        if (strLen <= 0 || strLen >= MAX_FORMULA_LEN)
        {
            return false;
        }

        return true;
    }

    // 注册公式
    public bool RegistFormula(string formula)
    {
        // 检测公式合法性
        if (!CheckFormula(formula))
        {
            return false;
        }

        // 公式已注册
        if (m_formulaParser.ContainsKey(formula))
        {
            return false;
        }

        // 创建公式解析树
        FormulaParser formulaParser = new FormulaParser();
        if (!formulaParser.CreateFormulaTree(formula))
        {
            return false;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            m_formulaParser.Add(formula, formulaParser);
        }

        return true;
    }

    // 计算公式值
    public float CalcFormula(IObject obj, string formula)
    {
        if (!CheckFormula(formula) || obj == null)
        {
            return 0.00f;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            FormulaParserManager.Instance.RegistFormula(formula);
            //return 0.00f;
        }

        FormulaParser formulaParser = m_formulaParser[formula];
        formulaParser.SetSelfObj(obj);

        return formulaParser.Value();
    }

    public float CalcFormula(IObject obj, IObject target, string formula)
    {
        if (!CheckFormula(formula) || obj == null)
        {
            return 0.00f;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            FormulaParserManager.Instance.RegistFormula(formula);
            //return 0.00f;
        }

        FormulaParser formulaParser = m_formulaParser[formula];
        formulaParser.SetSelfObj(obj);
        formulaParser.SetTargetObj(target);

        return formulaParser.Value();
    }

    /// <summary>
    /// 根据手动传值来计算公式
    /// </summary>
    /// <param name="dic"></param>
    /// <param name="formula"></param>
    /// <returns></returns>
    public float CalcFormula(Dictionary<string, float> dic, string formula)
    {
        if (!CheckFormula(formula) || dic == null)
        {
            return 0.00f;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            FormulaParserManager.Instance.RegistFormula(formula);
        }

        FormulaParser formulaParser = m_formulaParser[formula];
        formulaParser.SetParameter(dic);

        return formulaParser.Value();
    }

    public float CalcFormula(string formula)
    {
        if (!CheckFormula(formula))
        {
            return 0.00f;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            FormulaParserManager.Instance.RegistFormula(formula);
        }

        FormulaParser formulaParser = m_formulaParser[formula];

        return formulaParser.Value();
    }

    // 计算战斗相关公式
    public float CalcSkillFormula(IObject self, IObject selfSkill, IObject target, IObject targetSkill, string formula)
    {
        if (!CheckFormula(formula) || self == null || selfSkill == null)
        {
            return 0.00f;
        }

        if (!m_formulaParser.ContainsKey(formula))
        {
            FormulaParserManager.Instance.RegistFormula(formula);
        }

        FormulaParser formulaParser = m_formulaParser[formula];
        formulaParser.SetSelfObj(self);
        formulaParser.SetSelfSkill(selfSkill);
        formulaParser.SetTargetObj(target);
        formulaParser.SetTargetSkill(targetSkill);

        return formulaParser.Value();
    }
}

/// <summary>
/// 公式节点
/// </summary>
public class FormulaNode
{
    public NodeType type;
    public char op;
    public float value;
    //public char[] chValue;
    public string chValue;
    public char from;
    public FormulaNode left;
    public FormulaNode right;
    public int sign;

    public FormulaNode()
    {
        type = NodeType.NODE_TYPE_OP;
        op = ' ';
        value = 0.0f;
        from = ' ';
        sign = 0;
        chValue = "";
        left = null;
        right = null;
    }
}

// 公式节点类型枚举
public enum NodeType
{
    NODE_TYPE_OP,
    NODE_TYPE_NUM,
    NODE_TYPE_VAR,
    NODE_TYPE_EXPAND_VAR,
    NODE_TYPE_PARAM,
    NODE_TYPE_RANDOM
}

// 运算符优先级
public class Priority
{
    public char op;
    public int isp;
    public int icp;

    public Priority(char op, int isp, int icp)
    {
        this.op = op;
        this.isp = isp;
        this.icp = icp;
    }
};

/// <summary>
/// 公式解析器
/// </summary>
public class FormulaParser
{
    // 公式操作自己对象
    private IObject m_selfObj;

    public void SetSelfObj(IObject obj)
    {
        m_UseParameter = false;
        m_selfObj = obj;
    }

    private Dictionary<string, float> m_parameter;
    private bool m_UseParameter = false;
    public void SetParameter(Dictionary<string, float> dic)
    {
        m_UseParameter = true;
        m_parameter = dic;
    }

    // 公式操作目标对象
    private IObject m_targetObj;
    public void SetTargetObj(IObject obj)
    {
        m_targetObj = obj;
    }

    // 自己的技能对象
    private IObject m_selfSkillObj;
    public void SetSelfSkill(IObject obj)
    {
        m_selfSkillObj = obj;
    }

    // 目标的技能对象
    private IObject m_targetSkillObj;
    public void SetTargetSkill(IObject obj)
    {
        m_targetSkillObj = obj;
    }

    // 公式节点池
    private List<FormulaNode> m_formulaNode = new List<FormulaNode>();

    // 当前有效节点数量
    private int m_count = 0;

    // 存储运算符优先级
    const int m_symbols = 8;
    public Priority[] m_priTable = new Priority[m_symbols];

    float[] m_param = new float[FormulaParserManager.MAX_PARAM_COUNT];

    // 根节点
    FormulaNode m_root = new FormulaNode();

    public FormulaParser()
    {
        // 初始化运算符优先级
        if (m_priTable[0] == null)
        {
            m_priTable[0] = new Priority('#', 0, 0);
            m_priTable[1] = new Priority('+', 3, 2);
            m_priTable[2] = new Priority('-', 3, 2);
            m_priTable[3] = new Priority('*', 5, 4);
            m_priTable[4] = new Priority('/', 5, 4);
            m_priTable[5] = new Priority('^', 6, 7);
            m_priTable[6] = new Priority('(', 1, 8);
            m_priTable[7] = new Priority(')', 8, 1);
        }

        // 初始化节点池
        for (int i = 0; i < FormulaParserManager.MAX_NODE_COUNT; i++)
        {
            FormulaNode node = new FormulaNode();
            m_formulaNode.Add(node);
        }
    }

    // 创建公式解析树
    public bool CreateFormulaTree(string formula)
    {
        if (!ScanNode(formula))
        {
            return false;
        }

        Stack<FormulaNode> nodeStack = new Stack<FormulaNode>();
        Stack<FormulaNode> opStack = new Stack<FormulaNode>();
        FormulaNode tempNode = new FormulaNode();
        tempNode.type = NodeType.NODE_TYPE_OP;
        tempNode.op = '#';
        opStack.Push(tempNode);

        for (int i = 0; i < m_count; i++)
        {
            if (m_formulaNode[i].type == NodeType.NODE_TYPE_NUM
                || m_formulaNode[i].type == NodeType.NODE_TYPE_VAR
                || m_formulaNode[i].type == NodeType.NODE_TYPE_PARAM
                || m_formulaNode[i].type == NodeType.NODE_TYPE_RANDOM)
            {
                m_formulaNode[i].left = null;
                m_formulaNode[i].right = null;

                nodeStack.Push(m_formulaNode[i]);
            }
            else if (m_formulaNode[i].type == NodeType.NODE_TYPE_OP)
            {
                switch (m_formulaNode[i].op)
                {
                    case '(':
                        {
                            opStack.Push(m_formulaNode[i]);
                        }
                        break;

                    case ')':
                        {
                            while (opStack.Peek().op != '(')
                            {
                                FormulaNode op = opStack.Peek();
                                opStack.Pop();

                                FormulaNode right = nodeStack.Peek();
                                nodeStack.Pop();

                                FormulaNode left = nodeStack.Peek();
                                nodeStack.Pop();

                                op.left = left;
                                op.right = right;
                                nodeStack.Push(op);
                            }
                            if (opStack.Count < 1)
                            {
                                return false;
                            }
                            opStack.Pop(); //弹出栈顶左括号
                        }
                        break;

                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                        {
                            while (CmpSymbolPri(opStack.Peek().op, m_formulaNode[i].op) >= 0)
                            {
                                FormulaNode op = opStack.Peek();
                                opStack.Pop();

                                FormulaNode right = nodeStack.Peek();
                                nodeStack.Pop();

                                FormulaNode left = nodeStack.Peek();
                                nodeStack.Pop();

                                op.left = left;
                                op.right = right;
                                nodeStack.Push(op);
                            }
                            opStack.Push(m_formulaNode[i]);
                        }
                        break;

                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        while (opStack.Peek().op != '#')
        {
            FormulaNode op = opStack.Peek();
            opStack.Pop();

            FormulaNode right = nodeStack.Peek();
            nodeStack.Pop();

            FormulaNode left = nodeStack.Peek();
            nodeStack.Pop();

            op.left = left;
            op.right = right;

            nodeStack.Push(op);
        }

        opStack.Pop();
        if (nodeStack.Count != 1)
        {
            return false;
        }

        m_root = nodeStack.Peek();
        nodeStack.Pop();
        return true;
    }

    // 比较2个运算符优先级
    private int CmpSymbolPri(char ispop, char icpop)
    {
        int priIsp = -1;
        int priIcp = -1;

        for (int i = 0, j = 0; i < m_symbols; i++)
        {
            if (m_priTable[i].op == ispop)
            {
                priIsp = m_priTable[i].isp;
                ++j;
                if (j == 2)
                {
                    break;
                }
            }
            if (m_priTable[i].op == icpop)
            {
                priIcp = m_priTable[i].icp;
                ++j;
                if (j == 2)
                {
                    break;
                }
            }
        }

        return priIsp - priIcp;
    }

    // 解析公式，生成节点
    private bool ScanNode(string formula)
    {
        int len = formula.Length;

        int sign = 1;
        bool inNum = true;

        for (int i = 0; i < len; i++)
        {
            if (OpNumber(formula[i]))
            {
                if (inNum)
                {
                    int bytes = 0;
                    bool scanRes = false;
                    string subStr = formula.Substring(i, len - i);

                    char[] chValue = new char[FormulaParserManager.MAX_NODE_LEN];

                    if (IsDigit(formula[i]) || formula[i] == '.')
                    {
                        m_formulaNode[m_count].type = NodeType.NODE_TYPE_NUM;
                        scanRes = ScanNumber(subStr, ref m_formulaNode[m_count].value, ref bytes);
                    }
                    else if (formula[i] == '@' || formula[i] == '#' || formula[i] == '$' || formula[i] == '%')
                    {
                        m_formulaNode[m_count].type = NodeType.NODE_TYPE_VAR;
                        scanRes = ScanSymbol(subStr, ref chValue, ref bytes);
                        m_formulaNode[m_count].op = subStr[0];
                    }
                    else if (formula[i] == '&')
                    {
                        m_formulaNode[m_count].type = NodeType.NODE_TYPE_EXPAND_VAR;
                        scanRes = ScanExpandSymbol(subStr, ref chValue, ref bytes);
                        m_formulaNode[m_count].op = subStr[0];
                    }
                    else if (formula[i] == 'R' || formula[i] == 'L')
                    {
                        m_formulaNode[m_count].type = NodeType.NODE_TYPE_RANDOM;
                        scanRes = ScanRandom(subStr, ref chValue, ref bytes);
                        m_formulaNode[m_count].op = subStr[0];
                    }
                    else if (formula[i] == 'P')
                    {
                        m_formulaNode[m_count].type = NodeType.NODE_TYPE_PARAM;
                        scanRes = ScanParam(subStr, ref chValue, ref bytes);
                        m_formulaNode[m_count].op = subStr[0];
                    }
                    else
                    {
                        return false;
                    }

                    if (!scanRes)
                    {
                        return false;
                    }

                    m_formulaNode[m_count].chValue = new string(chValue, 0, bytes + 1);

                    i += bytes;
                    m_formulaNode[m_count].sign = sign;
                    m_count++;

                    if (m_count > FormulaParserManager.MAX_NODE_COUNT)
                    {
                        return false;
                    }

                    sign = 1;
                    inNum = false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                switch (formula[i])
                {
                    case '(':
                    case ')':
                        {
                            m_formulaNode[m_count].type = NodeType.NODE_TYPE_OP;
                            m_formulaNode[m_count].op = formula[i];

                            m_count++;

                            if (m_count > FormulaParserManager.MAX_NODE_COUNT)
                            {
                                return false;
                            }
                        }
                        break;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                        {
                            if (inNum)
                            {
                                if (formula[i] != '+' && formula[i] != '-')
                                {
                                    return false;
                                }

                                while (formula[i] == '+' || formula[i] == '-')
                                {
                                    if (formula[i] == '-')
                                    {
                                        sign *= -1;
                                    }

                                    i++;
                                }

                                i--;
                            }
                            else
                            {
                                m_formulaNode[m_count].type = NodeType.NODE_TYPE_OP;
                                m_formulaNode[m_count].op = formula[i];

                                m_count++;

                                if (m_count > FormulaParserManager.MAX_NODE_COUNT)
                                {
                                    return false;
                                }
                                inNum = true;
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
        }

        return m_count > 0;
    }

    private bool ScanNumber(string str, ref float val, ref int bytes)
    {
        int len = str.Length;
        if (len < 0)
        {
            return false;
        }

        val = 0.00f;

        bool hasDot = false;
        float w = 1;
        int i = 0;

        for (; i < len; i++)
        {
            if (IsDigit(str[i]) || str[i] == '.')
            {
                if (str[i] == '.')
                {
                    if (hasDot)
                    {
                        return false;
                    }
                    hasDot = true;
                }
                else
                {
                    if (hasDot)
                    {
                        w *= 0.1f;
                        val += (str[i] - '0') * w;
                    }
                    else
                    {
                        val = val * 10 + str[i] - '0';
                    }
                }
            }
            else
            {
                break;
            }
        }

        bytes = i - 1;
        return true;
    }

    private bool ScanSymbol(string str, ref char[] buf, ref int bytes)
    {
        //最少两个字符'@' '#' '$' '%' 与一个字母的组合
        int len = str.Length;
        if (len < 2)
        {
            return false;
        }

        int i = 0;
        buf[i] = str[i];

        i++;

        //首字符必须是字母或者数字或下划线
        if (IsLetterUnderline(str[i]))
        {

            buf[i] = str[i];
            i++;

            for (; i < len; i++)
            {
                //后序字符
                if (IsLetterUnderline(str[i]) || IsDigit(str[i]))
                {
                    buf[i] = str[i];
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            return false;
        }

        //buf[i] = '\0';
        bytes = i - 1;

        return true;
    }

    private bool ScanExpandSymbol(string str, ref char[] buf, ref int bytes)
    {
        int len = str.Length;

        //最少三个字符'&' 数字 与一个字母的组合
        if (len < 3)
        {
            return false;
        }

        int i = 0;

        //'&'符号
        buf[i] = str[i];
        i++;

        //首字符必须是字母或者数字或下划线
        if (IsLetterUnderline(str[i]))
        {
            buf[i] = str[i];
            i++;

            for (; i < len; i++)
            {
                //后序字符
                if (IsLetterUnderline(str[i]) || IsDigit(str[i]))
                {
                    buf[i] = str[i];
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            return false;
        }

        //buf[i] = '\0';
        bytes = i - 1;
        return true;
    }

    private bool ScanRandom(string str, ref char[] buf, ref int bytes)
    {
        int len = str.Length;

        //R[M,N] or R[M]
        if (len < 6)
        {
            return false;
        }

        int i = 0;

        //R
        buf[i] = str[i];
        i++;

        //[
        if (str[i] != '[')
        {
            return false;
        }

        buf[i] = str[i];
        i++;

        bool find = false;

        for (; i < len; i++)
        {
            buf[i] = str[i];
            if (buf[i] == ']')
            {
                find = true;
                break;
            }
        }

        if (!find)
        {
            return false;
        }

        buf[i] = str[i];
        i++;

        //buf[i] = '\0';
        bytes = i - 1;

        return true;
    }

    private bool ScanParam(string str, ref char[] buf, ref int bytes)
    {
        int len = str.Length;
        if (len < 2)
        {
            return false;
        }
        int i = 0;
        buf[i] = str[i];
        i++;
        buf[i] = str[i];
        i++;
        //buf[i] = '\0';
        bytes = i - 1;
        return true;
    }

    //L与R用法相同，区别是L在计算随机值的时候受幸运值影响
    private bool OpNumber(char ch)
    {
        return ((ch >= '0' && ch <= '9' || ch == '.') ||
            (ch == '@' || ch == '#' || ch == '$' || ch == '%' || ch == '&') || (ch == 'R' || ch == 'L' || ch == 'P'));
    }

    // 是否是数字
    private bool IsDigit(char ch)
    {
        return ch >= '0' && ch <= '9';
    }

    // 是否是字母或下划线
    private bool IsLetterUnderline(char ch)
    {
        return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_';
    }

    // 公式结果
    public float Value()
    {
        return ValueNode(m_root);
    }

    private float ValueNode(FormulaNode cur)
    {
        if (cur == null)
        {
            return 0.00f;
        }

        if (cur.type == NodeType.NODE_TYPE_OP)
        {
            return Cacul(ValueNode(cur.left), cur.op, ValueNode(cur.right));
        }
        else
        {
            if (cur.type == NodeType.NODE_TYPE_NUM)
            {
                return cur.value * cur.sign;
            }
            else if (cur.type == NodeType.NODE_TYPE_VAR || cur.type == NodeType.NODE_TYPE_EXPAND_VAR ||
                cur.type == NodeType.NODE_TYPE_PARAM || cur.type == NodeType.NODE_TYPE_RANDOM)
            {
                return GetParamValue(cur.chValue) * cur.sign;
            }
            else
            {
                return 0.00f;
            }
        }
    }

    // 计算加减乘除，开方
    private float Cacul(float a, char op, float b)
    {
        float ret = 0.00f;
        switch (op)
        {
            case '+':
                ret = a + b;
                break;
            case '-':
                ret = a - b;
                break;
            case '*':
                ret = a * b;
                break;
            case '/':
                {
                    if (!(b == 0.0f))
                    {
                        ret = a / b;
                    }
                }
                break;
            case '^':
                ret = (float)System.Math.Pow(a, b);
                break;
            default:
                break;
        }

        return ret;
    }

    private float GetParamValue(string formula)
    {
        formula.Trim();
        float res = 0.0f;
        int len = formula.Length;
        //if (len == 0 || len >= FormulaParserManager.MAX_NODE_LEN - 1)
        if (string.IsNullOrEmpty(formula))
        {
            return res;
        }

        if (IsDigit(formula[0]) || formula[0] == '.')
        {
            bool hasDot = false;
            float w = 1;
            for (int i = 0; i < len; i++)
            {
                if (IsDigit(formula[i]) || formula[i] == '.')
                {
                    if (formula[i] == '.')
                    {
                        if (hasDot)
                        {
                            res = 0.00f;
                            break;
                        }
                        hasDot = true;
                    }
                    else
                    {
                        if (hasDot)
                        {
                            w *= 0.1f;
                            res += (formula[i] - '0') * w;
                        }
                        else
                        {
                            res = res * 10 + formula[i] - '0';
                        }
                    }
                }
                else
                {
                    res = 0.00f;
                    break;
                }
            }
        }
        else if (formula[0] == '@' || formula[0] == '#' || formula[0] == '$' || formula[0] == '%')
        {
            char[] var = new char[FormulaParserManager.MAX_NODE_LEN];
            int i = 0;
            //去掉头@ # $ %
            while (i < len - 1)
            {
                var[i] = formula[i + 1];
                i++;
            }
            var[i] = (char)0;
            //char op = formula[0];
            res = QueryPropValue(formula[0], new string(var, 0, i));
        }
        else if (formula[0] == '&')
        {
            char[] var = new char[FormulaParserManager.MAX_NODE_LEN];

            //去掉 &
            int i = 0;
            //获取参数
            while (i < len - 1)
            {
                var[i] = formula[i + 1];
                i++;
            }
            var[i] = (char)0;
            res = 0.0f;
        }
        //判断是否是属性相关的变量
        else if (formula[0] == 'P')
        {
            int index = formula[1] - '1';

            if (index < FormulaParserManager.MAX_PARAM_COUNT)
            {
                res = m_param[index];
            }
        }

        return res;
    }

    private float QueryPropValue(char chr, string propName)
    {
        IObject obj = null;

        float ret = 0.00f;
        if (chr == '@')
        {
            obj = m_selfObj;
        }
        else if (chr == '#')
        {
            obj = m_selfSkillObj;
            return 0.00f;
        }
        else if (chr == '$')
        {
            obj = m_targetObj;
        }
        else if (chr == '%')
        {
            obj = m_targetObj;
            return 0.00f;
        }
        if (m_UseParameter)
            ret = QueryPropValueToFloat(m_parameter, propName);
        else
            ret = QueryPropValueToFloat(obj, propName);

        return ret;
    }
    /// <summary>
    /// 根据手动传值来计算公式
    /// </summary>
    /// <param name="dic"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    private float QueryPropValueToFloat(Dictionary<string, float> dic, string propName)
    {
        if (propName == null || propName == string.Empty || dic == null || !dic.ContainsKey(propName))
        {
            return 0.0f;
        }
        return dic[propName];
    }

    private float QueryPropValueToFloat(IObject obj, string propName)
    {
        if (propName == null || propName == string.Empty || obj == null)
        {
            return 0.0f;
        }

        // 服务器数据，暂时不用
        //float prop = 0.0f;
        //obj.QueryPropFloat(propName, ref prop);
        float prop = (float)obj.GetProperty(propName).GetInt();

        return prop;
    }
}
