using System;
using System.Json;

namespace QuizHub.Server.Exceptions {
    public class Ex01_Unexpected : JsonableException {
        protected override int Code => 1;
        protected override string Message => "Unexpected exception was occurred.";

        protected override JsonObject Details
            => new JsonObject {
                ["name"] = Exception.GetType().ToString(),
                ["message"] = Exception.Message,
                ["stackTrace"] = Exception.StackTrace,
                ["innerException"] = Exception.InnerException != null
                    ? new Ex01_Unexpected(Exception.InnerException).Details
                    : null
            };
        
        private Exception Exception { get; }

        public Ex01_Unexpected(Exception ex) => Exception = ex;
        
    }
}