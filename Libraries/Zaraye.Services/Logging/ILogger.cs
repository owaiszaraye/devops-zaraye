using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Logging;

namespace Zaraye.Services.Logging
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public partial interface ILogger
    {
        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Clears a log
        /// </summary>
        /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ClearLogAsync(DateTime? olderThan = null);

      

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a log item
        /// </returns>
        Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null);

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>
        /// Log item
        /// </returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null);

       
        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Information(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task WarningAsync(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Warning(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ErrorAsync(string message, Exception exception = null, Customer customer = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        void Error(string message, Exception exception = null, Customer customer = null);
    }
}