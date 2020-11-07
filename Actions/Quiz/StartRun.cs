using System;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Exceptions;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Quiz {
    public class StartRun : Action {
        public override string Name => "quiz.startRun";
        protected override string[] Fields => new[] {"user", "quizId"};
        protected override JsonType[] Types => new[] {JsonType.Object, JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);
            
            /*var com = await DB.CommandAsync(Consts.SELECT_LAST_RUN);

            try {
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = (ulong) json["quizId"];
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.UInt64).Value = userId;

                var reader = com.ExecuteReader();

                if (await reader.ReadAsync())
                    if (DateTime.Now.Subtract(reader.GetDateTime(3)).TotalSeconds < Consts.COOLDOWN_DURATION)
                        throw new Ex08_CooldownException();
            } finally { await com.Connection.CloseAsync(); }*/

            var com = await DB.CommandAsync(Consts.SELECT_QUESTIONS);

            try {
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = (ulong) json["quizId"];

                var reader = com.ExecuteReader();

                if (!reader.HasRows) throw new Ex09_QuizNotFound();

                var array = new JsonArray();

                while (await reader.ReadAsync())
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["text"] = reader.GetString(1),
                        ["options"] = JsonValue.Parse(reader.GetString(3)) as JsonArray,
                        ["type"] = reader.GetInt32(5)
                    });

                return new JsonObject(Json.Ok()) {["questions"] = array};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}