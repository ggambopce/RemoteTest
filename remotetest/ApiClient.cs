using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace remotetest
{
    /// <summary>
    /// Spring Boot API 클라이언트 - 정적 클래스 (모든 메서드 동기/블로킹)
    /// </summary>
    public static class ApiClient
    {
        private static readonly HttpClient _http = new HttpClient();
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        public static string BaseUrl { get; set; } = AgentConfig.ApiBaseUrl;
        public static string DeviceKey { get; private set; }

        /// <summary>
        /// POST /api/host/register → DeviceKey 저장
        /// </summary>
        public static void Register(string localBox, string hostName,
                                    string osType, string appVersion, string relayIp)
        {
            var body = new Dictionary<string, string>
            {
                { "localBox",   localBox   },
                { "hostName",   hostName   },
                { "osType",     osType     },
                { "appVersion", appVersion },
                { "relayIp",    relayIp    }
            };

            var response = Post("/api/host/register", body);
            if (response != null && response.ContainsKey("data"))
            {
                var data = response["data"] as Dictionary<string, object>;
                if (data != null && data.ContainsKey("deviceKey"))
                    DeviceKey = data["deviceKey"]?.ToString();
            }
        }

        /// <summary>
        /// POST /api/host/heartbeat
        /// </summary>
        public static void Heartbeat()
        {
            if (string.IsNullOrEmpty(DeviceKey)) return;
            Post("/api/host/heartbeat", new Dictionary<string, string>
            {
                { "deviceKey", DeviceKey },
                { "relayIp",   NetworkInfo.DefaultIP }
            });
        }

        /// <summary>
        /// POST /api/agent/sessions/poll → null이면 대기 세션 없음
        /// </summary>
        public static AgentPollResult PollForSession()
        {
            if (string.IsNullOrEmpty(DeviceKey)) return null;

            var response = Post("/api/agent/sessions/poll", new Dictionary<string, string>
            {
                { "deviceKey", DeviceKey }
            });

            if (response == null || !response.ContainsKey("data")) return null;

            var data = response["data"] as Dictionary<string, object>;
            if (data == null) return null;

            bool has = data.ContainsKey("hasPendingSession") &&
                       Convert.ToBoolean(data["hasPendingSession"]);
            if (!has) return null;

            var result = new AgentPollResult
            {
                HasPendingSession = true,
                SessionKey        = data.ContainsKey("sessionKey") ? data["sessionKey"]?.ToString() : null,
                Status            = data.ContainsKey("status") ? data["status"]?.ToString() : null
            };
            if (data.ContainsKey("sessionId") && data["sessionId"] != null)
                result.SessionId = Convert.ToInt64(data["sessionId"]);

            return result;
        }

        /// <summary>
        /// POST /api/agent/sessions/activate
        /// </summary>
        public static void ActivateSession(string sessionKey)
        {
            Post("/api/agent/sessions/activate", new Dictionary<string, string>
            {
                { "deviceKey",  DeviceKey  },
                { "sessionKey", sessionKey }
            });
        }

        /// <summary>
        /// POST /api/agent/sessions/end
        /// </summary>
        public static void EndSession(string sessionKey)
        {
            Post("/api/agent/sessions/end", new Dictionary<string, string>
            {
                { "deviceKey",  DeviceKey  },
                { "sessionKey", sessionKey }
            });
        }

        /// <summary>
        /// GET /api/agent/sessions/relay?sessionKey=xxx → relayIp 반환 (실패 시 null)
        /// </summary>
        public static string GetRelayIp(string sessionKey)
        {
            try
            {
                string url = BaseUrl + "/api/agent/sessions/relay?sessionKey=" + Uri.EscapeDataString(sessionKey);
                string raw = _http.GetStringAsync(url).Result;
                var response = _json.Deserialize<Dictionary<string, object>>(raw);
                if (response == null || !response.ContainsKey("data")) return null;

                var data = response["data"] as Dictionary<string, object>;
                if (data == null) return null;

                return data.ContainsKey("relayIp") ? data["relayIp"]?.ToString() : null;
            }
            catch
            {
                return null;
            }
        }

        // ─── 내부 헬퍼 ───────────────────────────────────────────────────────────

        private static Dictionary<string, object> Post(string path, object body)
        {
            try
            {
                string json = _json.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = _http.PostAsync(BaseUrl + path, content).Result;
                string raw = response.Content.ReadAsStringAsync().Result;
                return _json.Deserialize<Dictionary<string, object>>(raw);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 폴링 응답 데이터 클래스
    /// </summary>
    public class AgentPollResult
    {
        public bool   HasPendingSession;
        public long   SessionId;
        public string SessionKey;
        public string Status;
    }
}
