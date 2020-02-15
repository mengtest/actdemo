

namespace Fm_ClientNet
{
    //客户端->服务器消息(引擎端)ID定义
    public class GlobalClineMsgId
    {
        //客户端->服务器
        //-----------一级协议--------------
        public static int CLIENT_GET_VERIFY = 0;
        public static int CLIENT_RET_ENCODE = 1;
        public static int CLIENT_LOGIN = 2;//登录
        public static int CLIENT_WORLD_INFO = 3;
        public static int CLIENT_CHOOSE_ROLE = 4;
        public static int CLIENT_CREATE_ROLE = 5;
        public static int CLIENT_DELETE_ROLE = 6;
        public static int CLIENT_SELECT = 7;
        public static int CLIENT_SPEECH = 8;
        public static int CLIENT_READY = 9;//客户端准备就绪
        public static int CLIENT_CUSTOM = 10;//自定义消息
        public static int CLIENT_REQUEST_MOVE = 11;
        public static int CLIENT_REVIVE_ROLE = 12;

        //安全消息定义
        public static int CLIENT_SECURITY_LOGIN = 20;
        public static int CLIENT_REQ_OUTLINE_STAT = 21;
        public static int CLIENT_REQ_DETAIL_STAT = 22;
        public static int CLIENT_REQ_REMOTE_CTRL = 23;

        public static int CLIENT_STUB_MSG_TO_SERVER;  //发送消息到服务器
        public static int CLIENT_BEAT = 25;         // 客户端心跳消息
        public static int CLIENT_TRACERT = 26;
       
    }
}

