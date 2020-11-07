using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Discounts {
    public class AddNew : Action {
        public override string Name => "admin.discounts.addNew";
        protected override string[] Fields => new[] {"shopId", "title", "price"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String, JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.INSERT_NEW_DISCOUNT);

            try {
                com.Parameters.Add(Consts.SHOP_ID, MySqlDbType.UInt64).Value = (ulong) json["shopId"];
                com.Parameters.Add(Consts.TITLE, MySqlDbType.VarChar, 255).Value = (string) json["title"];
                com.Parameters.Add(Consts.PRICE, MySqlDbType.Int32).Value = (int) json["price"];

                await com.ExecuteNonQueryAsync();

                return new JsonObject(Json.Ok())
                    {["discount"] = await DB.GetDiscountByIdAsync((ulong) com.LastInsertedId)};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}