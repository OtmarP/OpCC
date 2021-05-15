using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpCC
{
    class Program
    {
        // Info:
        // EnvironmentVariableTarget.Machine ... HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment 
        // EnvironmentVariableTarget.User ...... HKEY_CURRENT_USER\Environment 

        public const string _OPCC_TM_START = "OPCC_TM_START";
        public static string VersionNumber = "";

        static void Main(string[] args)
        {
            // ToDo: OpCC -wait:5

            VersionNumber = "1.0.15.1202";
            AddRDToVersion();

            // Check Parameter
            if (args.Length == 0)
            {
                DisplayNoParameter();
                return;
            }
            else
            {
                if (args[0] == "-?")
                {
                    DisplayHelp();
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
                else if (args[0] == "-time")
                {
                    TMStop(false);
                    return;
                }
                else if (args[0] == "-stop")
                {
                    TMStop(true);
                    return;
                }

                // next...

                DisplayWrongParameter();
            }
        }

        public static void AddRDToVersion()
        {
#if DEBUG
            VersionNumber += ".D";
#else
            VersionNumber+=".R";
#endif
        }

        public static void DisplayNoParameter()
        {
            // Display Help
            DisplayHeaderAndHelp(true, "Warning: no Parameter.", true);

            System.Environment.Exit(0);
            return;
        }

        public static void DisplayHelp()
        {
            // Display Help
            DisplayHeaderAndHelp(true, "", true);

            System.Environment.Exit(0);
            return;
        }

        public static void DisplayWrongParameter()
        {
            DisplayHeaderAndHelp(true, "Warning: wrong Parameter.", false);

            System.Environment.Exit(1);
            return;
        }

        public static void DisplayHeaderAndHelp(bool dispHeader, string message, bool dispHelp)
        {
            if (dispHeader)
            {
                Console.WriteLine("OpCC v:" + VersionNumber);
            }
            Console.WriteLine(message);
            if (dispHelp)
            {
                Console.WriteLine("OpCC [-?] (Help)");
                Console.WriteLine("OpCC [-start|-time|-stop] ... TM - TimeMark, NCC - Norton Control Center");
                Console.WriteLine("OpCC [-clear] ............... Clear all EnvironmentVariables");
            }
        }

        public static void ClearEnvironmentVariables()
        {
            string envName;
            string envValue;

            envName = "OPCC_TM_START";
            envValue = System.Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(envValue))
            {
                Console.WriteLine("clear EnvironmentVariable: " + envName);
                System.Environment.SetEnvironmentVariable(envName, "", EnvironmentVariableTarget.User);
            }

            envName = "OPCC_TM";
            envValue = System.Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(envValue))
            {
                Console.WriteLine("clear EnvironmentVariable: " + envName);
                System.Environment.SetEnvironmentVariable(envName, "", EnvironmentVariableTarget.User);
            }

            Console.WriteLine("clear all EnvironmentVariables... OK.");

            System.Environment.Exit(0);
            return;
        }

        public static void TMStart()
        {
            // save Time
            long startTicks = DateTime.Now.Ticks;
            DateTime startTime = new DateTime(startTicks);
            string startString = startTime.ToString("ddd.dd.MM.yyyy HH:mm:ss");
            System.Environment.SetEnvironmentVariable(_OPCC_TM_START, startTicks.ToString(), EnvironmentVariableTarget.User);
            Console.WriteLine("Start: " + startTime);

            System.Environment.Exit(0);
            return;
        }

        public static void TMStop(bool stop)
        {
            // calc Time-diff and display
            string startTicksString = System.Environment.GetEnvironmentVariable(_OPCC_TM_START, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(startTicksString))
            {
                Console.WriteLine("Warning: no -start.");

                System.Environment.Exit(1);
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
                    System.Environment.SetEnvironmentVariable(_OPCC_TM_START, "", EnvironmentVariableTarget.User);
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

                System.Environment.Exit(0);
                return;
            }
        }
    }
}
