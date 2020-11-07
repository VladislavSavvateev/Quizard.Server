using System;
using System.Json;
using System.Linq;
using QuizHub.Server.Exceptions;

namespace QuizHub.Server.Util {
    public static class Json {
        public static JsonObject Ok() => new JsonObject {["status"] = true};
        public static JsonObject Error(JsonableException ex) => new JsonObject {["status"] = false, ["error"] = ex.ToJson()};

        public static void CheckFields(JsonObject json, params string[] fields) {
            var fieldsThatNotFound = new JsonArray(fields.Where(f => !json.ContainsKey(f)).Select(f => (JsonValue) f));
            if (fieldsThatNotFound.Count > 0) throw new Ex02_FieldNotFound(fieldsThatNotFound);
        }

        public static void CheckTypes(JsonObject json, string[] fields, JsonType[] types) {
            var arr = new JsonArray();
            for (var i = 0; i < Math.Min(fields.Length, types.Length); i++) 
                if (json[fields[i]].JsonType != types[i])
                    arr.Add(new JsonObject {
                        ["fieldName"] = fields[i],
                        ["expectedType"] = ReturnJsonTypeString(types[i])
                    });

            if (arr.Count > 0) throw new Ex03_InvalidField(arr);
        }

        private static string ReturnJsonTypeString(JsonType jsonType) => jsonType switch {
            JsonType.String => "string",
            JsonType.Number => "number",
            JsonType.Object => "object",
            JsonType.Array => "array",
            JsonType.Boolean => "boolean",
            _ => throw new ArgumentOutOfRangeException(nameof(jsonType), jsonType, null)
        };
    }
}