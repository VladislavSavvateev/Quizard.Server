using System;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Global {
    public class GetCoupons : Action {
        public override string Name => "global.getCoupons";
        protected override string[] Fields => new[] {"user"};
        protected override JsonType[] Types => new[] {JsonType.Object};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);

            var com = await DB.CommandAsync(Consts.SELECT_COUPONS);

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = userId;

                var reader = com.ExecuteReader();

                var array = new JsonArray();

                while (await reader.ReadAsync())
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["coupon"] = reader.GetString(1),
                        ["createdAt"] = ((DateTimeOffset) reader.GetDateTime(2)).ToUnixTimeSeconds(),
                        ["discount"] = new JsonObject {
                            ["id"] = reader.GetUInt64(3),
                            ["title"] = reader.GetString(4),
                            ["price"] = reader.GetInt32(5),
                            ["shop"] = new JsonObject {
                                ["id"] = reader.GetUInt64(6),
                                ["name"] = reader.GetString(7),
                                ["imgUrl"] = reader.IsDBNull(8) ? null : reader.GetString(8),
                                ["url"] = reader.GetString(9)
                            }
                        }
                    });

                return new JsonObject(Json.Ok()) {["coupons"] = array};
            } finally { await com.Connection.CloseAsync(); }
        }
    }
}