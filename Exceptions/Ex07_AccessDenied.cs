using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex07_AccessDenied : JsonableException {
        protected override int Code => 7;
        protected override string Message => "Access denied.";
        protected override JsonObject Details => null;
    }
}