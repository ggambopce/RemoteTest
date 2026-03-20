using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace remotetest
{
    /// <summary>
    /// API 에이전트 설정 - 정적 클래스
    /// </summary>
    public static class AgentConfig
    {
        public static string ApiBaseUrl =
            ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:8080";
        public static string AppVersion = "1.0.0";

        private const string LocalBoxKey = "LocalBoxId";

        /// <summary>
        /// 로컬 박스 ID 반환 (없으면 생성 후 app.config에 저장)
        /// MachineName + GUID 해시 기반으로 생성
        /// </summary>
        public static string GetOrCreateLocalBoxId()
        {
            string existing = ConfigurationManager.AppSettings[LocalBoxKey];
            if (!string.IsNullOrEmpty(existing))
                return existing;

            string raw = Environment.MachineName + Guid.NewGuid().ToString();
            byte[] hash;
            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

            string id = "box" + BitConverter.ToString(hash).Replace("-", "").Substring(0, 16).ToLower();

            // app.config에 영속
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add(LocalBoxKey, id);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
                // 저장 실패 시 메모리 내 값만 사용
            }

            return id;
        }
    }
}
