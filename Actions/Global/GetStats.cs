using System.Data;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Exceptions;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Global {
    public class GetStats : Action {
        public override string Name => "global.getStats";
        protected override string[] Fields => new[] {"user"};
        protected override JsonType[] Types => new[] {JsonType.Object};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);

            var com = await DB.CommandAsync(Consts.SELECT_USER_STAT);

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.UInt64).Value = userId;

                var reader = com.ExecuteReader();

                if (await reader.ReadAsync())
                    return new JsonObject(Json.Ok()) {
                        ["runsAtCurrentMonthCount"] = reader.GetInt64(0),
                        ["totalRunCount"] = reader.GetInt64(1),
                        ["favouriteCategoryName"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ["balance"] = reader.GetInt64(3)
                    };

                throw new Ex07_AccessDenied();
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}