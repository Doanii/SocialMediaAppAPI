using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class GenerateString
    {
        private static Random random = new Random();

        public static string Generate(int length = 80)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
