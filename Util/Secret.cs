using System.Net;
using QuizHub.Server.Exceptions;

namespace QuizHub.Server.Util {
    public static class Secret {
        public static void Check(HttpListenerContext context) {
            if (PrivateConsts.ADMIN_SECRET_KEY != context.Request.Headers["Secret-Key"])
                throw new Ex07_AccessDenied();
        }
    }
}