﻿using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace NLog.Extensions.AzureTableStorage.Partition
{
    class NLogEntity : TableEntity
    {
        public string LogTimeStamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public string FullMessage { get; set; }
        public string MachineName { get; set; }

        public NLogEntity(LogEventInfo logEvent, string layoutMessage, string machineName, string partitionKey, string logTimeStampFormat)
        {
            FullMessage = layoutMessage;
            Level = logEvent.Level.Name;
            LoggerName = logEvent.LoggerName;
            Message = logEvent.Message;
            LogTimeStamp = logEvent.TimeStamp.ToString(logTimeStampFormat);
            MachineName = machineName;
            if (logEvent.Exception != null)
            {
                var exception = logEvent.Exception;
                var innerException = exception.InnerException;
                if (exception is AggregateException aggregateException)
                {
                    var innerExceptions = aggregateException.Flatten();
                    if (innerExceptions.InnerExceptions?.Count == 1)
                    {
                        exception = innerExceptions.InnerExceptions[0];
                        innerException = null;
                    }
                    else
                    {
                        innerException = innerExceptions;
                    }
                }

                Exception = string.Concat(exception.Message, " - ", exception.GetType().ToString());
                StackTrace = exception.StackTrace;
                if (innerException != null)
                {
                    InnerException = innerException.ToString();
                }
            }
            RowKey = string.Concat((DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"), "__", Guid.NewGuid().ToString());
            PartitionKey = partitionKey;
        }
        public NLogEntity() { }
    }
}
