using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Logging;
using Zaraye.Data;

namespace Zaraye.Services.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        
        private readonly IRepository<Log> _logRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public DefaultLogger(CommonSettings commonSettings,
            IRepository<Log> logRepository,
            IWebHelper webHelper)
        {
            _commonSettings = commonSettings;
            _logRepository = logRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether this message should not be logged
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Result</returns>
        protected virtual bool IgnoreLog(string message)
        {
            if (!_commonSettings.IgnoreLogWordlist.Any())
                return false;

            if (string.IsNullOrWhiteSpace(message))
                return false;

            return _commonSettings
                .IgnoreLogWordlist
                .Any(x => message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => false,
                _ => true,
            };
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearLogAsync(DateTime? olderThan = null)
        {
            if(olderThan == null)
                await _logRepository.TruncateAsync();
            else
                await _logRepository.DeleteAsync(p => p.CreatedOnUtc < olderThan.Value);
        }


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
        public virtual async Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            //check ignore word/phrase list?
            if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
                return null;

            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                CustomerId = customer?.Id,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            await _logRepository.InsertAsync(log, false);

            return log;
        }

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
        public virtual Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            //check ignore word/phrase list?
            if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
                return null;

            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                CustomerId = customer?.Id,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            _logRepository.Insert(log, false);

            return log;
        }

        
        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Information(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Information))
                InsertLog(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task WarningAsync(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Warning))
                await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Warning(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Warning))
                InsertLog(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ErrorAsync(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        public virtual void Error(string message, Exception exception = null, Customer customer = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
        }

        #endregion
    }
}