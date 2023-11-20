using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Zaraye.Services.Logging
{
    public partial class AppLoggerService : IAppLoggerService
    {
        public virtual async Task<object> WriteLogs(Exception exception)
        {
            try
            {
                string target = System.IO.Directory.GetCurrentDirectory() + "\\Logs";
                if (!System.IO.Directory.Exists(target))
                {
                    System.IO.Directory.CreateDirectory(target);
                }
                string logFilePath = target + "\\"+ DateTime.Now.ToString("yyyyMMdd")+"_log.txt";
                using (StreamWriter logFileWriter = new StreamWriter(logFilePath, append: true))
                {
                    ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                    {
                        builder.AddSimpleConsole(options =>
                        {
                            options.IncludeScopes = true;
                            options.SingleLine = true;
                            options.TimestampFormat = "HH:mm:ss ";
                        });

                        builder.AddProvider(new CustomFileLoggerProvider(logFileWriter));
                    });
                    
                    //Create an ILogger
                    ILogger<AppLoggerService> logger = loggerFactory.CreateLogger<AppLoggerService>();
                    
                    //Output some text on the console
                    using (logger.BeginScope("[scope is enabled]"))
                    {
                        logger.LogInformation(exception.Message);
                        logger.LogInformation(exception.Source);
                        logger.LogInformation("Each log message is fit in a single line.");
                        logger.LogWarning(exception.StackTrace);
                    }
                }

                return exception.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("logsss" + JsonConvert.SerializeObject(ex));
                return ex.Message;
            }
        }
    }
    // Customized ILoggerProvider, writes logs to text files
    public class CustomFileLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        private readonly StreamWriter _logFileWriter;

        public CustomFileLoggerProvider(StreamWriter logFileWriter)
        {
            _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
        }

        public void Dispose()
        {
            _logFileWriter.Dispose();
        }

        Microsoft.Extensions.Logging.ILogger ILoggerProvider.CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName, _logFileWriter);
        }
    }

    // Customized ILogger, writes logs to text files
    public class CustomFileLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _categoryName;
        private readonly StreamWriter _logFileWriter;

        public CustomFileLogger(string categoryName, StreamWriter logFileWriter)
        {
            _categoryName = categoryName;
            _logFileWriter = logFileWriter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Ensure that only information level and higher logs are recorded
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            // Ensure that only information level and higher logs are recorded
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Get the formatted log message
            var message = formatter(state, exception);

            //Write log messages to text file
            _logFileWriter.WriteLine($"[{logLevel}] [{_categoryName}] {message}");
            _logFileWriter.Flush();
        }
    }
}



