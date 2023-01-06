using ConsoleCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JimmikerNetwork;
using JimmikerNetwork.Client;

namespace WarFightGameServer
{
    public class Program
    {
        public static bool stop { get; private set; } = false;
        public static Appllication appllication { get; private set; }

        public static ClientToLoginServer client { get; private set; }

        static void Help()
        {
            Console.Error.WriteLine(
                                "Usage: WarFightGameServer [options...]\n" +
                                " -h                           info for command\n" +
                                " -p <port>                    server port\n" +
                                " --SetServerIP <ipaddress>    set server ip\n" +
                                " --LoginServerHost <host>     Login Server Host Ex: --LoginServerHost www.chummydns.com\n" +
                                " --LoginServerPort <port>     Login Server Port Ex: --LoginServerPort 5800\n"
                                );
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            int port = 5810;
            string serverip = null;
            string loginserverhost = null;
            int loginserverport = 5800;

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
                    case "--SetServerIP":
                        {
                            i++;
                            serverip = args[i];
                        }
                        break;
                    case "--LoginServerHost":
                        {
                            i++;
                            loginserverhost = args[i];
                            break;
                        }
                    case "--LoginServerPort":
                        {
                            i++;
                            loginserverport = Convert.ToInt32(args[i]);
                            break;
                        }
                    case "-h":
                        {
                            Help();
                            return;
                        }
                }
            }

            if (loginserverhost == null) Help();

            appllication = new Appllication(serverip, port, System.Net.Sockets.ProtocolType.Udp);
            appllication.GetMessage += Appllication_GetMessage;
            appllication.Start();

            client = new ClientToLoginServer(System.Net.Sockets.ProtocolType.Tcp);
            client.Connect(loginserverhost, loginserverport);

            ConsoleCtrl.ConsoleCtrl consoleCtrl = (Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX) || ((int)Environment.OSVersion.Platform == 128) ? (ConsoleCtrl.ConsoleCtrl)new UnixConsoleCtrl() : (ConsoleCtrl.ConsoleCtrl)new WinConsoleCtrl();

            consoleCtrl.OnExit += (sender, e) =>
            {
                Console.Error.WriteLine("Closing...");
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
