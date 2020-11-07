using System.IO;
using System.Json;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace QuizHub.Server.Util {
    public static class Answer {
        public static void Code(HttpListenerContext context, int code) {
            context.Response.StatusCode = code;
            context.Response.Close(Encoding.UTF8.GetBytes($"<h1>{code}</h1>"), false);
        }

        public static void File(HttpListenerContext context, FileInfo fileInfo) {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileInfo.FullName, out var contentType))
                contentType = "application/octet-stream";
            context.Response.ContentType = contentType;
            
            using var inputStream = fileInfo.OpenRead();
            Stream(context, inputStream);
            context.Response.Close();
        }

        public static void Stream(HttpListenerContext context, Stream stream) {
            using var os = context.Response.OutputStream;
            stream.CopyTo(os);
        }

        public static void Json(HttpListenerContext context, JsonValue jsonValue) {
            context.Response.ContentType = "application/json";
            context.Response.Close(Encoding.UTF8.GetBytes(jsonValue.ToString()), false);
        }
    }
}