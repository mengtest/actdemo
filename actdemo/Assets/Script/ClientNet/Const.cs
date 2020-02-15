

namespace Fm_ClientNet
{
    //游戏世界中用到的一些常量
    public class Const
    {

        //传输消息分割符
        public const byte MESSAGE_END_SUFFIX = 0xEE;

        //传输消息0xEE转义符
        public const byte MESSAGE_MID_SUFFIX = 0x00;

        //socket每次读取的消息长度
        public const int MAX_MESSAGE_LENGTH = 1024;

        //默认buffer缓冲区大小
        public const int DEFAULT_SOCKET_BUFFER_SIZE = 1024;

        //一次性验证串长度
        public const int VALIDATE_LEN_LENGTH = 1024;

        // 最大名字长度
        public const int OUTER_OBJNAME_LENGTH = 32;
        //属性数据类型
    }

    //属性数据类型
    public class OuterDataType
    {
        public const int OUTER_TYPE_UNKNOWN = 0;	// 未知
        public const int OUTER_TYPE_BYTE = 1;   // 一字节 //ReadInt8
        public const int OUTER_TYPE_WORD = 2;   // 二字节 //ReadInt16 或ReadUInt16
        public const int OUTER_TYPE_DWORD = 3;   // 四字节  //ReadUInt32 或ReadInt32
        public const int OUTER_TYPE_QWORD = 4;	// 八字节  //ReadUInt64 或ReadInt64
        public const int OUTER_TYPE_FLOAT = 5;	// 浮点四字节 //ReadFloat
        public const int OUTER_TYPE_DOUBLE = 6;	// 浮点八字节 // ReadDouble
        public const int OUTER_TYPE_STRING = 7;	// 字符串，前四个字节为长度 //ReadString
        public const int OUTER_TYPE_WIDESTR = 8;	// UNICODE宽字符串，前四个字节为长度 //ReadWideStr
        public const int OUTER_TYPE_OBJECT = 9;

    }

}
