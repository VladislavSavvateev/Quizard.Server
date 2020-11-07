using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex02_FieldNotFound : JsonableException {
        protected override int Code => 2;
        protected override string Message => "Field(s) not found.";
        protected override JsonObject Details => new JsonObject {["fields"] = Fields};
        
        private JsonArray Fields { get; }

        public Ex02_FieldNotFound(JsonArray fields) => Fields = fields;
    }
}