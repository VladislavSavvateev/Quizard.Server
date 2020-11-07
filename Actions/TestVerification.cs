using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions {
    public class TestVerification : Action {
        public override string Name => "testVerification";
        protected override string[] Fields => new[] {"user"};
        protected override JsonType[] Types => new[] {JsonType.Object};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            await VerifyVk.Verify(json, context);
            return await Task.Run(() => new JsonObject(Json.Ok()) {["successful"] = true});
        }
    }
}