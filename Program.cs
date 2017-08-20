using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicLink {
    class Program {

        #region Private functions for command line

        /// <summary>
        /// Parse command line parameter
        /// </summary>
        /// <param name="s">Command line parameter</param>
        /// <param name="bSwitch">True, if the parameter is a switch, otherwise false</param>
        /// <returns></returns>
        private static string GetArg(string s, out bool bSwitch) {
            if(string.IsNullOrEmpty(s)) {
                bSwitch = false;
                return s;
            } else switch(s[0]) {
                    case '-':
                    case '/':
                        bSwitch = true;
                        return s.Substring(1).ToLower();
                    default:
                        bSwitch = false;
                        return s;
                }
        }

        private static string GetArg(string s) {
            bool b;
            return GetArg(s, out b);
        }

        private static bool UnknownArg(string s) {
#if DEBUG
            if(GetArg(s).FirstOrDefault() != '!') {                     // Ignore 'bang-ed' command line switches during debugging
#endif
                Trace.TraceError($"Unknown command line parameter '{s}'.");
                Environment.ExitCode = 1;
                return true;
#if DEBUG
            } else
                return false;
#endif
        }

        private static bool ArgOutOfRange(string s) {
#if DEBUG
            if(GetArg(s).FirstOrDefault() != '!') {                     // Ignore 'bang-ed' command line switches during debugging
#endif
                Trace.TraceError($"Command line parameter '{s}' out of range.");
                Environment.ExitCode = 1;
                return true;
#if DEBUG
            } else
                return false;
#endif
        }

        #endregion

        #region Private functions for NIM

        /// <summary>
        /// Shows the playground of the NIM game. Each heap is displayed by it's object count.
        /// </summary>
        /// <param name="nim">NIM game</param>
        /// <param name="bBinary">Displays a binary representation of the heap's object count.</param>
        private static void ShowPlayground(NIM nim, bool bXor = false, bool bBinary = false, bool bColor = false) {
            if(bColor)
                Console.ForegroundColor = ConsoleColor.Yellow;

            if(bBinary) {
                int r = 1;
                if(nim.Playground.Max() > 0xFF) {
                    foreach(int i in nim.Playground)
                        Console.WriteLine($"{r++,3}: {Convert.ToString(i >> 8, 2).PadLeft(8, '0')} {Convert.ToString(i & 0xFF, 2).PadLeft(8, '0')} = {i,5}");
                    if(bXor) {
                        Console.WriteLine($"     -------- --------");
                        Console.WriteLine($"XOR: {Convert.ToString(nim.Xor() >> 8, 2).PadLeft(8, '0')} {Convert.ToString(nim.Xor() & 0xFF, 2).PadLeft(8, '0')} = {nim.Xor(),5}");
                    } else
                        Console.WriteLine();
                } else {
                    foreach(int i in nim.Playground)
                        Console.WriteLine($"{r++,3}: {Convert.ToString(i, 2).PadLeft(8, '0')} = {i,5}");
                    if(bXor) {
                        Console.WriteLine($"     --------");
                        Console.WriteLine($"XOR: {Convert.ToString(nim.Xor(), 2).PadLeft(8, '0')} = {nim.Xor(),5}");
                    } else
                        Console.WriteLine();
                }
            } else {
                foreach(int i in nim.Playground)
                    Console.Write($" {i,5}");
                if(bXor)
                    Console.WriteLine($" XOR {nim.Xor(),5}");
                else
                    Console.WriteLine();
            }

            if(bColor)
                Console.ResetColor();
        }

        private static void ShowMoves(NIM nim, bool bColor = false) {
            if(bColor)
                Console.ForegroundColor = ConsoleColor.DarkCyan;

            IEnumerable<Move> ms = nim.GetMoves();
            if(ms.Count() == 0)
                Console.WriteLine($"\t No more optimal move possible");
            else
                foreach(Move m in nim.GetMoves())
                    Console.WriteLine($"\t Move {m} possible");

            if(bColor)
                Console.ResetColor();
        }

        private static void PlayRandom(NIM nim, bool bColor = false) {
            int i = 0;
            Random rnd = new Random();
            while(!nim.EndOfGame) {
                Move m = nim.Play(rnd);
                Console.WriteLine(m);
                nim.Apply(m);
                ShowPlayground(nim, bColor: bColor);
                ShowPlayground(nim, true, bColor: bColor);
                i++;
            }
            ConsoleExtensions.WriteLine($"END OF GAME - {((i & 1) == 1 ? "1st" : "2nd")} player wins.", bColor ? ConsoleColor.Red : default(ConsoleColor));
        }

        private static void PlaySelf(NIM nim, bool bColor = false) {
            int i = 0;
            while(!nim.EndOfGame) {
                Move m = nim.Play();
                Console.WriteLine(m);
                nim.Apply(m);
                ShowPlayground(nim);
                i++;
            }
            ConsoleExtensions.WriteLine($"END OF GAME - {((i & 1)==1? "1st" : "2nd")} player wins.", bColor ? ConsoleColor.Red : default(ConsoleColor));
        }
        #endregion

        /// <summary>
        /// Show help
        /// </summary>
        private static void Help() {
            AssemblyProductAttribute atProd = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyProductAttribute>();
            if(atProd != null)
                Console.Write(atProd.Product);
            Console.WriteLine(" Version {0}", Assembly.GetCallingAssembly().GetName().Version.ToString());

            AssemblyDescriptionAttribute atDesc = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>();
            if(atProd != null)
                Console.WriteLine(atDesc.Description);

            Console.WriteLine($"Usage:");
            Console.WriteLine($"{Assembly.GetCallingAssembly().GetName().Name} [N N N...] [-f][-s,-l][-u M][-m][-v,-vv,-vvv][-h]");
            Console.WriteLine($"Parameter:");
            Console.WriteLine($" N N N... Number of elements in heap.");
            Console.WriteLine($" -f       First move by computer.");
            Console.WriteLine($" -s       Smallest move by computer.");
            Console.WriteLine($" -l       Largest move by computer (default).");
            Console.WriteLine($" -u M     M is the upper limit of elements to be removed.");
            Console.WriteLine($" -m       Misère game in which last element taken looses.");
            Console.WriteLine($" -v       Verbose output with playground.");
            Console.WriteLine($" -vv      Verbose output with playground and XOR value.");
            Console.WriteLine($" -vvv     Verbose output with playground in binary format and XOR value.");
            Console.WriteLine($" -vvvv    Verbose output with playground in binary format and XOR value including possible moves.");
            Console.WriteLine($" -c       Colorized output.");
            Console.WriteLine($" -h       Help for the command.");
            Console.WriteLine($"After the start of the command you have to enter your moves in [Heap]:[Count] format.");
            Console.WriteLine($"[Heap] represents the number of the heap and [Count] the number of objects taken.");
            Console.WriteLine($"You get a hint by entering a question mark instead of a move");

            AssemblyCopyrightAttribute atCpy = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>();
            if(atCpy != null) {
                Console.Write(atCpy.Copyright);
                AssemblyCompanyAttribute atCmp = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>();
                if(atCmp != null)
                    Console.WriteLine(" by {0}", atCmp.Company);
            } else {
                AssemblyCompanyAttribute atCmp = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>();
                if(atCmp != null)
                    Console.WriteLine(" by {0}", atCmp.Company);
            }
        }

        static void Main(string[] args) {
            NIM nim = new NIM();

            bool bFirst = false;

            bool bVerbose = false;
            bool bVerboseXor = false;
            bool bVerboseBinary = false;
            bool bVerboseMoves = false;

            bool bColor = false;

            bool bHelp = false;

#if !DEBUG
            // Add ConsoleTraceListener with output to standard error stream
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener(true));
#endif

            // Parse command line switches
            for(int i = 0; i < args.Length; i++) {
                switch(GetArg(args[i])) {

                    // Logger settings and operations

                    case "u":
                        int iUpper;
                        if(i + 1 < args.Length && int.TryParse(args[++i], out iUpper) && iUpper > 0)
                            nim.Upper = iUpper;
                        else {
                            Trace.TraceError("Upper limit of elements to be removed is illegal or missing.");
                            Environment.ExitCode = 1;
                            bHelp = true;
                        }
                        break;

                    case "f":
                        bFirst = true;
                        break;

                    case "s":
                        nim.Strategy = Strategy.Smallest;
                        break;

                    case "l":
                        nim.Strategy = Strategy.Largest;
                        break;

                    case "m":
                        nim.Misère = true;
                        break;

                    case "v":
                        bVerbose = true;
                        break;

                    case "vv":
                        bVerboseXor = true;
                        break;

                    case "vvv":
                        bVerboseXor = true;
                        bVerboseBinary = true;
                        break;

                    case "vvvv":
                        bVerboseXor = true;
                        bVerboseBinary = true;
                        bVerboseMoves = true;
                        break;

                    case "c":
                        bColor = true;
                        break;

                    case "h":
                        bHelp = true;
                        break;

                    default:
                        int iCount;
                        if(int.TryParse(GetArg(args[i]), out iCount)) {
                            if(iCount > short.MaxValue)
                                bHelp = ArgOutOfRange(args[i]);
                            else
                                nim.Playground.Add(iCount);
                        } else
                            bHelp = UnknownArg(args[i]);
                        break;
                }
            }

            if(bHelp) {
                Help();
#if DEBUG
                Console.ReadKey();
#endif
                return;
            }

            DateTime dt = DateTime.Now;

            if(bVerbose)
                ShowPlayground(nim, bVerboseXor, bColor: bColor);
            if(bVerboseBinary)
                ShowPlayground(nim, bVerboseXor, true, bColor);
            if(bVerboseMoves)
                ShowMoves(nim, bColor);

            Move m; 
            if(bFirst) {                                                // First move by computer

                // Computer's move
                m = nim.Play();
                ConsoleExtensions.WriteLine($"MY MOVE    ([Heap]:[Count]) {m}",  bColor ? ConsoleColor.Cyan : default(ConsoleColor));
                nim.Apply(m);

                if(bVerbose)
                    ShowPlayground(nim, bVerboseXor, bColor: bColor);
                if(bVerboseBinary)
                    ShowPlayground(nim, bVerboseXor, true, bColor);
                if(bVerboseMoves)
                    ShowMoves(nim, bColor);
                if(nim.EndOfGame)
                    ConsoleExtensions.WriteLine($"YOU {(nim.Misère ? "WIN" : "LOST")} AFTER {(DateTime.Now - dt).ToString(@"hh\:mm\:ss")}", bColor ? ConsoleColor.Red : default(ConsoleColor));

            }

            while(!nim.EndOfGame) {                                     // Loop to moves by user

                // User input
                if(bColor)
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("YOUR MOVE? ([Heap]:[Count]) ");
                string s = Console.ReadLine();
                if(bColor)
                    Console.ResetColor();

                if(s == "?") {

                    // Hint for user
                    ShowMoves(nim, bColor);

                } else if(Move.TryParse(s, out m)) {

                    // User move and subseqent move by computer
                    try {

                        // User's move
                        nim.Apply(m);

                        if(bVerbose)
                            ShowPlayground(nim, bVerboseXor, bColor: bColor);
                        if(bVerboseBinary)
                            ShowPlayground(nim, bVerboseXor, true, bColor);
                        if(bVerboseMoves)
                            ShowMoves(nim, bColor);
                        if(nim.EndOfGame) {
                            ConsoleExtensions.WriteLine($"YOU {(nim.Misère ? "LOST" : "WIN")} AFTER {(DateTime.Now - dt).ToString(@"hh\:mm\:ss")}", bColor ? ConsoleColor.Red : default(ConsoleColor));

                        } else {

                            // Computer's move
                            m = nim.Play();
                            ConsoleExtensions.WriteLine($"MY MOVE    ([Heap]:[Count]) {m}",  bColor ? ConsoleColor.Cyan : default(ConsoleColor));
                            nim.Apply(m);

                            if(bVerbose)
                                ShowPlayground(nim, bVerboseXor, bColor: bColor);
                            if(bVerboseBinary)
                                ShowPlayground(nim, bVerboseXor, true, bColor);
                            if(bVerboseMoves)
                                ShowMoves(nim, bColor);
                            if(nim.EndOfGame)
                                ConsoleExtensions.WriteLine($"YOU {(nim.Misère ? "WIN" : "LOST")} AFTER {(DateTime.Now - dt).ToString(@"hh\:mm\:ss")}", bColor ? ConsoleColor.Red : default(ConsoleColor));
                        }
                    } catch(Exception ex) {
                        Trace.TraceError(ex.Message);
                    }
                } else
                    Trace.TraceError("Invalid input format");
            }

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
