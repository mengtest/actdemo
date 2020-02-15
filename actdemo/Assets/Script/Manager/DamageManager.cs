using UnityEngine;
using System.Collections;

public class DamageManager
{
    /// <summary>
    /// 计算普攻伤害
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    public static float GetDamageByAttack(IObject attacker, IObject defender)
    {
        if (attacker == null || defender == null)
        {
            return 0.0f;
        }

        string formula = XmlManager.Instance.GetFormula("1001");
        if (string.IsNullOrEmpty(formula))
        {
            return 0.0f;
        }
        
        float damage = FormulaParserManager.Instance.CalcFormula(attacker, defender, formula);
        if (damage < 0.0f)
        {
            damage = 0.0f;
        }

        return damage;
    }
}