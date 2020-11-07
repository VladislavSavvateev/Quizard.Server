using System;
using System.Data;
using System.Json;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuizHub.Server.Exceptions;

namespace QuizHub.Server.Util {
    public static class DB {

        public static async Task<MySqlCommand> CommandAsync(string format, params object[] vars) 
            => new MySqlCommand(string.Format(format, vars), await ConnectionAsync());

        public static async Task<MySqlConnection> ConnectionAsync() {
            var con = new MySqlConnection(CONNECTION_STRING);
            await con.OpenAsync();

            return con;
        }

        public static async Task<JsonObject> GetCategoryByIdAsync(ulong categoryId) {
            var com = await CommandAsync(Consts.SELECT_CATEGORY_BY_ID);

            try {
                com.Parameters.Add(Consts.ID, MySqlDbType.UInt64).Value = categoryId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync()
                    ? new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["name"] = reader.GetString(1),
                        ["imgUrl"] = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    }
                    : null;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonArray> GetRightAnswersFromQuizAsync(ulong quizId, long vkId) {
            var com = await CommandAsync(Consts.SELECT_ANSWERS_FROM_QUESTIONS);

            try {
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = quizId;
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = vkId;

                var reader = com.ExecuteReader();

                if (!reader.HasRows) throw new Ex09_QuizNotFound();

                var array = new JsonArray();

                while (await reader.ReadAsync())
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["points"] = reader.GetInt32(1),
                        ["type"] = reader.GetInt32(2),
                        ["answer"] = reader.IsDBNull(3) ? null : (JsonValue.Parse(reader.GetString(3)) as JsonObject)?["answer"],
                        ["wasAnsweredRight"] = reader.GetBoolean(4)
                    });

                return array;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task UpdateBalanceAsync(long vkId, long balanceDelta) {
            var com = await CommandAsync(Consts.UPDATE_BALANCE);

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.UInt64).Value = vkId;
                com.Parameters.Add(Consts.BALANCE_DELTA, MySqlDbType.Int64).Value = balanceDelta;

                if (await com.ExecuteNonQueryAsync() == 0) await InsertNewBalanceAsync(vkId, balanceDelta);
            } finally { await com.Connection.CloseAsync(); }
        }

        private static async Task InsertNewBalanceAsync(long vkId, long balance) {
            var com = await CommandAsync(Consts.INSERT_NEW_BALANCE);

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = vkId;
                com.Parameters.Add(Consts.BALANCE, MySqlDbType.Int64).Value = balance;

                await com.ExecuteNonQueryAsync();
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonObject> GetBookByIdAsync(ulong bookId) {
            var com = await CommandAsync(Consts.SELECT_BOOK_BY_ID);

            try {
                com.Parameters.Add(Consts.BOOK_ID, MySqlDbType.UInt64).Value = bookId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync()
                    ? new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["title"] = reader.GetString(1),
                        ["description"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ["url"] = reader.GetString(3),
                        ["imgUrl"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ["shop"] = new JsonObject {
                            ["id"] = reader.GetUInt64(5),
                            ["name"] = reader.GetString(6),
                            ["imgUrl"] = reader.IsDBNull(7) ? null : reader.GetString(7),
                            ["url"] = reader.GetString(8)
                        }
                    } : null;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonArray> GetBooksByQuizIdAsync(ulong quizId) {
            var com = await CommandAsync(Consts.SELECT_BOOK_ASSIGNMENTS_BY_QUIZ_ID);

            try {
                com.Parameters.Add(Consts.QUIZ_ID, MySqlDbType.UInt64).Value = quizId;

                var reader = com.ExecuteReader();

                var array = new JsonArray();

                while (await reader.ReadAsync())
                    array.Add(new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["title"] = reader.GetString(1),
                        ["description"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ["url"] = reader.GetString(3),
                        ["imgUrl"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ["shop"] = new JsonObject {
                            ["id"] = reader.GetUInt64(5),
                            ["name"] = reader.GetString(6),
                            ["imgUrl"] = reader.IsDBNull(7) ? null : reader.GetString(7),
                            ["url"] = reader.GetString(8)
                        }
                    });

                return array;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonObject> GetDiscountByIdAsync(ulong discountId) {
            var com = await CommandAsync(Consts.SELECT_DISCOUNT_BY_ID);

            try {
                com.Parameters.Add(Consts.DISCOUNT_ID, MySqlDbType.UInt64).Value = discountId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync()
                    ? new JsonObject {
                        ["id"] = discountId,
                        ["title"] = reader.GetString(0),
                        ["price"] = reader.GetInt32(1),
                        ["shop"] = new JsonObject {
                            ["id"] = reader.GetUInt64(2),
                            ["name"] = reader.GetString(3),
                            ["imgUrl"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ["url"] = reader.GetString(5)
                        }
                    } : null;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<long> GetUserBalanceAsync(long vkId) {
            var com = await CommandAsync(Consts.SELECT_BALANCE);

            try {
                com.Parameters.Add(Consts.VK_ID, MySqlDbType.Int64).Value = vkId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync() ? reader.GetInt64(0) : 0;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonObject> GetCouponByIdAsync(ulong couponId) {
            var com = await CommandAsync(Consts.SELECT_COUPON_BY_ID);

            try {
                com.Parameters.Add(Consts.COUPON_ID, MySqlDbType.UInt64).Value = couponId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync()
                    ? new JsonObject {
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
                    }
                    : null;
            } finally { await com.Connection.CloseAsync(); }
        }

        public static async Task<JsonObject> GetShopByIdAsync(ulong shopId) {
            var com = await CommandAsync(Consts.SELECT_SHOP_BY_ID);

            try {
                com.Parameters.Add(Consts.SHOP_ID, MySqlDbType.UInt64).Value = shopId;

                var reader = com.ExecuteReader();

                return await reader.ReadAsync()
                    ? new JsonObject {
                        ["id"] = reader.GetUInt64(0),
                        ["name"] = reader.GetString(1),
                        ["imgUrl"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ["url"] = reader.GetString(3)
                    } : null;
            } finally { await com.Connection.CloseAsync(); }
        }

        private const string CONNECTION_STRING =
            "host=localhost;database=quiz_hub;user=quiz_hub_user;password=12345678";
    }
}