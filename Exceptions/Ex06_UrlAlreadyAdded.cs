using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex06_UrlAlreadyAdded : JsonableException {
        protected override int Code => 6;
        protected override string Message => "URL is already added.";
        protected override JsonObject Details => null;
    }
}