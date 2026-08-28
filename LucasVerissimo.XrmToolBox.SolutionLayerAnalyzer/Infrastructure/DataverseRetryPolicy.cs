using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Microsoft.Xrm.Sdk;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure
{
    internal sealed class DataverseRetryPolicy
    {
        private const int ThrottlingErrorCode = -2147015902;
        private const int RateLimitExceededErrorCode = -2147015903;
        private const int ConcurrencyLimitExceededErrorCode = -2147015898;
        private readonly AnalyzerOptions options;
        private readonly Action<string> log;

        public DataverseRetryPolicy(AnalyzerOptions options, Action<string> log)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.log = log ?? delegate { };
        }

        public T Execute<T>(
            Func<T> operation,
            string operationName,
            AnalysisMetrics metrics,
            CancellationToken cancellationToken,
            bool retryTimeouts = true
        )
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            Exception lastError = null;
            for (var attempt = 0; attempt <= options.MaxRetries; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    metrics?.RecordRequest();
                    return operation();
                }
                catch (Exception error)
                {
                    lastError = error;
                    var isThrottling = IsThrottling(error);
                    var isTimeout = IsTimeout(error);
                    var isTransient =
                        isThrottling || (isTimeout && retryTimeouts) || IsTransient(error);
                    if (isTimeout && !retryTimeouts)
                    {
                        isTransient = false;
                    }

                    if (isThrottling)
                    {
                        metrics?.RecordThrottling();
                    }

                    if (isTimeout)
                    {
                        metrics?.RecordTimeout();
                    }

                    if (!isTransient || attempt == options.MaxRetries)
                    {
                        throw;
                    }

                    metrics?.RecordRetry();
                    var delay = GetRetryDelay(error, attempt);
                    log(
                        operationName
                            + " retry "
                            + (attempt + 1)
                            + "/"
                            + options.MaxRetries
                            + " after "
                            + delay.TotalSeconds.ToString("0.0")
                            + "s: "
                            + error.Message
                    );

                    if (cancellationToken.WaitHandle.WaitOne(delay))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            }

            throw lastError ?? new InvalidOperationException(operationName + " failed.");
        }

        public static bool IsTimeout(Exception error)
        {
            if (error == null)
            {
                return false;
            }

            if (error is TimeoutException)
            {
                return true;
            }

            return Contains(error.Message, "timeout")
                || Contains(error.Message, "timed out")
                || Contains(error.Message, "tempo limite")
                || Contains(error.Message, "canal de solicitação")
                || IsTimeout(error.InnerException);
        }

        public static bool IsThrottling(Exception error)
        {
            var fault = FindOrganizationServiceFault(error);
            if (fault != null)
            {
                return fault.ErrorCode == ThrottlingErrorCode
                    || fault.ErrorCode == RateLimitExceededErrorCode
                    || fault.ErrorCode == ConcurrencyLimitExceededErrorCode
                    || Contains(fault.Message, "429")
                    || Contains(fault.Message, "throttl")
                    || Contains(fault.Message, "rate limit")
                    || Contains(fault.Message, "service protection");
            }

            return Contains(error?.Message, "429")
                || Contains(error?.Message, "throttl")
                || Contains(error?.Message, "rate limit");
        }

        public static bool IsTransient(Exception error)
        {
            if (error == null)
            {
                return false;
            }

            return IsThrottling(error)
                || IsTimeout(error)
                || error is CommunicationException
                || Contains(error.Message, "temporar")
                || Contains(error.Message, "server busy")
                || Contains(error.Message, "connection was closed")
                || Contains(error.Message, "connection reset")
                || IsTransient(error.InnerException);
        }

        private TimeSpan GetRetryDelay(Exception error, int attempt)
        {
            var fault = FindOrganizationServiceFault(error);
            if (fault != null)
            {
                object retryAfter;
                if (
                    fault.ErrorDetails != null
                    && fault.ErrorDetails.TryGetValue("Retry-After", out retryAfter)
                )
                {
                    var retryDelay = ConvertRetryAfter(retryAfter);
                    if (retryDelay.HasValue)
                    {
                        return retryDelay.Value;
                    }
                }
            }

            var multiplier = Math.Pow(2, attempt);
            return TimeSpan.FromMilliseconds(
                options.InitialRetryDelay.TotalMilliseconds * multiplier
            );
        }

        private static TimeSpan? ConvertRetryAfter(object value)
        {
            if (value is TimeSpan)
            {
                return (TimeSpan)value;
            }

            if (value is int)
            {
                return TimeSpan.FromSeconds((int)value);
            }

            if (value is long)
            {
                return TimeSpan.FromSeconds((long)value);
            }

            double seconds;
            if (double.TryParse(Convert.ToString(value), out seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return null;
        }

        private static OrganizationServiceFault FindOrganizationServiceFault(Exception error)
        {
            var current = error;
            while (current != null)
            {
                var faultException = current as FaultException<OrganizationServiceFault>;
                if (faultException != null)
                {
                    return faultException.Detail;
                }

                current = current.InnerException;
            }

            return null;
        }

        private static bool Contains(string value, string text)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
