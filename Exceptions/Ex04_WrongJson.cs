using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex04_WrongJson : JsonableException {
        protected override int Code => 4;
        protected override string Message => "Wrong JSON.";
        protected override JsonObject Details => null;
    }
}