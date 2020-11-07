using System.Json;
using System.Net;
using System.Threading.Tasks;

namespace QuizHub.Server.Actions {
    public class Foo : Action {
        public override string Name => "foo";
        protected override string[] Fields => null;
        protected override JsonType[] Types => null;

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) =>
            await Task.Run(() => new JsonObject {["bar"] = true});
    }
}