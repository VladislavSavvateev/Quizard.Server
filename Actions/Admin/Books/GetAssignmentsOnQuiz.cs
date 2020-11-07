using System.Json;
using System.Net;
using System.Threading.Tasks;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Books {
    public class GetAssignmentsOnQuiz : Action {
        public override string Name => "admin.books.getAssignmentsOnQuiz";
        protected override string[] Fields => new[] {"quizId"};
        protected override JsonType[] Types => new[] {JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) 
            => new JsonObject(Json.Ok()) {["books"] = await DB.GetBooksByQuizIdAsync(json["quizId"])};
    }
}