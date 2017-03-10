using System;
using System.Collections.Generic;
using System.Text;

namespace Mast.DBUtility
{
    public class DbTypeConvert
    {
        static decimal ToDecimal(int value)
        {
            return Convert.ToDecimal(value);
        }
    }
}
