using Dapper;
using Npgsql;
using Polly;
using StackExchange.Redis;
[module:DapperAot]

namespace rinha_worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var warmupAsyncRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
            retryCount: 60,
            sleepDurationProvider: _ => TimeSpan.FromSeconds(1),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Async Retry {retryCount}: {exception.GetType().Name} - {exception.Message}");

            });

            var builder = WebApplication.CreateSlimBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "redis:6379";

                var options = ConfigurationOptions.Parse(redisConnection);

                options.AbortOnConnectFail = false;
                options.ConnectRetry = 3;
                options.SyncTimeout = 5000;
                options.ConnectTimeout = 5000;
                options.KeepAlive = 60;

                options.SocketManager = new SocketManager(workerCount: Environment.ProcessorCount);

                return ConnectionMultiplexer.Connect(options);
            });

            #region Redis

            builder.Services.AddHostedService<RedisConsumer>();

            builder.Services.AddHostedService<PaymentVerifier>();

            builder.Services.AddSingleton<PaymentDecider>();

            builder.Services.AddSingleton<PaymentProcessor>();

            #endregion

            var postgresConn = builder.Configuration.GetConnectionString("postgres")!;

            builder.Services.AddSingleton(provider =>
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(postgresConn);
                return dataSourceBuilder.Build();
            });

            builder.Services.AddHttpClient("default", o =>
                o.BaseAddress = new Uri(builder.Configuration.GetConnectionString("default")!));

            builder.Services.AddHttpClient("fallback", o =>
                o.BaseAddress = new Uri(builder.Configuration.GetConnectionString("fallback")!));

            var app = builder.Build();

            await warmupAsyncRetryPolicy.ExecuteAsync(async () =>
            {
                using var scope = app.Services.CreateScope();

                await using var connection = new NpgsqlConnection(postgresConn);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT 1;", connection);
                await command.ExecuteNonQueryAsync();

                Console.WriteLine("Sucesso: WarmUp Connection");
            });

            var apiGroup = app.MapGroup("/");
            apiGroup.MapGet("/", () => Results.Ok());
            apiGroup.MapPost("/", () => Results.Ok());

            app.Run();
        }
    }
}
