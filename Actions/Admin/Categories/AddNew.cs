using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Categories {
    public class AddNew : Action {
        public override string Name => "admin.categories.addNew";
        protected override string[] Fields => new[] {"categoryName"};
        protected override JsonType[] Types => new[] {JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_CATEGORY);

            try {
                com.Parameters.Add(Consts.NAME, MySqlDbType.VarChar, 255).Value = (string) json["categoryName"];

                com.ExecuteNonQuery();

                return new JsonObject(Json.Ok()) {
                    ["category"] = new JsonObject {
                        ["id"] = com.LastInsertedId,
                        ["name"] = (string) json["categoryName"],
                        ["imgUrl"] = ""
                    }
                };
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}