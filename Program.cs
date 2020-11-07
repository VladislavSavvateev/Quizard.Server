using System;
using System.Threading;

namespace QuizHub.Server {
    internal static class Program {
        private static void Main() {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                if (args.ExceptionObject is Exception ex) 
                    Console.WriteLine("ERR: {0}", ex.Message);
            };
            
            var server = new Server();
            try {
                server.Start();
                Console.WriteLine("Server started!");
                Thread.Sleep(-1);
            } catch (Exception ex) {
                Console.WriteLine("Error while starting the server. {0}\n\n{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}