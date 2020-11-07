using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Admin.Books {
    public class ChangeImg : Action {
        public override string Name => "admin.books.changeImg";
        protected override string[] Fields => new[] {"bookId", "imgUrl"};
        protected override JsonType[] Types => new[] {JsonType.Number, JsonType.String};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var com = await DB.CommandAsync(Consts.UPDATE_BOOK, "img_url = @ImgUrl");

            try {
                com.Parameters.Add(Consts.ID, MySqlDbType.UInt64).Value = (ulong) json["bookId"];
                com.Parameters.Add(Consts.IMG_URL, MySqlDbType.VarChar, 255).Value = (string) json["imgUrl"];

                await com.ExecuteNonQueryAsync();
            } finally { await com.Connection.CloseAsync(); }

            return new JsonObject(Json.Ok()) {["book"] = await DB.GetBookByIdAsync(json["bookId"])};
        }
    }
}