using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Books {
    public class AssignBookToQuiz : Action {
        public override string Name => "admin.books.assignBookToQuiz";
        protected override string[] Fields => new[] {"bookId", "quizId"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_MAP_BOOK_QUIZ);

            try {
                com.Parameters.Add(Consts.BOOK_ID, MySqlDbType.UInt64).Value = (ulong) json["bookId"];
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = (ulong) json["quizId"];

                com.ExecuteNonQuery();
            } finally { await com.Connection.CloseAsync(); }

            return Json.Ok();
        }
    }
}