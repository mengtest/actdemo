/*!
 * @brief	玩家使用技能请求
 * @param	string		使用技能的id
 * @param	float	x,y,z 默认是施法者脚下的点，如果需要在某个位置释放（比如点击下技能，再点击下屏幕其他位置并在这个位	置下火雨这样的情况），那么就是这个位置点。
 * @param	float orient 人物的朝向
 * @param	PERSISTID 客户端锁定的目标
 */
//CLIENT_CUSTOMMSG_USE_SKILL  = 211,

/*!	
 * @brief	技能流程消息 消息号由3个值拼成 
        每个阶段的事件类型A 		ESkillStageType(技能阶段)     		消息号
            0xff      	                ff    				ffff
//*************************************SKILL_STAGE_TYPE_BEGIN*********************/
//* @param	int64_t		技能的uid
//* @param	string		使用技能的id
//* @param	PERSISTID	施法者的对象号
//* @param	int		冷却时间
//*************************************SKILL_STAGE_TYPE_EFFECT*********************//
//    A:ESkillEventType
//* @param	int64_t  技能uid
//* @param	int	效果个数
//每个效果的参数
//* @param	具体见每个事件的参数类型
// */ 257	阶段1　　事件1　　技能的uid 使用技能的id  PERSISTID  冷却int
// 技能阶段定义


public class SkillDefine
{
    //人物y值
    public static float ROLE_HIGHT = 1.5f;
    //四方剑
    public static string SHI_FANG_SKILL = "10231";

    public enum ESkillStageType
    {
        SKILL_STAGE_TYPE_NONE = 0,
        SKILL_STAGE_TYPE_BEGIN,         // 开始使用技能：使用者，目标类型
        SKILL_STAGE_TYPE_PREPARE,       // 吟唱：截至时间
        SKILL_STAGE_TYPE_LEAD,          // 引导：截至时间
        SKILL_STAGE_TYPE_EFFECT,        // 造成效果：类型
        SKILL_STAGE_TYPE_BREAK,         // 打断：原因
        SKILL_STAGE_TYPE_FINISH,        // 结束：原因
        SKILL_STAGE_TYPE_RESET_DEST,    // 修改攻击目标  	

        SKILL_STAGE_TYPE_MAX,
    }

    // 事件类型定义定义
    public enum ESkillEventType
    {
        UNKNOW_TYPE = 0,

        // 技能检测可以释放
        SKILL_BEGIN = 1,
        // 技能开始准备的事件
        SKILL_PREPARE = 2,
        // 技能准备之后的事件
        SKILL_AFTER_PREPARE = 3,
        // 技能命中之前的事件
        SKILL_BEFORE_HIT = 4,
        // 技能命中并已伤害了对象的事件
        SKILL_HIT_DAMAGE = 5,
        // 技能使用结束的事件
        SKILL_FINISH = 6,
        // 技能被打断的事件
        SKILL_BREAK = 7,

        // 技能命中目标后，扣血之前触发的事件
        SKILL_AFTER_HIT = 8,

        MAX_EVENT_TYPE = 30,
    }

    // 攻击结果
    public enum EHIT_TYPE
    {
        HIT_TYPE_INVALID = -1,
        //物理攻击
        PHY_HIT_TYPE_MISS,//未命中
        PHY_HIT_TYPE_DODGE,//闪避
        PHY_HIT_TYPE_VA,//暴击
        PHY_HIT_TYPE_NORMAL,//一般命中
        PHY_HIT_TYPE_ADDHP,//加血
        PHY_HIT_TYPE_GAINT,//免疫
        ATTACK_POWER_RISE,//功击力上升
    };

    // 技能造成效果类型定义
    public enum ESkillStageEffectType
    {
        eSkillStageEffectType_None = 0,
        eSkillStageEffectType_DecHP,            // 扣除HP：hp
        eSkillStageEffectType_Miss,             // 未命中:
        eSkillStageEffectType_Hits,             // 击中次数：hits
        eSkillStageEffectType_AddHP,            // 医疗：hp
        eSkillStageEffectType_VA,               // 爆击：hp, times
        eSkillStageEffectType_Dodge,            // 闪避：
        eSkillStageEffectType_PlayEffect,       // 播放效果
        eSkillStageEffectType_BeHitAttack,		// 反击
        eSkillStageEffectType_Gaint, //免疫
        eSkillStageEffectType_Dec_CD, //重置CD
    }

    // 技能使用目标类型定义
    public enum ESkillStageTargetType
    {
        /// <summary>
        /// 没有目标
        /// </summary>
        SKILL_STAGE_TARGET_TYPE_NONE = 0,		// 

        /// <summary>
        /// 目标者
        /// </summary>
        SKILL_STAGE_TARGET_TYPE_TARGET,             // 

        /// <summary>
        ///  位置
        /// </summary>
        SKILL_STAGE_TARGET_TYPE_POSITION,           //

        /// <summary>
        ///  多位置
        /// </summary>
        SKILL_STAGE_TARGET_TYPE_MULTI_POSITION,     //

        /// <summary>
        /// 多目标
        /// </summary>
        SKILL_STAGE_TARGET_TYPE_MULTI_TARGET,       // 
    };




    //    /*!	
    // * @brief	buff流程消息  
    // * @param	int		技能阶段(BUFFER_STAGE)
    ////*************************************BUFFER_STAGE_BEGIN*********************//
    // * @param	int		0 
    // * @param	PERSISTID	目标的对象号 3
    // * @param	string		buff的id
    // * @param	int64_t		buff的uid

    //// 统一在确定目标后再释放技能
    ////*************************************BUFFER_STAGE_EFFECT*********************// 
    //* @param	int		技能使用目标类型(ESkillStageTargetType)
    //* @param	int64_t		buff的uid
    //* @param	PERSISTID	目标的对象号
    //* @param	具体见每个事件的参数类型

    ////*************************************BUFFER_STAGE_BREAK*********************//
    //* @param	int		buff清除原因(BUFFER_REMOVE)
    //* @param	int64_t		buff的uid
    //* @param	PERSISTID	目标的对象号

    ////*************************************BUFFER_STAGE_FINISH*********************//
    //* @param	int		buff清除原因(BUFFER_REMOVE)
    //* @param	int64_t		buff的uid
    //* @param	PERSISTID	目标的对象号
    // */
    //SERVER_CUSTOMMSG_BUFFER = 702
    // Buffer阶段定义
    public enum CustomDisplayType
    {
        CUSTOM_ADD_HP = 0,				// 加HP	int hp
        CUSTOM_DEC_HP_WITH_EFFECT,		// 减HP+减HP的效果 int hp string effectid
        CUSTOM_ADD_EFFECT_TO_OBJ,		// 增加为一个对象增加效果 string effectid
        CUSTOM_ADD_EFFECT_TO_POS,		// 在某个坐标增加效果 float x,z string effectid

    };


    // Buffer清除原因
    public enum BUFFER_REMOVE
    {
        BUFFER_REMOVE_NONE = 0,
        BUFFER_REMOVE_BREAK,         // 打断
        BUFFER_REMOVE_REPLACE,       // 替换
        BUFFER_REMOVE_TIMEEND,       // 结束
        BUFFER_REMOVE_CLEAR,         //清除
        BUFFER_REMOVE_OFFLINE,       //下线清除
        BUFFER_REMOVE_DEAD,          //死亡清除
        BUFFER_REMOVE_OWNER,         //主清除子
        BUFFER_REMOVE_TIMEOVER,      //次数达到
    };
}