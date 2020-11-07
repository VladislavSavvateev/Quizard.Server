using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Categories {
    public class ChangeImg : Action {
        public override string Name => "admin.categories.changeImg";
        protected override string[] Fields => new[] {"categoryId", "imgUrl"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.UPDATE_CATEGORY, "img_url = @ImgUrl");

            try {
                com.Parameters.Add(Consts.ID, MySqlDbType.UInt64).Value = (ulong) json["categoryId"];
                com.Parameters.Add(Consts.IMG_URL, MySqlDbType.VarChar, 255).Value = (string) json["imgUrl"];

                com.ExecuteNonQuery();
            } finally { await com.Connection.CloseAsync(); }

            return new JsonObject(Json.Ok()) {["category"] = await DB.GetCategoryByIdAsync(json["categoryId"])};
        }
    }
}