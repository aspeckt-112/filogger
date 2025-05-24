using FiLogger;

using Microsoft.Extensions.Logging;

// ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
// {
//     string path = Path.Combine(
//         Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
//         "filogger_test",
//         "test.txt");
//
//     builder.AddFiLogger(
//         path,
//         new FiLoggerOptions
//         {
//             FlushMethod = FlushMethod.Immediate
//         },
//         LogLevel.Debug);
//
//     builder.AddConsole();
// });
//
// ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
//
// logger.LogInformation("Hello, this is a test log message.");

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "filogger_test",
        "test.txt");

    builder.AddFiLogger(
        path,
        new FiLoggerOptions
        {
            FlushMethod = FlushMethod.Periodic,
            FlushIntervalSeconds = 5
        },
        LogLevel.Debug);

    builder.AddConsole();
});

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

for (int i = 0; i < 10; i++)
{
    logger.LogInformation("Hello, this is a test log message {Index}.", i);
    await Task.Delay(TimeSpan.FromSeconds(2.5));
}

Console.ReadLine();