using ConsoleCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarFightLoginServer
{
    public class Program
    {
        static bool stop = false;
        public static Appllication appllication { get; private set; }

        static void Help()
        {
            Console.WriteLine(
                                "Usage: WarFightLoginServer [options...]\n" +
                                " -h                           info for command\n" +
                                " -p <port>                    server port\n"
                                );
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            int port = 5800;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-p":
                        {
                            i++;
                            port = Convert.ToInt32(args[i]);
                            break;
                        }
                    case "-h":
                        {
                            Help();
                            return;
                        }
                }
            }


            appllication = new Appllication(port, System.Net.Sockets.ProtocolType.Tcp);
            appllication.GetMessage += Appllication_GetMessage;
            appllication.Start();

            ConsoleCtrl.ConsoleCtrl consoleCtrl = (Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX) || ((int)Environment.OSVersion.Platform == 128) ? (ConsoleCtrl.ConsoleCtrl)new UnixConsoleCtrl() : (ConsoleCtrl.ConsoleCtrl)new WinConsoleCtrl();

            consoleCtrl.OnExit += (sender, e) =>
            {
                Console.WriteLine("Closing...");
                appllication.Disconnect();
                SpinWait.SpinUntil(() => stop);
            };

            SpinWait.SpinUntil(() => Console.ReadLine() == "exit");
            appllication.StopUpdateThread();
        }

        private static void Appllication_GetMessage(Appllication.MessageType type, string message)
        {
            if (type == Appllication.MessageType.ServerClose)
            {
                stop = true;
                appllication.GetMessage -= Appllication_GetMessage;
                appllication = null;
            }
            Console.Error.WriteLine(type.ToString() + ": " + message);
        }
    }
}
