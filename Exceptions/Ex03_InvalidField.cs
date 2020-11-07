using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex03_InvalidField : JsonableException {
        protected override int Code => 3;
        protected override string Message => "Invalid field(s).";
        protected override JsonObject Details => new JsonObject {["fields"] = Fields};
        
        private JsonArray Fields { get; }

        public Ex03_InvalidField(JsonArray fields) => Fields = fields;
    }
}