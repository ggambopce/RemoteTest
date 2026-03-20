using System;
using System.Windows.Forms;

namespace remotetest
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // vdesk:// URL 프로토콜 핸들러 등록 (최초 실행 시)
            UrlProtocolRegistrar.EnsureRegistered();

            bool isHostMode = Array.Exists(args, a =>
                a.Equals("/host", StringComparison.OrdinalIgnoreCase) ||
                a.Equals("--host", StringComparison.OrdinalIgnoreCase));

            // vdesk://IP:PORT 형태로 실행된 경우 (브라우저에서 "뷰어 실행" 클릭)
            string relayIp = null;
            int relayPort = 20020;
            if (args.Length > 0 && args[0].StartsWith("vdesk://", StringComparison.OrdinalIgnoreCase))
            {
                string hostPort = args[0].Substring("vdesk://".Length).TrimEnd('/');
                int colonIdx = hostPort.LastIndexOf(':');
                if (colonIdx > 0 && int.TryParse(hostPort.Substring(colonIdx + 1), out int parsedPort))
                {
                    relayIp = hostPort.Substring(0, colonIdx);
                    relayPort = parsedPort;
                }
                else
                {
                    relayIp = hostPort;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(isHostMode, relayIp, relayPort));
        }
    }
}
