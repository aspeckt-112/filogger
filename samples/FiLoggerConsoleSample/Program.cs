using FiLogger;
using FiLogger.Enums;
using FiLogger.Extensions;
using FiLogger.Options;

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
//         FiLoggerOptions.CreateImmediate(),
//         LogLevel.Debug);
//
//     builder.AddConsole();
// });
//
// ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
//
// logger.LogInformation("Hello, this is a test log message.");

// ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
// {
//     string path = Path.Combine(
//         Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
//         "filogger_test",
//         "test.txt");
//
//     builder.AddFiLogger(
//         path,
//         FiLoggerOptions.CreatePeriodic(5),
//         LogLevel.Debug);
//
//     builder.AddConsole();
// });
//
// ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
//
// for (int i = 0; i < 10; i++)
// {
//     logger.LogInformation("Hello, this is a test log message {Index}.", i);
//     await Task.Delay(TimeSpan.FromSeconds(2.5));
// }

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "filogger_test",
        "test.txt");

    builder.AddFiLogger(
        path,
        FiLoggerOptions.CreateImmediate(),
        LogLevel.Debug);

    builder.AddConsole();
});

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

var someObject = new
{
    Name = "Test Object",
    Value = 42
};

logger.LogInformation("Hello, someObject: {SomeObject}", someObject);

Console.ReadLine();