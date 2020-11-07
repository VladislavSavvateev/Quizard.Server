using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Global {
    public class GetQuizzes : Action {
        public override string Name => "global.getQuizzes";
        protected override string[] Fields => new []{"user", "categoryId"};
        protected override JsonType[] Types => new[] {JsonType.Object, JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);
            
            var com = await DB.CommandAsync(Consts.SELECT_QUIZZES);

            try {
                com.Parameters.Add(Consts.CATEGORY_ID, MySqlDbType.UInt64).Value = (ulong) json["categoryId"];
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = userId;

                var reader = com.ExecuteReader();

                var array = new JsonArray();

                while (await reader.ReadAsync()) 
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["title"] = reader.GetString(1),
                        ["difficulty"] = reader.GetInt32(2),
                        ["run"] = reader.IsDBNull(3) ? null : JsonValue.Parse(reader.GetString(3)) as JsonObject
                    });

                return new JsonObject(Json.Ok()) {["quizzes"] = array};
            } finally { await com.Connection.CloseAsync();}
        }
    }
}