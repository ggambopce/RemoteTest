using System.Windows.Forms;
using Microsoft.Win32;

namespace remotetest
{
    /// <summary>
    /// Windows 레지스트리에 vdesk:// URL 프로토콜 핸들러를 등록합니다.
    /// HKCU 를 사용하므로 관리자 권한 불필요.
    /// </summary>
    public static class UrlProtocolRegistrar
    {
        public static void EnsureRegistered()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\vdesk"))
                {
                    key.SetValue("", "URL:VDesk Remote Protocol");
                    key.SetValue("URL Protocol", "");

                    using (var icon = key.CreateSubKey("DefaultIcon"))
                        icon.SetValue("", exePath + ",1");

                    using (var open = key.CreateSubKey(@"shell\open\command"))
                        open.SetValue("", "\"" + exePath + "\" \"%1\"");
                }
            }
            catch { /* 레지스트리 쓰기 실패는 치명적이지 않음 */ }
        }
    }
}
