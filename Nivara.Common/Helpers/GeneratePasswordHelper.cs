using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Common.Helpers
{
    public static class GeneratePasswordHelper
    {
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz";
            const string valid1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string valid2 = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
                res.Append(valid1[rnd.Next(valid1.Length)]);
                res.Append(valid2[rnd.Next(valid2.Length)]);
            }
            res.Append('@');
            return res.ToString();
        }
    }
}
