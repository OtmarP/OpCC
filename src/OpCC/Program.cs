using OpLib;
using System;
using System.Threading;

namespace OpCC
{
    class Program
    {
        // Info:
        // EnvironmentVariableTarget.Machine ... HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment 
        // EnvironmentVariableTarget.User ...... HKEY_CURRENT_USER\Environment 

        public const string _OPCC_TM_START = "OPCC_TM_START";

        public static bool _launchedFromStudio;

        static void Main(string[] args)
        {
            string version = "1.0.18.321";    // "1.0.15.1202";
            bool flag = Helper.CheckLaunchedFromStudio();
            _launchedFromStudio = flag;
            version = Helper.AddRDToVersion(version);

            // Check Parameter
            if (args.Length == 0)
            {
                DisplayNoParameter(version);
                return;
            }
            if (args[0] == "-?")
            {
                DisplayHelp(version);
                return;
            }

            // -clear
            if (args[0] == "-clear")
            {
                ClearEnvironmentVariables();
                return;
            }

            // -start, -time, -stop
            if (args[0] == "-start")
            {
                TMStart();
                return;
            }
            if (args[0] == "-time")
            {
                TMStop(false);
                return;
            }
            if (args[0] == "-stop")
            {
                TMStop(true);
                return;
            }

            // OpCC -wait:5
            if (args[0].StartsWith("-wait:"))
            {
                WaitSeconds(args[0]);
                return;
            }
            if (!args[0].StartsWith("-display:"))
            {
                WaitInIDE(flag);
                DisplayWrongParameter(version);
                return;
            }
            string str = "";
            if ((int)args.Length == 2)
            {
                str = args[1];
            }
            DisplayText(args[0], str);
        }

        public static void DisplayNoParameter(string versionNumber)
        {
            // Display Help
            Console.ForegroundColor = ConsoleColor.Red;
            DisplayHeaderAndHelp(true, versionNumber, "Warning: no Parameter.", true);
            Console.ResetColor();
            WaitInIDE(_launchedFromStudio);
            Environment.Exit(0);
        }

        public static void DisplayHelp(string versionNumber)
        {
            // Display Help
            DisplayHeaderAndHelp(true, versionNumber, "", true);
            //WaitInIDE(_launchedFromStudio);
            Environment.Exit(0);
        }

        public static void DisplayWrongParameter(string versionNumber)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            DisplayHeaderAndHelp(true, versionNumber, "Warning: wrong Parameter.", false);
            Console.ResetColor();
            //WaitInIDE(_launchedFromStudio);
            Environment.Exit(1);
        }

        public static void DisplayHeaderAndHelp(bool dispHeader, string versionNumber, string message, bool dispHelp)
        {
            if (dispHeader)
            {
                Console.WriteLine(string.Concat("OpCC v:", versionNumber));
            }
            Console.WriteLine(message);
            if (dispHelp)
            {
                Console.WriteLine("OpCC -? .................... Display Help");
                Console.WriteLine("OpCC -start|-time|-stop .... TM - TimeMark, NCC - Norton Control Center");
                Console.WriteLine("OpCC -clear ................ Clear all EnvironmentVariables");
                Console.WriteLine("OpCC -wait:5 ............... Waits for 5 Seconds");
                Console.WriteLine("OpCC -display:Text -c:0C ... Display 'Text' in Color Red");
                Console.WriteLine("OpCC -display:Text -c:0A ... Display 'Text' in Color Green");
                Console.WriteLine("                               -c:BF the first corresponds to the background,");
                Console.WriteLine("                                     the second the foreground");
                Console.WriteLine("                               0 = Black       8 = Gray");
                Console.WriteLine("                               1 = Blue        9 = Light Blue");
                Console.WriteLine("                               2 = Green       A = Light Green");
                Console.WriteLine("                               3 = Aqua        B = Light Aqua");
                Console.WriteLine("                               4 = Red         C = Light Red");
                Console.WriteLine("                               5 = Purple      D = Light Purple");
                Console.WriteLine("                               6 = Yellow      E = Light Yellow");
                Console.WriteLine("                               7 = White       F = Bright White");
            }
        }

