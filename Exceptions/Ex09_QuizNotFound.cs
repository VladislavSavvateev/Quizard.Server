using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex09_QuizNotFound : JsonableException {
        protected override int Code => 9;
        protected override string Message => "Quiz not found.";
        protected override JsonObject Details => null;
    }
}