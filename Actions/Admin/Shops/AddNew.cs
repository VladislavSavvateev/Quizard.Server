using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Shops {
    public class AddNew : Action {
        public override string Name => "admin.shops.addNew";
        protected override string[] Fields => new[] {"name", "url"};
        protected override JsonType[] Types => new[] {JsonType.String, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_SHOP);

            try {
                com.Parameters.Add(Consts.NAME, MySqlDbType.VarChar, 255).Value = (string) json["name"];
                com.Parameters.Add(Consts.URL, MySqlDbType.VarChar, 255).Value = (string) json["url"];

                await com.ExecuteNonQueryAsync();

                return new JsonObject(Json.Ok()) {["shop"] = await DB.GetShopByIdAsync((ulong) com.LastInsertedId)};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}