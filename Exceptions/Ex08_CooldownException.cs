using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex08_CooldownException : JsonableException {
        protected override int Code => 8;
        protected override string Message => "Cooldown!";
        protected override JsonObject Details => null;
    }
}