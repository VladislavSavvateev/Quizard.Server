using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QuizHub.Server.Exceptions;
using QuizHub.Server.Util;
using Action = QuizHub.Server.Actions.Action;

namespace QuizHub.Server {
    public class Server {
        private HttpListener Listener { get; }

        private Dictionary<string, Action> Actions { get; }

        public Server() {
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://*:1781/");

            Actions = GetActions();
        }

        private static Dictionary<string, Action> GetActions() {
            var dict = new Dictionary<string, Action>();

            foreach (var (name, action) in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(Action)).Select(t => Activator.CreateInstance(t) as Action)
                .Where(a => a != null).Select(a => (a.Name, a)))
                if (name != null && action != null)
                    dict.Add(name, action);

            return dict;
        }

        public void Start() {
            Listener.Start();
            Listener.BeginGetContext(OnConnection, null!);

            DB.ConnectionAsync().GetAwaiter().GetResult();
        }

        private void OnConnection(IAsyncResult ar) {
            try {
                Listener.BeginGetContext(OnConnection, null!);
                var context = Listener.EndGetContext(ar);

                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.AddHeader("Server", "QuizHub/1.0");
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");

                try {
                    switch (context.Request.HttpMethod) {
                        case "GET":
                            OnGET(context);
                            break;
                        case "POST":
                            OnPOST(context);
                            break;
                        case "OPTIONS":
                            context.Response.Close();
                            break;
                        default:
                            context.Response.Abort();
                            break;
                    }
                } catch (JsonableException ex) {
                    Answer.Json(context, Json.Error(ex));
                } catch (ArgumentException ex) {
                    Answer.Json(context,
                        ex.StackTrace?.Contains("Json.JavaScriptReader") ?? false
                            ? Json.Error(new Ex04_WrongJson())
                            : Json.Error(new Ex01_Unexpected(ex)));
                } catch (Exception ex) {
                    Answer.Json(context, Json.Error(new Ex01_Unexpected(ex)));
                }
            } catch (Exception ex) { Console.WriteLine("ERR: {0}\n\n{1}", ex.Message, ex.StackTrace); }
        }

        private static void OnGET(HttpListenerContext context) {
            var storageDir = Directory.CreateDirectory("storage");
            var url = context.Request.Url;
            if (url == null) throw new Ex07_AccessDenied();
            
            if (url.AbsolutePath.Contains("..")) {
                Answer.Code(context, 403);
                return;
            }

            var file = new FileInfo(Path.Join(storageDir.FullName,
                url.AbsolutePath == "/" ? "/index.html" : url.AbsolutePath));

            if (!file.Exists) {
                Answer.Code(context, 404);
                return;
            }

            Answer.File(context, file);
        }

        private void OnPOST(HttpListenerContext context) {
            string inputString;
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8)) 
                inputString = sr.ReadToEnd();

            if (!(JsonValue.Parse(inputString) is JsonObject json)) throw new Ex04_WrongJson();
            
            Json.CheckFields(json, "action");
            Json.CheckTypes(json, new []{"action"}, new [] {JsonType.String});

            if (!Actions.TryGetValue(json["action"], out var action)) throw new Ex05_ActionNotFound();
            
            action.Validate(this, context, json);
            action.DoWork(this, context, json).ContinueWith(t => {
                switch (t.Status) {
                    case TaskStatus.Faulted when t.Exception != null: {
                        Answer.Json(context,
                            Json.Error(t.Exception.InnerException is JsonableException jsonable
                                ? jsonable
                                : new Ex01_Unexpected(t.Exception.InnerException)));

                        break;
                    }
                    case TaskStatus.RanToCompletion: Answer.Json(context, t.Result);
                        break;
                }
            });
        }
    }
}