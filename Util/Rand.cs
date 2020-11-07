using System;

namespace QuizHub.Server.Util {
    public static class Rand {
        private static Random Random { get; } = new Random();
        
        public static string Coupon(int length = 16) {
            var result = "";

            for (var i = 0; i < length; i++) result += ALPHABET[Random.Next(ALPHABET.Length)];

            return result;
        }

        private const string ALPHABET = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    }
}