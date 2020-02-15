using System;

/// <summary>
/// 游戏常用数据管理类
/// </summary>
public class GameDefine
{
    /// <summary>
    /// 与服务器误差（Tick计算）
    /// </summary>
    static long compareTimeTicks = 0;
    static long serverEnterTicks = 0;                   //进入服务器的时候，服务器的64位时间戳
    static long clientTicks = 0;                        //进入服务器的时候，客户端的64位时间戳

    /// <summary>
    /// 是否同步时间
    /// </summary>
    public static bool SynTimer = false;

    /// <summary>
    /// 校正时间
    /// </summary>
    public static UserDelegate CorrectionTime = new UserDelegate();

    public static void UnInit()
    {
        SynTimer = false;
        CorrectionTime.ClearCalls();
    }

    /// <summary>
    /// 转换秒为具体的时间(1:1:12:21 天:时:分:秒)
    /// </summary>
    /// <param name="t">秒</param>
    /// <returns>天:时:分:秒</returns>
    public static string changeSecsToTime(int t)
    {
        string r = string.Empty;
        int day, hour, minute, second;

        day = Convert.ToInt16(t / 86400);
        hour = Convert.ToInt16((t % 86400) / 3600);
        minute = Convert.ToInt16((t % 86400 % 3600) / 60);
        second = Convert.ToInt16(t % 86400 % 3600 % 60);
        r = UtilTools.StringBuilder(string.Format("{0:00}", day), ":", string.Format("{0:00}", hour), ":", string.Format("{0:00}", minute), ":", string.Format("{0:00}", second));

        return r;
    }

    /// <summary>
    /// 转换秒为具体的时间(1:12:21 时:分:秒)
    /// </summary>
    /// <param name="t">秒</param>
    /// <returns>时:分:秒</returns>
    public static string ConvertSecsToHours(int t)
    {
        string r = string.Empty;
        int hour, minute, second;

        hour = Convert.ToInt16(t / 3600);
        minute = Convert.ToInt16((t % 3600) / 60);
        second = Convert.ToInt16(t % 3600 % 60);
        r = UtilTools.StringBuilder(string.Format("{0:00}", hour), ":", string.Format("{0:00}", minute), ":", string.Format("{0:00}", second));

        return r;
    }

    /// <summary>
    /// 转换时间戳为具体时间（时:分:秒）
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ConverLocSecsToTime(long t)
    {
        DateTime time = new DateTime(1970, 1, 1).AddSeconds(t);
        time = time.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(time).TotalHours);
        return UtilTools.StringBuilder(time.Hour.ToString("00"), ":", time.Minute.ToString("00"), ":", time.Second.ToString("00"));
    }

    /// <summary>
    /// 转换秒为具体的时间
    /// </summary>
    /// <returns></returns>
    public static string ConvertSecsToTime(int t)
    {
        DateTime time = new DateTime().AddSeconds(t);
        return UtilTools.StringBuilder(time.Minute.ToString("00"), ":", time.Second.ToString("00"));
    }

    /// <summary>
    /// 转换秒为具体时间（小时：分钟   01:01）
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ConvertSecsToMinute(long t)
    {
        DateTime time = new DateTime().AddSeconds(t);
        return UtilTools.StringBuilder(time.Hour.ToString("00"), ":", time.Minute.ToString("00"));
    }

    /// <summary>
    /// 转换秒为具体时间（年-月-日 2015-04-09）
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ConvertSecsToDay(long t)
    {
        DateTime time = new DateTime(1970, 1, 1).AddSeconds(t);
        time = time.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(time).TotalHours);
        return UtilTools.StringBuilder(time.Year, "-", time.Month.ToString("00"), "-", time.Day.ToString("00"));
    }

    /// <summary>
    /// 转换秒为具体的时间--增加时区信息
    /// </summary>
    /// <returns></returns>
    public static DateTime ConvertSecsToDateTime(long timestamp)
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
        return start.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(start).TotalHours);
    }

    /// <summary>
    /// 转换秒为具体的时间(12:21 分:秒)
    /// </summary>
    /// <param name="t">秒</param>
    /// <returns>时:分:秒</returns>
    public static string ConvertSecsToMinute(int t)
    {
        string r = string.Empty;
        int minute, second;
        minute = Convert.ToInt16((t % 3600) / 60);
        second = Convert.ToInt16(t % 3600 % 60);
        r = UtilTools.StringBuilder(string.Format("{0:00}", minute), ":", string.Format("{0:00}", second));
        return r;
    }
    /// <summary>
    /// 得到utc时间
    /// </summary>
    /// <returns></returns>
    public static long GetUTCTime()
    {
        TimeSpan sp = new TimeSpan(GetServerDataTime().Ticks);
        return (long)sp.TotalSeconds;
    }

    /// <summary>
    /// 获取服务器时间
    /// (不允许修改)
    /// </summary>
    /// <returns></returns>
    public static DateTime GetServerDataTime()
    {
        DateTime dt = new DateTime().AddTicks(DateTime.Now.Ticks + compareTimeTicks);
        return dt;
    }

    /// <summary>
    /// 获取本地时间（校验）
    /// </summary>
    /// <returns></returns>
    public static DateTime GetLocalDataTime()
    {
        DateTime dt = new DateTime(1970, 1, 1).AddTicks(DateTime.Now.Ticks + compareTimeTicks);
        dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).TotalHours);
        return dt;
    }

    /// <summary>
    /// 获取当天0点之后的总秒数
    /// </summary>
    /// <returns></returns>
    public static int GetLocalTotleSecond()
    {
        return GameDefine.GetLocalDataTime().Hour * 3600 + GameDefine.GetLocalDataTime().Minute * 60 + GameDefine.GetLocalDataTime().Second;
    }

    /// <summary>
    /// 获取总秒数(系统时间)
    /// </summary>
    /// <returns></returns>
    public static long GetTotleSeconds()
    {
        TimeSpan sp = new TimeSpan(GetServerDataTime().Ticks);
        return (long)sp.TotalSeconds;
    }

    /// <summary>
    /// 获取当前时间 时间戳 与服务端同步
    /// </summary>
    /// <returns></returns>
    //public static long GetLocalTicks()
    //{
    //    DateTime time = MathDateTime(compareTimeTicks);
    //    return time.Ticks;
    //}

    /// <summary>
    /// 时间换算（进行时区换算）;
    /// </summary>
    public DateTime MathDateTime(long milliseconds)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
        dt = dt.AddMilliseconds(milliseconds);
        dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).TotalHours);
        return dt;
    }

    /// <summary>
    /// 校正本地时间
    /// </summary>
    /// <param name="timer">服务器以秒计算的时间</param>
    public static void CorrectLocalTime(long timer , long pServerTicks)
    {
        DateTime serverTime = new DateTime().AddSeconds(timer);
        DateTime nowTime = DateTime.Now;
        TimeSpan compareTime = serverTime.Subtract(nowTime);
        compareTimeTicks = compareTime.Ticks;

        serverEnterTicks = pServerTicks;
        clientTicks = nowTime.Ticks / 10000;

        SynTimer = true;
        if (CorrectionTime != null)
            CorrectionTime.ExecuteCalls();
    }
    
    /// <summary>
    /// 获得服务器当前的ticks
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    public static long ServerNowTicks(long timer)
    {
        DateTime nowTime = DateTime.Now;
        return (serverEnterTicks + nowTime.Ticks / 10000 - clientTicks);
    }
}
