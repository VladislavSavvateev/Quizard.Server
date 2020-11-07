using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex05_ActionNotFound : JsonableException {
        protected override int Code => 5;
        protected override string Message => "Action not found.";
        protected override JsonObject Details => null;
    }
}