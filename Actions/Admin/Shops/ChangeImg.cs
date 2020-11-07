using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Shops {
    public class ChangeImg : Action {
        public override string Name => "admin.shops.changeImg";
        protected override string[] Fields => new [] {"shopId", "imgUrl"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.UPDATE_SHOP, "img_url = @ImgUrl");

            try {
                com.Parameters.Add(Consts.ID, MySqlDbType.UInt64).Value = (ulong) json["shopId"];
                com.Parameters.Add(Consts.IMG_URL, MySqlDbType.VarChar, 255).Value = (string) json["imgUrl"];

                await com.ExecuteNonQueryAsync();
            } finally { await com.Connection.CloseAsync(); }

            return new JsonObject(Json.Ok()) {["shop"] = await DB.GetShopByIdAsync(json["shopId"])};
        }
    }
}