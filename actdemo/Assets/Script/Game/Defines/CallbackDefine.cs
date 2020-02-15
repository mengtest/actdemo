
/****************************
 * 服务器注册事件回调消息头 *
 * **************************/

public class CallbackDefine
{
    // 系统消息，格式：string msgid, int tipstype, string stringid, ...(参数表)
    public const int SERVER_CUSTOMMSG_SYSINFO = 1;
    // 发出的聊天信息，格式：int msgid, string stringid, ...(参数表)
    public const int SERVER_CUSTOMMSG_SPEECH = 2;
    //向客户端发送自定义的虚拟表格的内容，消息格式：
    //unsigned msgid, string recname, ...(第一行数据), ..., ...(第n行数据)
    public const int SERVER_CUSTOMMSG_VIRTUALREC_ADD = 3;
    //通知客户端清除自定义的虚拟表格的内容，消息格式：unsigned msgid, string recname
    public const int SERVER_CUSTOMMSG_VIRTUALREC_CLEAR = 4;
    //通知客户端更新自定义的虚拟表格的某行某列的内容，消息格式：
    //unsigned msgid, string recname, int row(行号), int col(列号),(数据)
    public const int SERVER_CUSTOMMSG_VIRTUALREC_UPDATE = 5;
    //通知客户端更新自定义的虚拟表格的某行的内容，消息格式：
    //unsigned msgid, string recname, int row(行号), ...(行数据)
    public const int SERVER_CUSTOMMSG_VIRTUALREC_UPDATE_ROW = 6;

    //通知客户端删除自定义的虚拟表格的某行，消息格式：
    //unsigned msgid, string recname, int row(行号)
    public const int SERVER_CUSTOMMSG_VIRTUALREC_REMOVE_ROW = 7;

    //通知客户端弹出一个MessageBox，消息格式：
    //unsigned msgid, int msgboxid, int count(按纽个数), ...(按纽的提示信息文本ID), string stringid(提示信息文本ID), ...(提示信息参数)
    public const int SERVER_CUSTOMMSG_SHOWMESSAGEBOX = 8;// "show_msgbox";

    //通知客户端弹出一个InputBox，消息格式：
    //unsigned msgid, int inputboxid, int type(0:只能数值,1:文本), string stringid(提示信息文本ID), ...(提示信息参数)
    public const int SERVER_CUSTOMMSG_SHOWINPUTBOX = 9;// "show_inputbox";
}