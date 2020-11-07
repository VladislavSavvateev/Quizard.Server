using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions {
    public abstract class Action {
        public abstract string Name { get; }
        protected abstract string[] Fields { get; }
        protected abstract JsonType[] Types { get; }

        public virtual void Validate(Server server, HttpListenerContext context, JsonObject json) {
            if (Fields == null || Fields.Length == 0) return;
            
            Json.CheckFields(json, Fields);
                
            if (Types != null && Types.Length > 0) Json.CheckTypes(json, Fields, Types);
            
            if (Name.StartsWith("admin.")) Secret.Check(context);
        }

        public abstract Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json);
    }
}