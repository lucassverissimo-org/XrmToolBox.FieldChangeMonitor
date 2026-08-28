using System;
using System.Collections.Generic;

namespace LucasVerissimo.XrmToolBox.SolutionLayerAnalyzer.Models
{
    public enum ComponentCorrelationStatus
    {
        Matched,
        MissingFromTargetSolution,
        MissingFromSourceSolution,
        MissingFromTargetEnvironment,
    }

    public enum BackupStatus
    {
        NotRequested,
        Succeeded,
        PartiallySucceeded,
        Failed,
        Skipped,
        Cancelled,
    }

    public enum RemovalStatus
    {
        NotStarted,
        Removed,
        Failed,
        Cancelled,
    }

    public enum ValidationStatus
    {
        NotStarted,
        RemovedAndValidated,
        RemovalSucceededButValidationFailed,
        ActiveLayerStillPresent,
        Cancelled,
    }

    public enum AnalysisStage
    {
        LoadingSourceComponents,
        LoadingTargetComponents,
        CorrelatingComponents,
        QueryingTargetLayers,
        FinalizingResults,
    }

    public enum RemovalStage
    {
        RemovingActiveLayer,
        ValidatingRemoval,
        ComponentCompleted,
    }

    public sealed class SolutionInfo
    {
        public Guid SolutionId { get; set; }

        public string FriendlyName { get; set; }

        public string UniqueName { get; set; }

        public string Version { get; set; }

        public bool IsManaged { get; set; }

        public override string ToString()
        {
            return FriendlyName + " (" + UniqueName + ") - " + Version;
        }
    }

    public sealed class SolutionComponentReference
    {
        public Guid SolutionComponentId { get; set; }

        public Guid? ObjectId { get; set; }

        public int ComponentType { get; set; }

        public Guid? RootSolutionComponentId { get; set; }

        public int? RootComponentBehavior { get; set; }

        public string FormattedComponentTypeName { get; set; }
    }

    public sealed class ComponentIdentity : IEquatable<ComponentIdentity>
    {
        public ComponentIdentity(int componentType, Guid componentId)
        {
            ComponentType = componentType;
            ComponentId = componentId;
        }

        public int ComponentType { get; private set; }

        public Guid ComponentId { get; private set; }

        public bool Equals(ComponentIdentity other)
        {
            return other != null
                && ComponentType == other.ComponentType
                && ComponentId == other.ComponentId;
        }

        public override bool Equals(object value)
        {
            return Equals(value as ComponentIdentity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ComponentType * 397) ^ ComponentId.GetHashCode();
            }
        }

        public override string ToString()
        {
            return ComponentType + ":" + ComponentId.ToString("D");
        }
    }

    public sealed class LayerInfo
    {
        public string ComponentId { get; set; }

        public string ComponentName { get; set; }

        public string SolutionComponentName { get; set; }

        public string SolutionName { get; set; }

        public string PublisherName { get; set; }

        public int? Order { get; set; }

        public bool IsActiveLayer
        {
            get
            {
                return string.Equals(SolutionName, "Active", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(SolutionName, "Unmanaged", StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    public sealed class LayerAnalysisResult
    {
        public bool Selected { get; set; }

        public int ComponentType { get; set; }

        public string ComponentTypeName { get; set; }

        public string LayerComponentName { get; set; }

        public Guid ComponentId { get; set; }

        public Guid? SourceComponentId { get; set; }

        public Guid? TargetComponentId { get; set; }

        public string ComponentName { get; set; }

        public bool ExistsInSourceSolution { get; set; }

        public bool ExistsInTargetSolution { get; set; }

        public bool ExistsInTargetEnvironment { get; set; }

        public ComponentCorrelationStatus CorrelationStatus { get; set; }

        public bool? HasActiveLayer { get; set; }

        public int LayerCount { get; set; }

        public string Status { get; set; }

        public string Error { get; set; }

        public long DurationMs { get; set; }

        public string ActiveLayerDisplay
        {
            get
            {
                if (!HasActiveLayer.HasValue)
                {
                    return "-";
                }

                return HasActiveLayer.Value ? "Yes" : "No";
            }
        }

        public string SourceSolutionDisplay
        {
            get { return ExistsInSourceSolution ? "Yes" : "No"; }
        }

        public string TargetSolutionDisplay
        {
            get { return ExistsInTargetSolution ? "Yes" : "No"; }
        }

        public string TargetEnvironmentDisplay
        {
            get { return ExistsInTargetEnvironment ? "Yes" : "No"; }
        }
    }

    public sealed class AnalysisMetricsSnapshot
    {
        public int TotalSourceComponents { get; set; }

        public int TotalTargetSolutionComponents { get; set; }

        public int ProcessedComponents { get; set; }

        public int FoundInTargetEnvironment { get; set; }

        public int MissingFromTargetEnvironment { get; set; }

        public int Matched { get; set; }

        public int MissingFromTargetSolution { get; set; }

        public int MissingFromSourceSolution { get; set; }

        public int ActiveLayers { get; set; }

        public int WithoutActiveLayer { get; set; }

        public int Errors { get; set; }

        public int Batches { get; set; }

        public int Requests { get; set; }

        public int Retries { get; set; }

        public int Throttlings { get; set; }

        public int Timeouts { get; set; }

        public TimeSpan Elapsed { get; set; }

        public double AverageBatchDurationMs { get; set; }
    }

    public sealed class AnalysisProgress
    {
        public AnalysisStage Stage { get; set; }

        public string Message { get; set; }

        public LayerAnalysisResult Result { get; set; }

        public AnalysisMetricsSnapshot Metrics { get; set; }
    }

    public sealed class AnalysisExecutionResult
    {
        public IReadOnlyCollection<LayerAnalysisResult> Results { get; set; }

        public AnalysisMetricsSnapshot Metrics { get; set; }

        public bool WasCancelled { get; set; }
    }

    public sealed class BackupResult
    {
        public bool Requested { get; set; }

        public BackupStatus Status { get; set; }

        public string FilePath { get; set; }

        public string TemporarySolutionUniqueName { get; set; }

        public int SelectedComponents { get; set; }

        public int AddedComponents { get; set; }

        public string Error { get; set; }

        public bool IsConfirmed
        {
            get { return Status == BackupStatus.Succeeded; }
        }
    }

    public sealed class RemovalResult
    {
        public Guid ComponentId { get; set; }

        public string ComponentName { get; set; }

        public int ComponentType { get; set; }

        public string ComponentTypeName { get; set; }

        public bool ExistsInTargetSolution { get; set; }

        public BackupStatus BackupStatus { get; set; }

        public RemovalStatus RemovalStatus { get; set; }

        public ValidationStatus ValidationStatus { get; set; }

        public string Error { get; set; }

        public long DurationMs { get; set; }

        public DateTime TimestampUtc { get; set; }

        public bool UserConfirmedRisk { get; set; }
    }

    public sealed class RemovalProgress
    {
        public RemovalStage Stage { get; set; }

        public int Current { get; set; }

        public int Total { get; set; }

        public RemovalResult Result { get; set; }

        public string Message { get; set; }
    }
}
