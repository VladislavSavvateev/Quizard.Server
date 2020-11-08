// #define SUPPORT_BYPASS_CHECKS

using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Exceptions;

namespace QuizHub.Server.Util {
    public static class VerifyVk {
        public static async Task<long> Verify(JsonObject thing, HttpListenerContext context) {
            var ip = context.Request.Headers["X-Real-IP"];
            if (!(thing["user"] is JsonObject user) || !user.ContainsKey("token") || !user.ContainsKey("id"))
                throw new Ex07_AccessDenied();

            #if SUPPORT_BYPASS_CHECKS
            if (context.Request.Headers["BypassChecks"] != null) return user["id"];
            #endif
            
            var success = false;

            foreach (var token in PrivateConsts.SERVICE_ACCESS_TOKEN) {
                var res = await new WebClient().DownloadStringTaskAsync(
                    $"https://api.vk.com/method/secure.checkToken?v=5.124&token={(string) user["token"]}&ip={ip}&access_token={token}");
                if (!(JsonValue.Parse(res) is JsonObject json) || !json.ContainsKey("response")) continue;

                if (json["response"] is not JsonObject userFromVk || (long) userFromVk["user_id"] != (long) user["id"])
                    continue;
                
                success = true;
                break;
            }

            if (!success) throw new Ex07_AccessDenied();

            return user["id"];
        }
    }
}