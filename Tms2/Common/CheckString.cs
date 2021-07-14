using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tms2.Common
{
    public static class CheckString
    {
        public static string NotNullOrEmpty(this string s)
        {
            if (s == null || s.Trim() == "")
            {
                return "?";
            }
            return s;
        }
    }
}
