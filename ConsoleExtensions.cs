using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLink {
    public static class ConsoleExtensions {

        /// <summary>
        /// Writes a string in a color to the console
        /// </summary>
        /// <param name="s">String to write</param>
        /// <param name="c">Color</param>
        public static void WriteLine(string s, ConsoleColor? c) {
            if(c != null)
                Console.ForegroundColor = c.Value;
            Console.WriteLine(s);
            if(c != null)
                Console.ResetColor();
        }
    }
}
