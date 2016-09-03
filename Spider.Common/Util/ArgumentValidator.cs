using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.Util
{
    public static class ArgumentValidator
    {
        public static void Validate<T>(string argName, T arg, Func<T, bool> predicate)
        {
            if (predicate!=null&&predicate(arg))
            {
                throw new ArgumentException(argName);
            }
        }
    }
}
