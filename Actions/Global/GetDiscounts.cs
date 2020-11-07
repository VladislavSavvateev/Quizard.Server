using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Global {
    public class GetDiscounts : Action {
        public override string Name => "global.getDiscounts";
        protected override string[] Fields => new []{"user"};
        protected override JsonType[] Types => new [] {JsonType.Object};
        
        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);
            
            var com = await DB.CommandAsync(Consts.SELECT_DISCOUNTS);

            try {
                var reader = com.ExecuteReader();

                var array = new JsonArray();

                while (await reader.ReadAsync())
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["title"] = reader.GetString(1),
                        ["price"] = reader.GetInt32(2),
                        ["shop"] = new JsonObject {
                            ["id"] = reader.GetUInt64(3),
                            ["name"] = reader.GetString(4),
                            ["imgUrl"] = reader.IsDBNull(5) ? null : reader.GetString(5),
                            ["url"] = reader.GetString(6)
                        }
                    });

                return new JsonObject(Json.Ok()) {["discounts"] = array, ["balance"] = await DB.GetUserBalanceAsync(userId)};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}