
using SysUtils;

namespace Fm_ClientNet.Interface
{
    //引擎初始化接口
    /*public interface IClientNet
    {
        //初始化
        bool Init();
        //结束
        bool UnInit();
        //接口获得
        IGameClient GetGameClient();
        IGameSock GetGameSock();
    }*/

    //回调接口
    public interface ISockCallee
    {
        /// <summary>
        /// 连接成功回调
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool on_connected(string addr, int port);

        /// <summary>
        ///  连接失败回调
        /// </summary>
        /// <param name="addr">远程ip</param>
        /// <param name="port">远程端口</param>
        /// <returns></returns>
        bool on_connect_failed(string addr, int port);

        /// <summary>
        /// 连接关闭回调
        /// </summary>
        /// <returns></returns>
        bool on_close();
    }


    public interface IGameSock
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sockcall"></param>
        /// <returns></returns>
        bool Init(ISockCallee sockcall);
        void InitUserSock();
        //获取发送对象
        IGameSender GetGameSender();

        //获取接受对象
        IGameReceiver GetGameReceiver();

        //连接服务器
        bool Connect(string addr, int port);

        //是否连接成功 
        bool Connected();

        //关闭连接
        bool Disconnect();

        //连接状态 
        SockState GetState();

        /// <summary>
        /// 网络是否消息阻塞
        /// </summary>
        /// <returns></returns>
        bool IsNetBlock();
        //消息注册
        bool RegistCallBack(string funcName, CallBack callBack);
        bool RemoveCallBack(string funcName);

    }

    public interface IGameSender
    {
        //客户端就绪
        bool ClientReady();
        bool ClientRetEncode();
        //
        bool GetVerify();

        bool Speech(string info);

        bool GetWorldInfo(int type);

        bool ChooseRole(string role_name);

        bool ReviveRole(string role_name);

        bool DeleteRole(string role_name);

        bool Select(string object_ident, int func_id);

        bool GetWorldInfo2(int type, string info);

        bool LoginByString(string account, string login_string, string device_uid);

        bool Login(string account, string password, string device_uid);

        bool CreateRole(ref VarList args);

        bool LoginByShield(string account, string password, string login_string, string device_uid);

        //发送带验证码的登录消息
        bool LoginVerify(string account, string password, string verify, string device_uid);

        bool LoginReconnect(string account, string password, string login_string, string device_uid, bool in_stub = false, string strRoleName = "", string strRoleInfo = "");

        bool RequestMove(ref VarList args, ref VarList ret);

        bool Custom(ref VarList args);

        bool SendTracert(uint uType = 0);
    }

    public interface IStubSender
    {
        // 本地副本发送自定义消息到FsSoloMember
        bool SendMsgToMember(int msg_type, ref VarList args, bool isSafe = false, bool isResend = false);
        //本地服登录
        bool LoginLocalService(int prop_sceneid, string role_name, string role_uid, string token, ref VarList args);
    }
    public interface IGameMsgHander
    {
        // 收到验证码图像
        int OnSetVerify(int width, int height, string code, byte[] pdata);

        // 收到加密数据
        int OnSetEncode();
        // 收到错误码
        int OnErrorCode(int error_code);
        // 收到登录成功
        int OnLoginSucceed(int is_free, int points, int year, int month,
            int day, int hour, int minute, int second, int roles);
        // 收到游戏世界信息
        int OnWorldInfo(int info_type, string info);
        // 收到服务主动断线
        int OnTerminate(int reason);
        // 收到可视属性列表
        int OnPropertyTable(int property_num);
        // 收到可视表格列表
        int OnRecordTable(int record_num);
        // 收到视野内聊天
        int OnSpeech(string ident,
            string content);
        // 收到系统信息
        int OnSystemInfo(int type, string info);
        // 收到菜单消息
        int OnMenu(string ident, int count);
        // 收到清除菜单
        int OnClearMenu();
        // 收到自定义消息
        int OnCustom(ref VarList args);
        // 收到空闲提示
        int OnIdle();
        // 收到警告消息
        int OnWarning(int type, string info);
        // 收到客服信息
        int OnFromGmcc(int gm_id, string gm_name,
            string info);
        // 收到服务器列表信息
        int OnServerInfo(int server_num);
        // 收到排队信息
        int OnQueue(int queue, int position, int prior_count);

        // 进入场景
        int OnEntryScene(string player_ident,
            int scene_property_num);
        int OnEntrySceneEx(string player_ident,
            int scene_property_num, int scene_motion_mode,
            int frame_time_delta,
            float move_line_diff_ratio,
            float move_rotate_diff_ratio);
        // 离开场景
        int OnExitScene();
        // 添加场景中对象
        int OnAddObject(string ident, int property_num);
        // 移除场景内对象之前
        int OnBeforeRemoveObject(string ident);
        // 移除场景内对象
        int OnRemoveObject(string ident);
        // 场景属性更新
        int OnSceneProperty(int property_num);
        // 场景内对象属性更新
        int OnObjectProperty(string ident,
            int property_num);
        // 场景内对对像，单个属性改变
        int OnObjectPropertyChange(string ident, string prop_name);
        // 视窗内对象属性更新
        int OnViewObjProperty(string view_ident,
            string item_ident, int property_num);
        // 创建视窗
        int OnCreateView(string view_ident, int capacity,
            int view_property_num);
        // 删除视窗
        int OnDeleteView(string view_ident);
        // 视窗属性更新
        int OnViewProperty(string view_ident,
            int property_num);
        // 视窗内添加对象 
        int OnViewAdd(string view_ident, string item_ident,
            int property_num);
        // 视窗内移除对象
        int OnViewRemove(string view_ident,
            string item_ident);
        // 视窗内对象发生移动
        int OnViewChange(string view_ident,
            string old_item_ident, string new_item_ident);
        // 场景内对象表格添加行
        int OnRecordAddRow(string ident, string record_name,
            int start_row, int row_num);
        // 视窗对象表格添加行
        int OnViewRecordAddRow(string view_ident,
            string record_name, int start_row, int row_num);
        // 视窗内对象表格添加行
        int OnViewObjRecordAddRow(string view_ident,
            string item_ident, string record_name, int start_row,
            int row_num);
        // 场景表格添加行
        int OnSceneRecordAddRow(string record_name, int start_row,
            int row_num);
        // 场景内对象表格移除行之前
        int OnRecordBeforeRemoveRow(string ident,
            string record_name, int row);
        // 场景内对象表格移除行
        int OnRecordRemoveRow(string ident,
            string record_name, int row);
        // 视窗对象表格移除行之前
        int OnViewRecordBeforeRemoveRow(string view_ident,
            string record_name, int row);
        // 视窗对象表格移除行
        int OnViewRecordRemoveRow(string view_ident,
            string record_name, int row);
        // 视窗内对象表格移除行之前
        int OnViewObjRecordBeforeRemoveRow(string view_ident,
            string item_ident, string record_name, int row);
        // 视窗内对象表格移除行
        int OnViewObjRecordRemoveRow(string view_ident,
            string item_ident, string record_name, int row);
        // 场景表格移除行之前
        int OnSceneRecordBeforeRemoveRow(string record_name,
            int row);
        // 场景内对象表格移除行
        int OnSceneRecordRemoveRow(string record_name,
            int row);
        // 场景内对象表格数据更新
        int OnRecordGrid(string ident,
            string record_name, int grid_num);
        // 视窗对象表格数据更新
        int OnViewRecordGrid(string view_ident,
            string record_name, int grid_num);
        // 视窗内对象表格数据更新
        int OnViewObjRecordGrid(string view_ident,
            string item_ident, string record_name,
            int grid_num);
        // 场景表格数据更新
        int OnSceneRecordGrid(string record_name,
            int grid_num);
        // 场景内对象表格清空
        int OnRecordClear(string ident,
            string record_name);
        // 视窗对象表格清空
        int OnViewRecordClear(string view_ident,
            string record_name);
        // 视窗内对象表格清空
        int OnViewObjRecordClear(string view_ident,
            string item_ident, string record_name);
        // 场景表格清空
        int OnSceneRecordClear(string record_name);
        // 场景内对象表格数据更新
        int OnRecordSingleGrid(string ident,
            string record_name, int row, int col);
        // 视窗对象表格数据更新
        int OnViewRecordSingleGrid(string view_ident,
            string record_name, int row, int col);
        // 视窗内对象表格数据更新
        int OnViewObjRecordSingleGrid(string view_ident,
            string item_ident, string record_name, int row,
            int col);
        // 场景表格数据更新
        int OnSceneRecordSingleGrid(string record_name, int row,
            int col);
        // 对象位置
        int OnLocation(string ident);
        // 对象移动
        int OnMoving(string ident);
        // 多个对象的移动目标
        int OnAllDestination(int object_num);
        // 连接到对象
        int OnLinkTo(string ident, string link_ident,
            float link_x, float link_y, float link_z, float link_orient);
        // 连接位置改变
        int OnLinkMove(string ident, string link_ident,
            float link_x, float link_y, float link_z, float link_orient);
        // 解除连接
        int OnUnlink(string ident);
        // 多个对象的属性变化
        int OnAllProperty(int object_num);
        // 增加多个对象
        int OnAddMoreObject(int object_num);
        // 移除多个对象
        int OnRemoveMoreObject(int object_num);
        // 给客户端发送计费验证串
        int OnServerChargeValidstring(string valid_string);
        // 服务器延迟度量
        int OnServerMeasureDelay(int index, int server_time);
        // 对象运动差值--基于逻辑帧
        int OnServerFrameMovingDiff(string ident, float diff_x, float diff_y,
            float diff_z, float diff_orient);
        // 服务器帧驱动
        int OnServerFrameUpdate(string ident, int frame_id);
        /// <summary>
        /// 格子系统定位
        /// </summary>
        /// <param name="strIdent"></param>
        /// <returns></returns>
        int OnLocationGrid(string strIdent);

        /// <summary>
        /// 格子系统移动
        /// </summary>
        /// <param name="strIdent"></param>
        /// <returns></returns>
        int OnMovingGrid(string strIdent);

        /// <summary>
        /// 格子系统定位
        /// </summary>
        /// <param name="strIdent"></param>
        /// <returns></returns>
        int OnAllDestinationGrid(int iCount);

        /// <summary>
        /// 服务器进入本地副本
        /// </summary>
        /// <param name="args">参数表</param>
        /// <returns></returns>
        ///int OnServerEntryStub(VarList args);
        /// <summary>
        /// 服务器离开本地副本
        /// </summary>
        /// <param name="args">参数表</param>
        /// <returns></returns>
        ///int OnServerExitStub(VarList args);
        /// <summary>
        /// 本地副本通知退出本地副本
        /// </summary>
        /// <param name="args">参数表</param>
        /// <returns></returns>
        ///int OnStubExitedStub(VarList args);
    }


    public interface ICustomHandler
    {
        // 处理消息
        bool Process(VarList msg);
    }

    /// <summary>
    /// 消息接收器
    /// 注意 不管是回调函数 还是 接口类 接收器都只回调一次
    /// </summary>

    //消息事件回调
    public delegate void CallBack(VarList args);

    public interface IGameReceiver
    {
        // 游戏消息处理对象
        void SetGameMsgHander(IGameMsgHander msgHandle);
        IGameMsgHander GetGameMsgHandler();

        // 自定义消息处理器对象
        void SetCustomHandler(ICustomHandler msgHandle);
        ICustomHandler GetCustomHandler();

        // 自定义消息的最大参数数量
        void SetMaxCustomArguments(int value);
        int GetMaxCustomArguments();

        //消息注册
        bool RegistCallBack(string funcName, CallBack callBack);
        bool RemoveCallBack(string funcName);

        /// <summary>
        /// 获取属性表的md5码
        /// </summary>
        string GetPropertyTableMd5();
        /// <summary>
        /// 获取表格表的md5码
        /// </summary>
        string GetRecordTableMd5();

        // 获得角色数量
        int GetRoleCount();

        // 获得每个角色的信息参数数量
        int GetRoleInfoCount();

        // 获得角色位置
        int GetRoleIndex(int index);

        // 获得角色标志
        int GetRoleSysFlags(int index);

        // 获得角色名称
        string GetRoleName(int index);

        // 获得角色参数
        string GetRolePara(int index);

        // 获得角色是否被删除
        int GetRoleDeleted(int index);

        // 获得角色删除时间
        double GetRoleDeleteTime(int index);

        // 获得所有的角色信息（可能包含附加信息）
        void GetRoleInfo(ref VarList args, ref VarList ret);

        // 获得菜单项的数量
        int GetMenuCount();

        // 获得菜单项的类型
        int GetMenuType(int index);

        // 获得菜单项的选择标记
        int GetMenuMark(int index);

        // 获得菜单项的内容
        string GetMenuContent(int index);

        // 清除所有数据信息
        bool ClearAll();

        // 导出消息统计数据
        bool DumpMsgStat(string file_name);

        //心跳时间
        uint GetBeatInterval();
    }
}
