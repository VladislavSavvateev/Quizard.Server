using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Quizzes {
    public class AddNew : Action {
        public override string Name => "admin.quizzes.addNew";
        protected override string[] Fields => new[] {"categoryId", "difficulty", "title", "questions"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.Number, JsonType.String, JsonType.Array};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_QUIZ);

            long quizId;

            try {
                com.Parameters.Add(Consts.TITLE, MySqlDbType.VarChar, 255).Value = (string) json["title"];
                com.Parameters.Add(Consts.CATEGORY_ID, MySqlDbType.UInt64).Value = (ulong) json["categoryId"];
                com.Parameters.Add(Consts.DIFFICULTY, MySqlDbType.Int32).Value = (int) json["difficulty"];

                com.ExecuteNonQuery();

                quizId = com.LastInsertedId;
            } finally { await com.Connection.CloseAsync(); }

            var questions = (JsonArray) json["questions"];

            foreach (var val in questions) {
                if (!(val is JsonObject question)) continue;

                com = await DB.CommandAsync(Consts.INSERT_NEW_QUESTION);

                try {
                    com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = quizId;
                    com.Parameters.Add(Consts.TEXT, MySqlDbType.Text).Value = (string) question["text"];
                    com.Parameters.Add(Consts.POINTS, MySqlDbType.Int32).Value = (int) question["epoints"];
                    com.Parameters.Add(Consts.OPTIONS, MySqlDbType.JSON).Value =
                        ((JsonArray) question["options"]).ToString();
                    com.Parameters.Add(Consts.ANSWER, MySqlDbType.JSON).Value =
                        new JsonObject {["answer"] = question["answer"]}.ToString();
                    com.Parameters.Add(Consts.TYPE, MySqlDbType.Int32).Value = (int) question["type"];

                    com.ExecuteNonQuery();
                } finally { await com.Connection.CloseAsync(); }
            }

            return Json.Ok();
        }
    }
}