using System;

namespace Serilog.Sinks.Vecc.RabbitMQ.Sandbox
{
    public static class Program
    {
        public static void Main()
        {
            var logConfiguration = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.RabbitMQ("localhost", "guest", "guest", "logs", "logevents", typeof(Program).Namespace)
                .MinimumLevel.Verbose();

            Log.Logger = logConfiguration.CreateLogger();

            while (true)
            {
                Log.Verbose("test {@prop1}", new { x = "xval", y = "yval" });
                Log.Debug("test {@prop1}", new { x = "xval", y = "yval" });
                Log.Information("test {@prop1}", new { x = "xval", y = "yval" });
                Log.Warning("test {@prop1}", new { x = "xval", y = "yval" });
                Log.Error("test {@prop1}", new { x = "xval", y = "yval" });
                Log.Fatal("test {@prop1}", new { x = "xval", y = "yval" });

                var exception = new Exception(Guid.NewGuid().ToString());
                Log.Verbose(exception, "test {@prop1}", new { x = "xval", y = "yval" });
                Log.Debug(exception, "test {@prop1}", new { x = "xval", y = "yval" });
                Log.Information(exception, "test {@prop1}", new { x = "xval", y = "yval" });
                Log.Warning(exception, "test {@prop1}", new { x = "xval", y = "yval" });
                Log.Error(exception, "test {@prop1}", new { x = "xval", y = "yval" });
                Log.Fatal(exception, "test {@prop1}", new { x = "xval", y = "yval" });

                if (Console.ReadLine() == "-1")
                {
                    break;
                }
            }

            Log.CloseAndFlush();
        }
    }
}
