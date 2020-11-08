using System.Json;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Exceptions;
using QuizHub.Server.Util;

namespace QuizHub.Server.Actions.Coupons {
    public class GenerateNew : Action {
        public override string Name => "coupons.generateNew";
        protected override string[] Fields => new[] {"user", "discountId"};
        protected override JsonType[] Types => new[] {JsonType.Object, JsonType.Number};

        public override async Task<JsonObject> DoWork(Server server, HttpListenerContext context, JsonObject json) {
            var userId = await VerifyVk.Verify(json, context);

            var discount = await DB.GetDiscountByIdAsync(json["discountId"]);
            if (discount == null) throw new Ex07_AccessDenied();

            var balance = await DB.GetUserBalanceAsync(userId);

            if (balance < discount["price"]) throw new Ex07_AccessDenied();

            var com = await DB.CommandAsync(Consts.INSERT_NEW_COUPON);

            JsonObject coupon;

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = userId;
                com.Parameters.Add(Consts.DISCOUNT_ID, MySqlDbType.UInt64).Value = (ulong) json["discountId"];
                com.Parameters.Add(Consts.COUPON, MySqlDbType.VarChar, 255).Value = Rand.Coupon();

                await com.ExecuteNonQueryAsync();

                coupon = await DB.GetCouponByIdAsync((ulong) com.LastInsertedId);
            } finally { await com.Connection.CloseAsync(); }

            await DB.UpdateBalanceAsync(userId, -discount["price"]);

            return new JsonObject(Json.Ok()) {["coupon"] = coupon};
        }
    }
}