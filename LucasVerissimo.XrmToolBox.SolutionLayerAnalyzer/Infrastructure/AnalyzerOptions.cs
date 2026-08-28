using System;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Infrastructure
{
    public sealed class AnalyzerOptions
    {
        public const int DefaultBatchSize = 25;
        public const int DefaultMinimumBatchSize = 1;
        public const int DefaultMaxDegreeOfParallelism = 2;
        public const int DefaultMaxRetries = 3;

        public int BatchSize { get; set; } = DefaultBatchSize;

        public int MinimumBatchSize { get; set; } = DefaultMinimumBatchSize;

        public int MaxDegreeOfParallelism { get; set; } = DefaultMaxDegreeOfParallelism;

        public int MaxRetries { get; set; } = DefaultMaxRetries;

        public TimeSpan InitialRetryDelay { get; set; } = TimeSpan.FromSeconds(2);

        public void Validate()
        {
            if (BatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(BatchSize));
            }

            if (MinimumBatchSize <= 0 || MinimumBatchSize > BatchSize)
            {
                throw new ArgumentOutOfRangeException(nameof(MinimumBatchSize));
            }

            if (MaxDegreeOfParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxDegreeOfParallelism));
            }

            if (MaxRetries < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxRetries));
            }
        }
    }
}
