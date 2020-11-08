using System;
using System.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Exceptions;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Quiz {
    public class EndRun : Action {
        public override string Name => "quiz.endRun";
        protected override string[] Fields => new[] {"user", "quizId", "answers"};
        protected override JsonType[] Types => new[] {JsonType.Object, JsonType.Number, JsonType.Array};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context); 
            
            var com = await DB.CommandAsync(Consts.SELECT_LAST_RUN);

            try {
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = (ulong) json["quizId"];
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.UInt64).Value = userId;

                var reader = com.ExecuteReader();

                if (await reader.ReadAsync())
                    if (DateTime.Now.Subtract(reader.GetDateTime(3)).TotalSeconds < Consts.COOLDOWN_DURATION)
                        throw new Ex08_CooldownException();
            } finally { await com.Connection.CloseAsync(); }

            var answersFromJson = (JsonArray) json["answers"];
            var answersFromDb = await DB.GetRightAnswersFromQuizAsync(json["quizId"], userId);

            var points = 0;
            
            foreach (var val in answersFromDb) {
                if (!(val is JsonObject answerFromDb)) continue;
                var answerFromJson = FindAnswerById(answersFromJson, answerFromDb["id"]);

                answerFromDb["answerFromClient"] = new JsonObject {["answer"] = answerFromJson?["answer"]};
                answerFromDb["right"] = answerFromJson != null &&
                                        CompareAnswers(answerFromDb["type"], answerFromDb, answerFromJson);

                if (answerFromDb["right"] && !answerFromDb["wasAnsweredRight"]) points += answerFromDb["points"];
            }

            com = await DB.CommandAsync(Consts.INSERT_NEW_RUN);

            long runId;

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = userId;
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = (ulong) json["quizId"];

                com.ExecuteNonQuery();

                runId = com.LastInsertedId;
            } finally { await com.Connection.CloseAsync(); }

            foreach (var val in answersFromDb) {
                if (!(val is JsonObject answer)) continue;

                com = await DB.CommandAsync(Consts.INSERT_NEW_ANSWER);

                try {
                    com.Parameters.Add(Consts.RUN_ID, MySqlDbType.UInt64).Value = runId;
                    com.Parameters.Add(Consts.ANSWER, MySqlDbType.JSON).Value =
                        ((JsonObject) answer["answerFromClient"]).ToString();
                    com.Parameters.Add(Consts.QUESTION_ID, MySqlDbType.UInt64).Value = (ulong) answer["id"];
                    com.Parameters.Add(Consts.IS_IT_RIGHT, MySqlDbType.Int16).Value = (bool) answer["right"];

                    com.ExecuteNonQuery();
                } finally { await com.Connection.CloseAsync(); }
            }

            if (points > 0) await DB.UpdateBalanceAsync(userId, points);

            return new JsonObject(Json.Ok()) {
                ["newEpoints"] = points,
                ["rightAnswerCount"] = answersFromDb.Count(a => a.ContainsKey("right") && a["right"]),
                ["totalQuestionCount"] = answersFromDb.Count,
                ["books"] = await DB.GetBooksByQuizIdAsync(json["quizId"])
            };
        }

        private static bool CompareAnswers(int type, JsonObject answer1, JsonObject answer2) {
            switch (type) {
                case 1: {
                    if (!(answer1["answer"] is JsonArray arr1) || !(answer2["answer"] is JsonArray arr2) ||
                        arr1.Count != arr2.Count)
                        return false;

                    return !arr1.Where((t, i) => t.JsonType != arr2[i].JsonType || t.JsonType != JsonType.Number ||
                                                 (int) t != (int) arr2[i]).Any();
                }
                case 2: {
                    return answer1["answer"].JsonType == answer2["answer"].JsonType &&
                           answer1["answer"].JsonType == JsonType.Number &&
                           (int) answer1["answer"] == (int) answer2["answer"];
                }
                case 3: {
                    return answer1["answer"].JsonType == answer2["answer"].JsonType &&
                           answer1["answer"].JsonType == JsonType.String &&
                           (string) answer1["answer"] == answer2["answer"];
                }
                default: return false;
            }
        }

        private static JsonObject FindAnswerById(JsonArray array, ulong id) 
            => array.FirstOrDefault(a => a.ContainsKey("id") && a["id"] == id) as JsonObject;
    }
}