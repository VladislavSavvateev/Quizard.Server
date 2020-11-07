using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Global {
    public class GetCategories : Action {
        public override string Name => "global.getCategories";
        protected override string[] Fields => new []{"user"};
        protected override JsonType[] Types => new [] {JsonType.Object};
        
        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            await VerifyVk.Verify(json, context);
            
            var com = await DB.CommandAsync(Consts.SELECT_CATEGORIES);

            try {
                var reader = com.ExecuteReader();
                
                var array = new JsonArray();

                while (await reader.ReadAsync()) {
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["name"] = reader.GetString(1),
                        ["imgUrl"] = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
                
                return new JsonObject(Json.Ok()) {["categories"] = array};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}