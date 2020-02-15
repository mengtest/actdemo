using System;
using System.Collections.Generic;
using System.Text;

namespace Demo_CSharp
{
    // 对象ID

    public struct PERSISTID
    {
        public UInt64 nData64;

        //public PERSISTID()
        //{
        //    nData64 = 0;
        //}

        public PERSISTID(UInt64 nData64)
        {
            this.nData64 = nData64;
        }

        public System.Boolean IsNull()
        {
            return 0 == nData64;
        }
    };

    //System.Boolean operator==(const ref PERSISTID source, const ref PERSISTID other)
    //{
    //    return source.nData64 == other.nData64;
    //}

    //System.Boolean operator!=(const PERSISTID& source, const PERSISTID& other)
    //{
    //    return source.nData64 != other.nData64;
    //}
}
