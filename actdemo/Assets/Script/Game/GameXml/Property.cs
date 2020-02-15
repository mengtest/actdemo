using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TinyBinaryXml;
using SysUtils;

/// <summary>
/// 种族类型
/// </summary>
public enum RaceType
{
    RaceType_Unknown = 0,

    RaceType_Human      = 1,    // 人类
    RaceType_Elf        = 2,    // 精灵
    RaceType_Undead     = 3,    // 亡灵
    RaceType_Animal     = 4,    // 动物
    RaceType_Orc        = 5,    // 兽人

    RaceType_MaxCount,
}

public class Property
{
    public string mPropName;
    public int mVarType;
    public Var mValue;
}