        public static void ClearEnvironmentVariables()
        {
            string envName;
            string envValue;

            envName = "OPCC_TM_START";
            envValue = Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(envValue))
            {
                Console.WriteLine("clear EnvironmentVariable: " + envName);
                Environment.SetEnvironmentVariable(envName, "", EnvironmentVariableTarget.User);
            }

            envName = "OPCC_TM";
            envValue = Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(envValue))
            {
                Console.WriteLine("clear EnvironmentVariable: " + envName);
                Environment.SetEnvironmentVariable(envName, "", EnvironmentVariableTarget.User);
            }

            Console.WriteLine("clear all EnvironmentVariables... OK.");

            Environment.Exit(0);
            return;
        }

        public static void TMStart()
        {
            // save Time
            long startTicks = DateTime.Now.Ticks;
            DateTime startTime = new DateTime(startTicks);
            string startString = startTime.ToString("ddd.dd.MM.yyyy HH:mm:ss");
            Environment.SetEnvironmentVariable(_OPCC_TM_START, startTicks.ToString(), EnvironmentVariableTarget.User);
            Console.WriteLine("Start: " + startTime);

            Environment.Exit(0);
            return;
        }

        public static void TMStop(bool stop)
        {
            // calc Time-diff and display
            string startTicksString = Environment.GetEnvironmentVariable(_OPCC_TM_START, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(startTicksString))
            {

                Console.WriteLine("Warning: no -start.");


                Environment.Exit(1);
                return;
            }
            else
            {
                long startTicks = Convert.ToInt64(startTicksString);
                DateTime startTime = new DateTime(startTicks);
                string startString = startTime.ToString("ddd.dd.MM.yyyy HH:mm:ss");

                // clear
                if (stop)
                {
                    Environment.SetEnvironmentVariable(_OPCC_TM_START, "", EnvironmentVariableTarget.User);
                }

                long stopTicks = DateTime.Now.Ticks;
                //string stopTime = DateTime.Now.ToString();
                DateTime stopTime = new DateTime(stopTicks);
                string stopString = stopTime.ToString("ddd.dd.MM.yyyy HH:mm:ss");

                //
                //startTime = new DateTime( 2015, 12, 1, 11, 1, 1, 101 );
                //stopTime = new DateTime( 2015, 12, 2, 11, 1, 1, 100);
                TimeSpan timespan = (stopTime - startTime);

                //TimeSpan durationTime = "";

                string formatString = @"hh\:mm\:ss\.fff";

                string postString = timespan.TotalHours.ToString("#,##0.00") + " Std";
                if (timespan.TotalMilliseconds >= 1)
                {
                    formatString = @"s\.fff";

                    postString = timespan.TotalMilliseconds.ToString("#,##0") + " MSec";
                }
                if (timespan.TotalSeconds >= 1)
                {
                    formatString = @"ss\.fff";

                    postString = timespan.TotalSeconds.ToString("#,##0.00") + " Sec";
                }
                if (timespan.TotalMinutes >= 1)
                {
                    formatString = @"mm\:ss\.fff";

                    postString = timespan.TotalMinutes.ToString("#,##0.00") + " Min";
                }
                if (timespan.TotalHours >= 1)
                {
                    formatString = @"hh\:mm\:ss\.fff";

                    postString = timespan.TotalHours.ToString("#,##0.00") + " Std";
                }
                if (timespan.TotalHours >= 24)
                {
                    formatString = @"dd\.hh\:mm\:ss\.fff";

                    postString = timespan.TotalHours.ToString("#,##0.00") + " Std";
                }
                string durationString = timespan.ToString(formatString);

                Console.WriteLine("Start-Stop: " + startString + " - " + stopString);
                Console.WriteLine("Duration: " + durationString + " (" + postString + ")");

                Environment.Exit(0);
                return;
            }
        }

        public static void WaitInIDE(bool launchedFromStudio)
        {
            if (launchedFromStudio)
            {
                Console.WriteLine("");
                Console.WriteLine("[DeveloperMode] weiter mit einer Taste...");
                Console.ReadKey();
            }
        }

        public static void WaitSeconds(string para)
        {
            string[] strArrays = para.Split(new char[] { ':' });
            int num = 5;
            if ((int)strArrays.Length >= 2)
            {
                try
                {
                    num = Convert.ToInt32(strArrays[1]);
                }
                catch (Exception exception)
                {
                }
            }
            Console.Write(string.Format("Wait {0} Seconds: ", num));
            for (int i = 0; i < num; i++)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.WriteLine("OK");
            WaitInIDE(_launchedFromStudio);
            Environment.Exit(0);
        }

        public static void DisplayText(string para, string color)
        {
            string str = "";
            string upper = "";
            string[] strArrays = para.Split(new char[] { ':' });
            if ((int)strArrays.Length >= 2)
            {
                for (int i = 1; i < (int)strArrays.Length; i++)
                {
                    if (i >= 2)
                    {
                        str = string.Concat(str, ":");
                    }
                    str = string.Concat(str, strArrays[i]);
                }
            }
            str = str.Replace("\\\\r", "\r");
            str = str.Replace("\\\\n", "\n");
            str = str.Replace("\\\\r\\n", "\r\n");
            str = str.Replace("\\\\t", "\t");
            string[] strArrays1 = color.Split(new char[] { ':' });
            if ((int)strArrays1.Length >= 2)
            {
                upper = strArrays1[1];
            }
            upper = upper.ToUpper();
            int? coloFromHexDigit = null;
            int? nullable = null;
            if (upper != "")
            {
                if (upper.Length == 1)
                {
                    nullable = GetColoFromHexDigit(upper);
                }
                else if (upper.Length == 2)
                {
                    string str1 = upper.Substring(0, 1);
                    string str2 = upper.Substring(1, 1);
                    coloFromHexDigit = GetColoFromHexDigit(str1);
                    nullable = GetColoFromHexDigit(str2);
                }
            }
            if (coloFromHexDigit.HasValue)
            {
                Console.BackgroundColor = (ConsoleColor)coloFromHexDigit.Value;
            }
            if (nullable.HasValue)
            {
                Console.ForegroundColor = (ConsoleColor)nullable.Value;
            }
            Console.WriteLine(str);
            Console.ResetColor();
            WaitInIDE(_launchedFromStudio);
            Environment.Exit(0);
        }

        public static int? GetColoFromHexDigit(string dispColorDigit)
        {
            int? nullable = null;
            if (dispColorDigit == "0")
            {
                nullable = new int?(0);
            }
            if (dispColorDigit == "1")
            {
                nullable = new int?(1);
            }
            if (dispColorDigit == "2")
            {
                nullable = new int?(2);
            }
            if (dispColorDigit == "3")
            {
                nullable = new int?(3);
            }
            if (dispColorDigit == "4")
            {
                nullable = new int?(4);
            }
            if (dispColorDigit == "5")
            {
                nullable = new int?(5);
            }
            if (dispColorDigit == "6")
            {
                nullable = new int?(6);
            }
            if (dispColorDigit == "7")
            {
                nullable = new int?(7);
            }
            if (dispColorDigit == "8")
            {
                nullable = new int?(8);
            }
            if (dispColorDigit == "9")
            {
                nullable = new int?(9);
            }
            if (dispColorDigit == "A")
            {
                nullable = new int?(10);
            }
            if (dispColorDigit == "B")
            {
                nullable = new int?(11);
            }
            if (dispColorDigit == "C")
            {
                nullable = new int?(12);
            }
            if (dispColorDigit == "D")
            {
                nullable = new int?(13);
            }
            if (dispColorDigit == "E")
            {
                nullable = new int?(14);
            }
            if (dispColorDigit == "F")
            {
                nullable = new int?(15);
            }
            return nullable;
        }
    }
}
