using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Books {
    public class AddNew : Action {
        public override string Name => "admin.books.addNew";
        protected override string[] Fields => new[] {"shopId", "title", "url"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_BOOK);

            try {
                com.Parameters.Add(Consts.SHOP_ID, MySqlDbType.UInt64).Value = (ulong) json["shopId"];
                com.Parameters.Add(Consts.TITLE, MySqlDbType.VarChar, 255).Value = (string) json["title"];
                com.Parameters.Add(Consts.URL, MySqlDbType.VarChar, 1023).Value = (string) json["url"];

                await com.ExecuteNonQueryAsync();

                return new JsonObject(Json.Ok()) {["book"] = await DB.GetBookByIdAsync((ulong) com.LastInsertedId)};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}