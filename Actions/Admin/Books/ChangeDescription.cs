using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Books {
    public class ChangeDescription : Action {
        public override string Name => "admin.books.changeDescription";
        protected override string[] Fields => new[] {"bookId", "description"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.UPDATE_BOOK, "description = @Description");

            try {
                com.Parameters.Add(Consts.ID, MySqlDbType.UInt64).Value = (ulong) json["bookId"];
                com.Parameters.Add(Consts.DESCRIPTION, MySqlDbType.Text).Value = (string) json["description"];

                await com.ExecuteNonQueryAsync();
            } finally { await com.Connection.CloseAsync(); }

            return new JsonObject(Json.Ok()) {["book"] = await DB.GetBookByIdAsync(json["bookId"])};
        }
    }
}