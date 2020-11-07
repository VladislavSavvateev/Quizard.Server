using System;
using System.Json;

namespace QuizHub.Server.Exceptions {
    public abstract class JsonableException : Exception {
        protected abstract int Code { get; }
        protected new abstract string Message { get; }
        protected abstract JsonObject Details { get; }

        public JsonObject ToJson()
            => new JsonObject
                {["code"] = Code, ["message"] = Message, ["details"] = Details, ["stackTrace"] = StackTrace};
    }
}