// namespace JustinWritesCode.Common;
// using System.Text.Json.Serialization;

// public enum RestoreEventType
// {
//     RestoredNew,
//     RestoredMissing,
//     Overwritten,
//     SkippedAlreadyExists,
//     SkippedNotEmpty,
//     CompleteCleanse
// }

// public abstract class RestoreEvent
// {
//     protected RestoreEvent() { }
//     protected RestoreEvent(DirectoryInfo? ProjectDirectory, DateTime Timestamp)
//     {
//         this.ProjectDirectory = ProjectDirectory;
//         this.Timestamp = Timestamp;
//     }

//     public DirectoryInfo? ProjectDirectory { get; init; }
//     public DateTime Timestamp { get; init; }
//     public abstract RestoreEventType Type { get; }
// }

// public class CleansedEvent : RestoreEvent
// {
//     public CleansedEvent() { }
//     public CleansedEvent(DirectoryInfo? ProjectDirectory, DateTime Timestamp) : base(ProjectDirectory, Timestamp) { }
//     public override RestoreEventType Type => RestoreEventType.CompleteCleanse;
// }
