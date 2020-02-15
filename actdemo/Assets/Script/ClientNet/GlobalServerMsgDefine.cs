
namespace Fm_ClientNet
{
    //服务器((引擎端))->客户端消息ID定义
    public enum GlobalServerMsg
    {
        SERVER_SERVER_INFO = 0,
        SERVER_SET_VERIFY = 1,
        SERVER_SET_ENCODE = 2,
        SERVER_ERROR_CODE = 3,
        SERVER_LOGIN_SUCCEED = 4,
        SERVER_WORLD_INFO = 5,
        SERVER_IDLE = 6,
        SERVER_QUEUE = 7,
        SERVER_TERMINATE = 8,
        SERVER_PROPERTY_TABLE = 9,
        SERVER_RECORD_TABLE = 10,
        SERVER_ENTRY_SCENE = 11,
        SERVER_EXIT_SCENE = 12,
        SERVER_ADD_OBJECT = 13,
        SERVER_REMOVE_OBJECT = 14,
        SERVER_SCENE_PROPERTY = 15,
        SERVER_OBJECT_PROPERTY = 16,
        SERVER_RECORD_ADDROW = 17,
        SERVER_RECORD_DELROW = 18,
        SERVER_RECORD_GRID = 19,
        SERVER_RECORD_CLEAR = 20,
        SERVER_CREATE_VIEW = 21,
        SERVER_DELETE_VIEW = 22,
        SERVER_VIEW_PROPERTY = 23,
        SERVER_VIEW_ADD = 24,
        SERVER_VIEW_REMOVE = 25,
        SERVER_SPEECH = 26,
        SERVER_SYSTEM_INFO = 27,
        SERVER_MENU = 28,
        SERVER_CLEAR_MENU = 29,
        SERVER_CUSTOM = 30,
        SERVER_LOCATION = 31,
        SERVER_MOVING = 32,
        SERVER_ALL_DEST = 33,
        SERVER_WARNING = 34,
        SERVER_FROM_GMCC = 35,
        SERVER_LINK_TO = 36,
        SERVER_UNLINK = 37,
        SERVER_LINK_MOVE = 38,
        SERVER_CP_CUSTOM = 39,			// 压缩的自定义消息
        SERVER_CP_ADD_OBJECT = 40,		// 压缩的添加可见对象消息
        SERVER_CP_RECORD_ADDROW = 41,// 压缩的表格添加行消息
        SERVER_CP_VIEW_ADD = 42,// 压缩的容器添加对象消息
        SERVER_VIEW_CHANGE = 43,// 容器改变
        SERVER_CP_ALL_DEST = 44,// 压缩的对象移动消息
        SERVER_ALL_PROP = 45,// 多个对象的属性改变信息
        SERVER_CP_ALL_PROP = 46,// 压缩的多个对象的属性改变信息
        SERVER_ADD_MORE_OBJECT = 47,// 增加多个对象
        SERVER_CP_ADD_MORE_OBJECT = 48,// 压缩的增加多个对象
        SERVER_REMOVE_MORE_OBJECT = 49,	// 移除多个对象
        SERVER_CHARGE_VALIDSTRING = 50, // 将计费服务器返回的验证串发给客户端

        // 安全消息定义
        SERVER_SECURITY_LOGIN = 51,
        SERVER_REP_OUTLINE_STAT = 52,
        SERVER_REP_DETAIL_STAT = 53,
        SERVER_REP_REMOTE_CTRL = 54,

        SERVER_MOVING_EX,			// 带延时的移动消息
	    SERVER_ALL_DEST_EX,			// 带延时的移动消息集合
	    SERVER_CP_ALL_DEST_EX,		// 压缩带延时的对象移动消息
	    SERVER_ENTRY_SCENE_EX,		// 支持帧同步的进入场景消息
	    SERVER_MEASURE_DELAY,		// 服务器延迟度量
	    SERVER_FRAME_UPDATE,        // 逻辑帧更新
	    SERVER_FRAME_MOVING,        // 基于逻辑帧的移动 -- 移动key
	    SERVER_FRAME_MOVING_DIFF,   // 基于逻辑帧的移动 -- 移动差值

	    SERVER_MSG_ENTRY_STUB,		// 远程服消息到client进入本地副本
	    SERVER_MSG_EXIT_STUB,		// 远程服消息到client退出本地副本
	    SERVER_MSG_TO_STUB,			// 远程服消息到客户端本地服

	    SERVER_MSG_VERSION,			// 服务端消息版本号
	    SERVER_MSG_BEAT,			// 服务器心跳消息
        SERVER_MSG_TRACERT,
	    SERVER_CP_RECORD_TABLE,
	    SERVER_CP_PROPERTY_TABLE,
	    SERVER_CP_LOGIN_SUCCEED,

	    SERVER_LOCATION_GRID,
	    SERVER_MOVING_GRID,
	    SERVER_ALL_DEST_GRID,

	    SERVER_CP_LOCATION_GRID,
	    SERVER_CP_MOVING_GRID,
	    SERVER_CP_ALL_DEST_GRID,

        SERVER_MSG_VOICE_KEY,	//语音服务器的key
        SERVER_MSG_STUB_TO_CLIENT,		// STUB 给客户端发消息（只是占位符），SERVER_MSG_STUB_TO_CLIENT + 100 为本地副本给net的消息号的开始位置
        SERVER_MSG_BATCH_COMPRESS,
        SERVER_MSG_STUB_EXITED_STUB = SERVER_MSG_STUB_TO_CLIENT+100,
	    SERVER_MESSAGE_MAX,
    }
}

