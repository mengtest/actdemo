using System;

namespace SysUtils
{
    public class BitUtils
    {
        public static bool MakeValue_16(ref ushort ret, ushort value, ushort begin, ushort end, bool bCheck = true)
        {
            // 一个 ushort 16 位
            if (begin > end || end >= 16)
                return false;

            ushort left_move = (ushort)(15 - (end - begin));
            ushort right_move = (ushort)(left_move - begin);

            if (bCheck)
            {
                if (value > Math.Pow(2, (end - begin + 1)))
                {
                    //Debug.WriteLine("value > Math.Pow(2, (end - begin)) Error");
                    return false;
                }

                //保证所在位置为0
                ushort temp = ret;

                temp = (ushort)(temp << right_move);
                temp = (ushort)(temp >> left_move);
                if (temp > 0)
                {
                    //Debug.WriteLine("temp > 0 Error");
                    return false;
                }
            }

            value = (ushort)(value << left_move);
            value = (ushort)(value >> right_move);

            ret |= value;

            return true;
        }

        public static ushort GetValue(ref ushort value, ushort begin, ushort end)
        {
            ushort ret = value;

            //  Math.Pow(2, 16) - 1 = 65535;  最大值
            ushort temp = 65535;

            ushort left_move = (ushort)(15 - end);
            ret = (ushort)(value << left_move);
            temp = (ushort)(temp << left_move);

            ushort right_move = (ushort)(left_move + begin);
            ret = (ushort)(ret >> right_move);

            temp = (ushort)(temp >> right_move);
            temp = (ushort)(temp << begin);
            value = (ushort)(value & (~temp));

            return ret;
        }
    }
}
